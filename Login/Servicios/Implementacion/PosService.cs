using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Models.ViewModels.Pos;
using Plataforma.Servicios.Contrato;
using System.Security.Claims;

namespace Plataforma.Servicios.Implementacion
{
    public class PosService : IPosService
    {
        private readonly BaseAdmContext _dbContext;
        private readonly ICxcService _cxcService;
        public PosService(BaseAdmContext dbContext, ICxcService cxcService)
        {
            _dbContext = dbContext;
            _cxcService = cxcService;
        }
        public async Task<PosPantallaViewModel> ObtenerPantallaAsync(ClaimsPrincipal usuario)
        {
            var ctx = await ObtenerContextoCajaAsync(usuario);

            var clientes = await _dbContext.Clientes
                .OrderBy(c => c.NombreCliente)
                .Select(c => new SelectListItem
                {
                    Value = c.IdCliente.ToString(),
                    Text = (c.NombreCliente ?? "Sin nombre") +
                           (c.CedulaCliente.HasValue ? $" - {c.CedulaCliente}" : "")
                })
                .ToListAsync();

            var categorias = await (
                from inv in _dbContext.InventarioSedes
                join p in _dbContext.Productos on inv.ProductoId equals p.Cod_Producto
                join c in _dbContext.CategoriaProductos on p.IdCatepro equals c.IdCateProducto into catJoin
                from cat in catJoin.DefaultIfEmpty()
                where inv.SedeId == ctx.IdSede
                      && inv.Cantidad > 0
                      && p.Estado == 1
                select cat != null ? (cat.Descripcion ?? "Sin categoría") : "Sin categoría"
            )
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();

            return new PosPantallaViewModel
            {
                IdSede = ctx.IdSede,
                InfoPdvId = ctx.InfoPdvId,
                NombreSede = ctx.NombreSede ?? $"Sede {ctx.IdSede}",
                Clientes = clientes,
                Categorias = categorias
            };
        }

        public async Task<PosProductosPaginadosVm> BuscarProductosAsync(string? texto, string? categoria, int pagina, int tamanoPagina, ClaimsPrincipal usuario)
        {
            var ctx = await ObtenerContextoCajaAsync(usuario);

            if (pagina <= 0) pagina = 1;
            if (tamanoPagina <= 0) tamanoPagina = 24;

            var query =
                from inv in _dbContext.InventarioSedes
                join p in _dbContext.Productos on inv.ProductoId equals p.Cod_Producto
                join c in _dbContext.CategoriaProductos on p.IdCatepro equals c.IdCateProducto into catJoin
                from cat in catJoin.DefaultIfEmpty()
                where inv.SedeId == ctx.IdSede
                      && inv.Cantidad > 0
                      && p.Estado == 1
                select new PosProductoVm
                {
                    Codigo = p.Cod_Producto ?? "",
                    Nombre = p.NombreProducto ?? "Sin nombre",
                    Categoria = cat != null ? (cat.Descripcion ?? "Sin categoría") : "Sin categoría",
                    ValorUnidad = p.ValorUnidad,
                    ValorVenta = p.ValorVentaProducto ?? 0,
                    StockDisponible = inv.Cantidad,
                    ImagenPath = p.ImagenPath
                };

            if (!string.IsNullOrWhiteSpace(texto))
            {
                texto = texto.Trim();

                query = query.Where(x =>
                    x.Codigo.Contains(texto) ||
                    x.Nombre.Contains(texto));
            }

            if (!string.IsNullOrWhiteSpace(categoria))
            {
                query = query.Where(x => x.Categoria == categoria);
            }

            var totalRegistros = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.Nombre)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();

            return new PosProductosPaginadosVm
            {
                Items = items,
                PaginaActual = pagina,
                TamanoPagina = tamanoPagina,
                TotalRegistros = totalRegistros,
                TotalPaginas = (int)Math.Ceiling(totalRegistros / (double)tamanoPagina)
            };
        }

        public async Task<PosProductoVm?> BuscarProductoPorCodigoAsync(string codigo, ClaimsPrincipal usuario)
        {
            var ctx = await ObtenerContextoCajaAsync(usuario);

            return await (
                from inv in _dbContext.InventarioSedes
                join p in _dbContext.Productos on inv.ProductoId equals p.Cod_Producto
                join c in _dbContext.CategoriaProductos on p.IdCatepro equals c.IdCateProducto into catJoin
                from cat in catJoin.DefaultIfEmpty()
                where inv.SedeId == ctx.IdSede
                      && inv.Cantidad > 0
                      && p.Estado == 1
                      && p.Cod_Producto == codigo
                select new PosProductoVm
                {
                    Codigo = p.Cod_Producto ?? "",
                    Nombre = p.NombreProducto ?? "Sin nombre",
                    Categoria = cat != null ? (cat.Descripcion ?? "Sin categoría") : "Sin categoría",
                    ValorUnidad = p.ValorUnidad,
                    ValorVenta = p.ValorVentaProducto ?? 0,
                    StockDisponible = inv.Cantidad,
                    ImagenPath = p.ImagenPath
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<PosResultadoVm> FacturarVentaPosAsync(PosCrearVentaRequest request, ClaimsPrincipal usuario)
        {
            try
            {
                if (request.Items == null || !request.Items.Any())
                {
                    return new PosResultadoVm
                    {
                        Ok = false,
                        Mensaje = "No hay productos en la venta."
                    };
                }

                var ctx = await ObtenerContextoCajaAsync(usuario);

                var codigos = request.Items
                    .Where(x => !string.IsNullOrWhiteSpace(x.Codigo))
                    .Select(x => x.Codigo.Trim())
                    .Distinct()
                    .ToList();

                var productosDb = await (
                    from inv in _dbContext.InventarioSedes
                    join p in _dbContext.Productos on inv.ProductoId equals p.Cod_Producto
                    where inv.SedeId == ctx.IdSede
                          && codigos.Contains(inv.ProductoId)
                          && p.Estado == 1
                    select new
                    {
                        Producto = p,
                        Inventario = inv
                    }
                ).ToListAsync();

                var mapa = productosDb.ToDictionary(x => x.Producto.Cod_Producto!, x => x);

                foreach (var item in request.Items)
                {
                    if (!mapa.ContainsKey(item.Codigo))
                    {
                        return new PosResultadoVm
                        {
                            Ok = false,
                            Mensaje = $"El producto {item.Codigo} no existe en el inventario de la sede actual."
                        };
                    }

                    var registro = mapa[item.Codigo];

                    if (item.Cantidad <= 0)
                    {
                        return new PosResultadoVm
                        {
                            Ok = false,
                            Mensaje = $"La cantidad del producto {item.Codigo} es inválida."
                        };
                    }

                    if (registro.Inventario.Cantidad < item.Cantidad)
                    {
                        return new PosResultadoVm
                        {
                            Ok = false,
                            Mensaje = $"Stock insuficiente para {item.Codigo}. Disponible: {registro.Inventario.Cantidad}."
                        };
                    }
                }

                decimal subtotal = 0m;
                decimal ivaTotal = 0m;
                decimal totalGeneral = 0m;

                decimal totalPagado =
                    request.MontoNequi +
                    request.MontoTarjeta +
                    request.MontoEfectivo +
                    request.MontoCredito;

                await using var trx = await _dbContext.Database.BeginTransactionAsync();

                try
                {
                    var cliente = await _dbContext.Clientes
                        .FirstOrDefaultAsync(c => c.IdCliente == request.IdCliente);

                    if (cliente == null)
                    {
                        await trx.RollbackAsync();
                        return new PosResultadoVm
                        {
                            Ok = false,
                            Mensaje = "El cliente seleccionado no existe."
                        };
                    }

                    if (request.MontoCredito > 0 && !request.FechaVencimientoCredito.HasValue)
                    {
                        await trx.RollbackAsync();
                        return new PosResultadoVm
                        {
                            Ok = false,
                            Mensaje = "Debe indicar fecha de vencimiento para el crédito."
                        };
                    }

                    var venta = new Ventas
                    {
                        IdCliente = request.IdCliente,
                        CedulaCliente = cliente.CedulaCliente ?? 0,
                        Cedula = ctx.Cedula,
                        FechaVenta = DateTime.Now,
                        EstadoVenta = "Facturada",
                        MetodoPago = ConstruirMetodoPago(request),
                        Total = 0,
                        Conceptos = request.Conceptos ?? "",
                        TipoVenta = "Normal"
                    };

                    _dbContext.Ventas.Add(venta);
                    await _dbContext.SaveChangesAsync();

                    foreach (var item in request.Items)
                    {
                        var data = mapa[item.Codigo];
                        var producto = data.Producto;
                        var inventario = data.Inventario;

                        decimal valorVentaUnitario = producto.ValorVentaProducto ?? 0m;
                        decimal? valorUnidadUnitario = producto.ValorUnidad;

                        decimal valorVentaTotal = item.Cantidad * valorVentaUnitario;
                        decimal? valorUnidadTotal = item.Cantidad * valorUnidadUnitario;
                        decimal valorIvaLinea = valorVentaTotal * (item.IvaPorcentaje / 100m);
                        decimal subtotalLinea = valorVentaTotal + valorIvaLinea;

                        inventario.Cantidad -= item.Cantidad;
                        inventario.ActualizadoEn = DateTime.Now;
                        inventario.Cedula = ctx.Cedula;

                        var pedido = new Pedidos
                        {
                            IdVenta = venta.IdVenta,
                            Codigo = producto.Cod_Producto,
                            Stock = item.Cantidad,
                            VUnidad = valorUnidadTotal,
                            VNeto = valorUnidadUnitario,
                            VVenta = valorVentaTotal,
                            InfopdvId = ctx.InfoPdvId,
                            FechaRegistro = DateTime.Now,
                            IvaPorcentaje = item.IvaPorcentaje,
                            IvaValor = valorIvaLinea,
                            SubTotal = subtotalLinea
                        };

                        _dbContext.Pedidos.Add(pedido);

                        subtotal += valorVentaTotal;
                        ivaTotal += valorIvaLinea;
                        totalGeneral += subtotalLinea;
                    }

                    if (totalPagado < totalGeneral)
                    {
                        await trx.RollbackAsync();
                        return new PosResultadoVm
                        {
                            Ok = false,
                            Mensaje = $"Pago incompleto. Total: {totalGeneral:N2}, pagado: {totalPagado:N2}."
                        };
                    }

                    venta.Total = totalGeneral;

                    var numeroFactura = await GenerarNumeroFacturaAsync("POS");

                    var factura = new Factura
                    {
                        NumeroFactura = numeroFactura,
                        IdVenta = venta.IdVenta,
                        FechaEmision = DateTime.Now,
                        SubTotal = subtotal,
                        IVA = ivaTotal,
                        Total = totalGeneral,
                        EstadoFactura = "Generada",
                        EsElectronica = false,
                        EstadoDian = "NoAplica"
                    };

                    _dbContext.Factura.Add(factura);

                    venta.NumeroFactura = numeroFactura;
                    venta.FechaEmisionFactura = factura.FechaEmision;

                    if (request.MontoCredito > 0)
                    {
                        var resultadoCxc = await _cxcService.CrearDesdeVentaAsync(
                            venta.IdVenta,
                            venta.IdCliente,
                            request.MontoCredito,
                            request.FechaVencimientoCredito,
                            "Generada automáticamente desde POS"
                        );

                        if (!resultadoCxc.Ok)
                            throw new Exception(resultadoCxc.Mensaje);
                    }

                    await _dbContext.SaveChangesAsync();
                    await trx.CommitAsync();

                    return new PosResultadoVm
                    {
                        Ok = true,
                        Mensaje = "Venta POS facturada correctamente.",
                        IdVenta = venta.IdVenta,
                        IdFactura = factura.IdFactura,
                        NumeroFactura = numeroFactura
                    };
                }
                catch (Exception ex)
                {
                    await trx.RollbackAsync();
                    return new PosResultadoVm
                    {
                        Ok = false,
                        Mensaje = $"Error al facturar en POS: {ex.Message}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new PosResultadoVm
                {
                    Ok = false,
                    Mensaje = $"Error general en POS: {ex.Message}"
                };
            }
        }

        private async Task<(int Cedula, int IdSede, int InfoPdvId, string? NombreSede)> ObtenerContextoCajaAsync(ClaimsPrincipal usuario)
        {
            var cedulaClaim = usuario.FindFirst("Cedula")?.Value;

            if (string.IsNullOrWhiteSpace(cedulaClaim))
                throw new Exception("No se encontró la cédula del usuario autenticado.");

            if (!int.TryParse(cedulaClaim, out int cedula))
                throw new Exception("La cédula del usuario autenticado no es válida.");

            var sede = await (
                from se in _dbContext.Sedeempleado
                join s in _dbContext.Sede on se.Id_sede equals s.Id_sede
                where se.Cedula == cedula
                select new
                {
                    s.Id_sede,
                    s.NombreSede
                }
            ).FirstOrDefaultAsync();

            if (sede == null)
                throw new Exception("El cajero no tiene una sede asignada.");

            var pdv = await _dbContext.Infopdv
                .FirstOrDefaultAsync(x => x.Id_Sede == sede.Id_sede);

            if (pdv == null)
                throw new Exception("La sede del cajero no tiene un PDV configurado.");

            return (cedula, sede.Id_sede, pdv.InfopdvId, sede.NombreSede);
        }

        private static string ConstruirMetodoPago(PosCrearVentaRequest request)
        {
            var metodos = new List<string>();

            if (request.MontoNequi > 0) metodos.Add("Nequi");
            if (request.MontoTarjeta > 0) metodos.Add("Tarjeta");
            if (request.MontoEfectivo > 0) metodos.Add("Efectivo");
            if (request.MontoCredito > 0) metodos.Add("Crédito");

            return metodos.Count == 0 ? "Sin definir" : string.Join(" + ", metodos);
        }

        private async Task<string> GenerarNumeroFacturaAsync(string prefijo)
        {
            var fecha = DateTime.Now.ToString("yyyyMMdd");

            var consecutivoHoy = await _dbContext.Factura
                .CountAsync(f => f.NumeroFactura != null &&
                                 f.NumeroFactura.StartsWith($"{prefijo}-{fecha}-"));

            return $"{prefijo}-{fecha}-{(consecutivoHoy + 1):D4}";
        }
    }
}
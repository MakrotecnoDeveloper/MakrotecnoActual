using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using Plataforma.ViewModels.Cotizacion;
using System.Security.Claims;

namespace Plataforma.Servicios.Implementacion;

public class CotizacionService : ICotizacionService
{
    private readonly BaseAdmContext _dbContext;

    public CotizacionService(BaseAdmContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<CotizacionIndexVm>> ObtenerCotizacionesAsync(
        ClaimsPrincipal usuario,
        string? estado,
        string? buscar)
    {
        var ctx = ObtenerContextoClaims(usuario);

        var query = _dbContext.Cotizaciones
            .AsNoTracking()
            .Where(c =>
                c.IdEmpresa == ctx.EmpresaId &&
                c.SedeId == ctx.SedeId &&
                c.InfopdvId == ctx.PdvId &&
                c.Activo);

        if (!string.IsNullOrWhiteSpace(estado))
        {
            query = query.Where(c => c.EstadoCotizacion == estado);
        }

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            buscar = buscar.Trim();

            query = query.Where(c =>
                c.NumeroCotizacion.Contains(buscar) ||
                c.NombreCliente!.Contains(buscar) ||
                c.TelefonoCliente!.Contains(buscar));
        }

        return await query
            .OrderByDescending(c => c.FechaCreacion)
            .Select(c => new CotizacionIndexVm
            {
                IdCotizacion = c.IdCotizacion,
                NumeroCotizacion = c.NumeroCotizacion,
                NombreCliente = c.NombreCliente ?? "",
                FechaCreacion = c.FechaCreacion,
                FechaVencimiento = c.FechaVencimiento,
                EstadoCotizacion = c.EstadoCotizacion,
                Total = c.Total,
                IdVentaGenerada = c.IdVentaGenerada
            })
            .ToListAsync();
    }

    public async Task<CotizacionCrearVm> ConstruirCrearVmAsync(ClaimsPrincipal usuario)
    {
        var clientes = await _dbContext.Clientes
            .OrderBy(c => c.NombreCliente)
            .Select(c => new SelectListItem
            {
                Value = c.IdCliente.ToString(),
                Text = $"{c.NombreCliente} - {c.CedulaCliente}"
            })
            .ToListAsync();

        return new CotizacionCrearVm
        {
            NumeroCotizacion = await GenerarNumeroCotizacionAsync(),
            FechaCreacion = DateTime.Now,
            Clientes = clientes
        };
    }

    public async Task<int> CrearCotizacionAsync(CotizacionCrearVm vm, ClaimsPrincipal usuario)
    {
        var ctx = ObtenerContextoClaims(usuario);

        var cliente = await _dbContext.Clientes
            .FirstOrDefaultAsync(c => c.IdCliente == vm.IdCliente);

        if (cliente == null)
            throw new Exception("El cliente seleccionado no existe.");

        var detallesValidos = vm.Detalles
            .Where(d => !string.IsNullOrWhiteSpace(d.CodigoProducto) && d.Cantidad > 0)
            .ToList();

        if (!detallesValidos.Any())
            throw new Exception("Debe agregar al menos un producto a la cotización.");

        var codigos = detallesValidos
            .Select(d => d.CodigoProducto)
            .Distinct()
            .ToList();

        var productos = await _dbContext.Productos
            .Where(p =>
                codigos.Contains(p.Cod_Producto!) &&
                p.ID_Empresa == ctx.EmpresaId &&
                p.Estado == 1)
            .ToListAsync();

        if (productos.Count != codigos.Count)
            throw new Exception("Uno o varios productos no pertenecen a la empresa actual o no están activos.");

        var inventarios = await _dbContext.InventarioSedes
            .Where(i => i.SedeId == ctx.SedeId && codigos.Contains(i.ProductoId))
            .ToDictionaryAsync(i => i.ProductoId, i => i.Cantidad);

        foreach (var item in detallesValidos)
        {
            if (!inventarios.TryGetValue(item.CodigoProducto, out var stockDisponible))
                throw new Exception($"No existe inventario para el producto {item.CodigoProducto} en la sede actual.");

            if (stockDisponible <= 0)
                throw new Exception($"El producto {item.CodigoProducto} no tiene stock disponible.");

            if (item.Cantidad > stockDisponible)
                throw new Exception($"La cantidad cotizada del producto {item.CodigoProducto} supera el stock disponible.");
        }

        decimal subtotal = 0;
        decimal ivaTotal = 0;

        var detalles = new List<CotizacionDetalle>();

        foreach (var item in detallesValidos)
        {
            var producto = productos.First(p => p.Cod_Producto == item.CodigoProducto);

            var valorVenta = item.ValorVenta > 0
                ? item.ValorVenta
                : producto.ValorVentaProducto ?? 0m;

            var valorNeto = item.ValorNeto > 0
                ? item.ValorNeto
                : producto.ValorNetoProducto ?? 0m;

            var valorUnidad = item.ValorUnidad > 0
                ? item.ValorUnidad
                : producto.ValorUnidad ?? 0m;

            var ivaPorcentaje = item.IvaPorcentaje > 0
                ? item.IvaPorcentaje
                : producto.Iva ?? 0m;

            var baseLinea = valorVenta * item.Cantidad;
            var ivaLinea = baseLinea * (ivaPorcentaje / 100m);
            var totalLinea = baseLinea + ivaLinea;

            subtotal += baseLinea;
            ivaTotal += ivaLinea;

            detalles.Add(new CotizacionDetalle
            {
                CodigoProducto = producto.Cod_Producto!,
                NombreProducto = producto.NombreProducto ?? "",
                Cantidad = item.Cantidad,
                ValorNeto = valorNeto,
                ValorUnidad = valorUnidad,
                ValorVenta = valorVenta,
                IvaPorcentaje = ivaPorcentaje,
                IvaValor = ivaLinea,
                SubTotal = baseLinea,
                TotalLinea = totalLinea,
                IncluidoEnVenta = true,
                Observacion = item.Observacion
            });
        }

        var total = subtotal + ivaTotal - vm.Descuento;

        if (total < 0)
            throw new Exception("El total de la cotización no puede ser negativo.");

        var cotizacion = new Cotizacion
        {
            NumeroCotizacion = await GenerarNumeroCotizacionAsync(),
            IdCliente = cliente.IdCliente,
            CedulaCliente = cliente.CedulaCliente,
            NombreCliente = cliente.NombreCliente,
            DocumentoCliente = cliente.CedulaCliente?.ToString(),
            TelefonoCliente = cliente.TelefonoCliente,
            CorreoCliente = cliente.CorreoCliente,
            DireccionCliente = cliente.DireccionCliente,
            FechaCreacion = DateTime.Now,
            FechaVencimiento = vm.FechaVencimiento,
            EstadoCotizacion = "Pendiente",
            ObservacionGeneral = vm.ObservacionGeneral,
            Subtotal = subtotal,
            IvaTotal = ivaTotal,
            Descuento = vm.Descuento,
            Total = total,
            IdEmpresa = ctx.EmpresaId,
            SedeId = ctx.SedeId,
            InfopdvId = ctx.PdvId,
            CedulaUsuario = ctx.Cedula,
            CreadoPor = usuario.Identity?.Name,
            Activo = true,
            Detalles = detalles
        };

        cotizacion.Seguimientos.Add(new CotizacionSeguimiento
        {
            EstadoAnterior = "",
            EstadoNuevo = "Pendiente",
            Observacion = "Cotización creada.",
            FechaRegistro = DateTime.Now,
            CedulaUsuario = ctx.Cedula,
            UsuarioRegistro = usuario.Identity?.Name
        });

        _dbContext.Cotizaciones.Add(cotizacion);
        await _dbContext.SaveChangesAsync();

        return cotizacion.IdCotizacion;
    }

    public async Task<CotizacionDetalleVm?> ObtenerDetalleAsync(int idCotizacion, ClaimsPrincipal usuario)
    {
        var ctx = ObtenerContextoClaims(usuario);

        var cotizacion = await _dbContext.Cotizaciones
            .Include(c => c.Detalles)
            .Include(c => c.Seguimientos)
            .AsNoTracking()
            .FirstOrDefaultAsync(c =>
                c.IdCotizacion == idCotizacion &&
                c.IdEmpresa == ctx.EmpresaId &&
                c.SedeId == ctx.SedeId &&
                c.InfopdvId == ctx.PdvId &&
                c.Activo);

        if (cotizacion == null)
            return null;

        var empresa = await _dbContext.Empresas
        .AsNoTracking()
        .FirstOrDefaultAsync(e => e.Id_empresa == ctx.EmpresaId);

        return new CotizacionDetalleVm
        {
            IdCotizacion = cotizacion.IdCotizacion,
            NumeroCotizacion = cotizacion.NumeroCotizacion,
            EstadoCotizacion = cotizacion.EstadoCotizacion,
            IdCliente = cotizacion.IdCliente,
            NombreCliente = cotizacion.NombreCliente ?? "",
            CedulaCliente = cotizacion.CedulaCliente,
            TelefonoCliente = cotizacion.TelefonoCliente,
            CorreoCliente = cotizacion.CorreoCliente,
            DireccionCliente = cotizacion.DireccionCliente,
            FechaCreacion = cotizacion.FechaCreacion,
            FechaVencimiento = cotizacion.FechaVencimiento,
            FechaAceptacion = cotizacion.FechaAceptacion,
            FechaRechazo = cotizacion.FechaRechazo,
            FechaConversionVenta = cotizacion.FechaConversionVenta,
            IdVentaGenerada = cotizacion.IdVentaGenerada,
            ObservacionGeneral = cotizacion.ObservacionGeneral,
            Subtotal = cotizacion.Subtotal,
            IvaTotal = cotizacion.IvaTotal,
            Descuento = cotizacion.Descuento,
            Total = cotizacion.Total,
            Empresa = empresa,

            Detalles = cotizacion.Detalles.Select(d => new CotizacionDetalleItemVm
            {
                IdCotizacionDetalle = d.IdCotizacionDetalle,
                CodigoProducto = d.CodigoProducto,
                NombreProducto = d.NombreProducto,
                Cantidad = d.Cantidad,
                ValorNeto = d.ValorNeto,
                ValorUnidad = d.ValorUnidad,
                ValorVenta = d.ValorVenta,
                IvaPorcentaje = d.IvaPorcentaje,
                IvaValor = d.IvaValor,
                SubTotal = d.SubTotal,
                TotalLinea = d.TotalLinea,
                IncluidoEnVenta = d.IncluidoEnVenta,
                Observacion = d.Observacion
            }).ToList(),

            Seguimientos = cotizacion.Seguimientos
                .OrderByDescending(s => s.FechaRegistro)
                .Select(s => new CotizacionSeguimientoItemVm
                {
                    FechaRegistro = s.FechaRegistro,
                    EstadoAnterior = s.EstadoAnterior,
                    EstadoNuevo = s.EstadoNuevo,
                    Observacion = s.Observacion,
                    UsuarioRegistro = s.UsuarioRegistro
                })
                .ToList(),

            NuevoSeguimiento = new CotizacionSeguimientoVm
            {
                IdCotizacion = cotizacion.IdCotizacion
            }
        };
    }

    public async Task<bool> RegistrarSeguimientoAsync(CotizacionSeguimientoVm vm, ClaimsPrincipal usuario)
    {
        var ctx = ObtenerContextoClaims(usuario);

        var cotizacion = await _dbContext.Cotizaciones
            .FirstOrDefaultAsync(c =>
                c.IdCotizacion == vm.IdCotizacion &&
                c.IdEmpresa == ctx.EmpresaId &&
                c.SedeId == ctx.SedeId &&
                c.InfopdvId == ctx.PdvId &&
                c.Activo);

        if (cotizacion == null)
            throw new Exception("La cotización no existe.");

        var estadoAnterior = cotizacion.EstadoCotizacion;
        var estadoNuevo = vm.EstadoNuevo;

        if (estadoAnterior == "ConvertidaVenta")
            throw new Exception("Una cotización convertida en venta no puede cambiar de estado.");

        if (estadoNuevo == "Rechazada")
            cotizacion.FechaRechazo = DateTime.Now;

        if (estadoNuevo == "Aceptada")
            cotizacion.FechaAceptacion = DateTime.Now;

        cotizacion.EstadoCotizacion = estadoNuevo;

        _dbContext.CotizacionSeguimientos.Add(new CotizacionSeguimiento
        {
            IdCotizacion = cotizacion.IdCotizacion,
            EstadoAnterior = estadoAnterior,
            EstadoNuevo = estadoNuevo,
            Observacion = vm.Observacion,
            FechaRegistro = DateTime.Now,
            CedulaUsuario = ctx.Cedula,
            UsuarioRegistro = usuario.Identity?.Name
        });

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<CotizacionConvertirVentaVm?> ObtenerParaConvertirVentaAsync(int idCotizacion, ClaimsPrincipal usuario)
    {
        var ctx = ObtenerContextoClaims(usuario);

        var cotizacion = await _dbContext.Cotizaciones
            .Include(c => c.Detalles)
            .AsNoTracking()
            .FirstOrDefaultAsync(c =>
                c.IdCotizacion == idCotizacion &&
                c.IdEmpresa == ctx.EmpresaId &&
                c.SedeId == ctx.SedeId &&
                c.InfopdvId == ctx.PdvId &&
                c.Activo);

        if (cotizacion == null)
            return null;

        if (cotizacion.EstadoCotizacion == "ConvertidaVenta")
            throw new Exception("Esta cotización ya fue convertida en venta.");

        if (cotizacion.EstadoCotizacion == "Rechazada" || cotizacion.EstadoCotizacion == "Anulada")
            throw new Exception("No se puede convertir una cotización rechazada o anulada.");

        var codigos = cotizacion.Detalles.Select(d => d.CodigoProducto).Distinct().ToList();

        var stocks = await _dbContext.InventarioSedes
            .Where(i => i.SedeId == ctx.SedeId && codigos.Contains(i.ProductoId))
            .ToDictionaryAsync(i => i.ProductoId, i => i.Cantidad);

        return new CotizacionConvertirVentaVm
        {
            IdCotizacion = cotizacion.IdCotizacion,
            NumeroCotizacion = cotizacion.NumeroCotizacion,
            IdCliente = cotizacion.IdCliente,
            CedulaCliente = cotizacion.CedulaCliente,
            NombreCliente = cotizacion.NombreCliente ?? "",
            ObservacionVenta = $"Venta generada desde cotización {cotizacion.NumeroCotizacion}.",

            Detalles = cotizacion.Detalles.Select(d => new CotizacionDetalleItemVm
            {
                IdCotizacionDetalle = d.IdCotizacionDetalle,
                CodigoProducto = d.CodigoProducto,
                NombreProducto = d.NombreProducto,
                Cantidad = d.Cantidad,
                ValorNeto = d.ValorNeto,
                ValorUnidad = d.ValorUnidad,
                ValorVenta = d.ValorVenta,
                IvaPorcentaje = d.IvaPorcentaje,
                IvaValor = d.IvaValor,
                SubTotal = d.SubTotal,
                TotalLinea = d.TotalLinea,
                IncluidoEnVenta = true,
                StockDisponible = stocks.ContainsKey(d.CodigoProducto) ? stocks[d.CodigoProducto] : 0
            }).ToList()
        };
    }

    public async Task<int> ConvertirCotizacionAVentaAsync(CotizacionConvertirVentaVm vm, ClaimsPrincipal usuario)
    {
        var ctx = ObtenerContextoClaims(usuario);

        var cotizacion = await _dbContext.Cotizaciones
            .Include(c => c.Detalles)
            .FirstOrDefaultAsync(c =>
                c.IdCotizacion == vm.IdCotizacion &&
                c.IdEmpresa == ctx.EmpresaId &&
                c.SedeId == ctx.SedeId &&
                c.InfopdvId == ctx.PdvId &&
                c.Activo);

        if (cotizacion == null)
            throw new Exception("La cotización no existe.");

        if (cotizacion.EstadoCotizacion == "ConvertidaVenta")
            throw new Exception("Esta cotización ya fue convertida en venta.");

        if (cotizacion.EstadoCotizacion == "Rechazada" || cotizacion.EstadoCotizacion == "Anulada")
            throw new Exception("No se puede convertir una cotización rechazada o anulada.");

        var seleccionados = vm.Detalles
            .Where(d => d.IncluidoEnVenta && d.Cantidad > 0)
            .ToList();

        if (!seleccionados.Any())
            throw new Exception("Debe seleccionar al menos un producto para convertir en venta.");

        using var tx = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            foreach (var item in seleccionados)
            {
                var inv = await _dbContext.InventarioSedes
                    .FirstOrDefaultAsync(i =>
                        i.SedeId == ctx.SedeId &&
                        i.ProductoId == item.CodigoProducto);

                if (inv == null)
                    throw new Exception($"No existe inventario para el producto {item.CodigoProducto} en la sede actual.");

                if (inv.Cantidad < item.Cantidad)
                    throw new Exception($"Stock insuficiente para el producto {item.NombreProducto}.");
            }

            var totalVenta = seleccionados.Sum(d => d.TotalLinea);

            var venta = new Ventas
            {
                IdCliente = cotizacion.IdCliente,
                Cedula = ctx.Cedula,
                CedulaCliente = cotizacion.CedulaCliente ?? 0,
                Conceptos = $"Venta generada desde cotización {cotizacion.NumeroCotizacion}",
                ObservacionVenta = string.IsNullOrWhiteSpace(vm.ObservacionVenta)
                    ? $"Venta generada desde cotización {cotizacion.NumeroCotizacion}."
                    : vm.ObservacionVenta.Trim(),
                FechaVenta = DateTime.Now,
                EstadoVenta = "Pendiente",
                Total = totalVenta,
                MetodoPago = "Pendiente",
                TipoVenta = "Normal",
                TipoOperacion = "Producto",
                OrigenModulo = "Cotizacion",
                IdOrigenModulo = cotizacion.IdCotizacion,
                CodigoReferenciaOrigen = cotizacion.NumeroCotizacion
            };

            _dbContext.Ventas.Add(venta);
            await _dbContext.SaveChangesAsync();

            venta.CodigoReferenciaOrigen = cotizacion.NumeroCotizacion;

            foreach (var item in seleccionados)
            {
                var detalleCotizacion = cotizacion.Detalles
                    .FirstOrDefault(d => d.IdCotizacionDetalle == item.IdCotizacionDetalle);

                if (detalleCotizacion == null)
                    throw new Exception("Uno de los detalles seleccionados no pertenece a la cotización.");

                var inv = await _dbContext.InventarioSedes
                    .FirstOrDefaultAsync(i =>
                        i.SedeId == ctx.SedeId &&
                        i.ProductoId == detalleCotizacion.CodigoProducto);

                if (inv == null)
                    throw new Exception($"No existe inventario para el producto {detalleCotizacion.CodigoProducto}.");

                inv.Cantidad -= detalleCotizacion.Cantidad;
                inv.ActualizadoEn = DateTime.Now;
                inv.Cedula = ctx.Cedula;

                var pedido = new Pedidos
                {
                    IdVenta = venta.IdVenta,
                    Codigo = detalleCotizacion.CodigoProducto,
                    Stock = detalleCotizacion.Cantidad,
                    VNeto = detalleCotizacion.ValorNeto,
                    VUnidad = detalleCotizacion.ValorUnidad * detalleCotizacion.Cantidad,
                    VVenta = detalleCotizacion.ValorVenta,
                    InfopdvId = ctx.PdvId,
                    FechaRegistro = DateTime.Now,
                    IvaPorcentaje = detalleCotizacion.IvaPorcentaje,
                    IvaValor = detalleCotizacion.IvaValor,
                    SubTotal = detalleCotizacion.TotalLinea
                };

                _dbContext.Pedidos.Add(pedido);

                detalleCotizacion.IncluidoEnVenta = true;
            }

            var idsSeleccionados = seleccionados
                .Select(x => x.IdCotizacionDetalle)
                .ToHashSet();

            foreach (var detalle in cotizacion.Detalles)
            {
                if (!idsSeleccionados.Contains(detalle.IdCotizacionDetalle))
                {
                    detalle.IncluidoEnVenta = false;
                    detalle.Observacion = "Producto no incluido al convertir la cotización en venta.";
                }
            }

            cotizacion.EstadoCotizacion = "ConvertidaVenta";
            cotizacion.FechaAceptacion ??= DateTime.Now;
            cotizacion.FechaConversionVenta = DateTime.Now;
            cotizacion.IdVentaGenerada = venta.IdVenta;

            _dbContext.CotizacionSeguimientos.Add(new CotizacionSeguimiento
            {
                IdCotizacion = cotizacion.IdCotizacion,
                EstadoAnterior = "Aceptada",
                EstadoNuevo = "ConvertidaVenta",
                Observacion = $"Cotización convertida en venta #{venta.IdVenta}.",
                FechaRegistro = DateTime.Now,
                CedulaUsuario = ctx.Cedula,
                UsuarioRegistro = usuario.Identity?.Name
            });

            await _dbContext.SaveChangesAsync();

            venta.Total = await _dbContext.Pedidos
                .Where(p => p.IdVenta == venta.IdVenta)
                .SumAsync(p => p.SubTotal);

            await _dbContext.SaveChangesAsync();

            await tx.CommitAsync();

            return venta.IdVenta;
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> AnularCotizacionAsync(int idCotizacion, string observacion, ClaimsPrincipal usuario)
    {
        var ctx = ObtenerContextoClaims(usuario);

        var cotizacion = await _dbContext.Cotizaciones
            .FirstOrDefaultAsync(c =>
                c.IdCotizacion == idCotizacion &&
                c.IdEmpresa == ctx.EmpresaId &&
                c.SedeId == ctx.SedeId &&
                c.InfopdvId == ctx.PdvId &&
                c.Activo);

        if (cotizacion == null)
            throw new Exception("La cotización no existe.");

        if (cotizacion.EstadoCotizacion == "ConvertidaVenta")
            throw new Exception("No se puede anular una cotización convertida en venta.");

        var estadoAnterior = cotizacion.EstadoCotizacion;

        cotizacion.EstadoCotizacion = "Anulada";
        cotizacion.Activo = false;

        _dbContext.CotizacionSeguimientos.Add(new CotizacionSeguimiento
        {
            IdCotizacion = cotizacion.IdCotizacion,
            EstadoAnterior = estadoAnterior,
            EstadoNuevo = "Anulada",
            Observacion = string.IsNullOrWhiteSpace(observacion)
                ? "Cotización anulada."
                : observacion,
            FechaRegistro = DateTime.Now,
            CedulaUsuario = ctx.Cedula,
            UsuarioRegistro = usuario.Identity?.Name
        });

        await _dbContext.SaveChangesAsync();

        return true;
    }

    private async Task<string> GenerarNumeroCotizacionAsync()
    {
        var fecha = DateTime.Now.ToString("yyyyMMdd");

        var consecutivoHoy = await _dbContext.Cotizaciones
            .CountAsync(c => c.NumeroCotizacion.StartsWith($"COT-{fecha}-"));

        return $"COT-{fecha}-{(consecutivoHoy + 1):D4}";
    }

    private (int Cedula, string EmpresaId, int SedeId, int PdvId) ObtenerContextoClaims(ClaimsPrincipal usuario)
    {
        var cedulaStr = usuario.FindFirst("Cedula")?.Value;
        var empresaId = usuario.FindFirst("EmpresaId")?.Value;
        var sedeIdStr = usuario.FindFirst("SedeId")?.Value;
        var pdvIdStr = usuario.FindFirst("PdvId")?.Value;

        if (!int.TryParse(cedulaStr, out var cedula))
            throw new Exception("No se pudo obtener la cédula del usuario autenticado.");

        if (string.IsNullOrWhiteSpace(empresaId))
            throw new Exception("No se pudo obtener la empresa del usuario autenticado.");

        if (!int.TryParse(sedeIdStr, out var sedeId) || sedeId <= 0)
            throw new Exception("No se pudo obtener la sede actual del usuario.");

        if (!int.TryParse(pdvIdStr, out var pdvId) || pdvId <= 0)
            throw new Exception("No se pudo obtener el PDV actual del usuario.");

        return (cedula, empresaId, sedeId, pdvId);
    }
}
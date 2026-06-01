using Microsoft.EntityFrameworkCore;
using Plataforma.Domain.Enums;
using Plataforma.Domain.Exceptions;
using Plataforma.Models;
using Plataforma.Models.Dto.Pedido;
using Plataforma.Servicios.Contrato;
using System.Security.Claims;

namespace Plataforma.Servicios.Implementacion
{
    public class PedidoService : IPedidoService
    {
        private readonly BaseAdmContext _dbContext;
        private readonly ICxcService _cxcService;
        public PedidoService(BaseAdmContext dbContext, ICxcService cxcService)
        {
            _dbContext = dbContext;
            _cxcService = cxcService;
        }
        public async Task<List<MetodoPagos>> TraerMetodosPagoDisponibles(int metodoPago)
        {
            return await _dbContext.MetodoPagos
                .Where(m => m.IdMetodo == metodoPago)
                .ToListAsync();
        }
        public async Task<bool> CrearVentaAsync(Ventas venta)
        {
            try
            {
                _dbContext.Ventas.Add(venta);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<Ventas?> ObtenerVentaConDetalleAsync(int idVenta)
        {
            return await _dbContext.Ventas
                .Include(v => v.Pedidos)
                .FirstOrDefaultAsync(v => v.IdVenta == idVenta);
        }

        public async Task<List<Ventas>> ObtenerTodasLasVentasAsync()
        {
            return await _dbContext.Ventas
                .OrderByDescending(v => v.FechaVenta)
                .ToListAsync();
        }
        public async Task<(decimal? valorVenta, decimal? valorNeto)?> BuscarProductoPorCodigoAsync(string codigo)
        {
            var producto = await _dbContext.Productos
                .Where(p => p.Cod_Producto == codigo)
                .Select(p => new
                {
                    p.ValorVentaProducto,
                    p.ValorNetoProducto
                })
                .FirstOrDefaultAsync();

            if (producto == null)
                return null;

            return (producto.ValorVentaProducto, producto.ValorNetoProducto);
        }
        public async Task<decimal> ObtenerCantidadProductoActual(string codigo)
        {
            try
            {
                var cantidad = await _dbContext.InventarioSedes.Where(x => x.ProductoId == codigo).FirstOrDefaultAsync();
                return cantidad.Cantidad;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<Dictionary<string, decimal>> ObtenerStockActualPorCodigosAsync(IEnumerable<string> codigos, int idSede)
        {
            return await _dbContext.InventarioSedes
                .Where(i => i.SedeId == idSede && codigos.Contains(i.ProductoId.ToString()))  // Convierte ProductoId a string
                .ToDictionaryAsync(i => i.ProductoId, i => i.Cantidad);
        }


        public async Task GuardarPedidosAsync(List<Pedidos> pedidos, int idVenta, ClaimsPrincipal usuario)
        {
            // 1) Obtener cédula
            var cedulaStr = usuario.FindFirst("Cedula")?.Value;
            if (!int.TryParse(cedulaStr, out int cedula))
                throw new PedidoException(PedidoErrorCode.UsuarioSinCedula,
                    "No se pudo obtener la cédula del usuario autenticado.");

            // 2) Consultar IdSede
            var idSede = await _dbContext.Sedeempleado
                .Where(se => se.Cedula == cedula)
                .Select(se => se.Id_sede)
                .FirstOrDefaultAsync();

            if (idSede == 0)
                throw new PedidoException(PedidoErrorCode.UsuarioSinSede,
                    "No se encontró una sede asociada al usuario.");

            // 3) Consultar InfoPdvId
            var infoPdvId = await _dbContext.Infopdv
                .Where(p => p.Id_Sede == idSede)
                .Select(p => p.InfopdvId)
                .FirstOrDefaultAsync();

            if (infoPdvId == 0)
                throw new PedidoException(PedidoErrorCode.SedeSinPdv,
                    "No se encontró un PDV válido para la sede.");

            // 4) Validar stock en batch (ProductoId string)
            var codigos = pedidos.Select(p => p.Codigo).Distinct().ToList();
            var stocks = await _dbContext.InventarioSedes
                .Where(i => i.SedeId == idSede && codigos.Contains(i.ProductoId))
                .ToDictionaryAsync(i => i.ProductoId, i => i.Cantidad); // Cantidad: ajusta tipo (int/decimal)

            foreach (var pedido in pedidos)
            {
                if (!stocks.TryGetValue(pedido.Codigo, out var stockActual))
                    throw new PedidoException(PedidoErrorCode.InventarioNoEncontrado,
                        $"No existe inventario para el producto {pedido.Codigo} en la sede.");

                if (pedido.Stock <= 0)
                    throw new PedidoException(PedidoErrorCode.StockInsuficiente,
                        $"Cantidad inválida para el producto {pedido.Codigo}.");

                if (stockActual < pedido.Stock)
                    throw new PedidoException(PedidoErrorCode.StockInsuficiente,$"Sin stock en productos.");
            }

            using var tx = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                decimal totalVenta = 0m;

                foreach (var pedido in pedidos)
                {
                    var inv = await _dbContext.InventarioSedes
                        .FirstOrDefaultAsync(i => i.SedeId == idSede && i.ProductoId == pedido.Codigo);

                    if (inv == null)
                        throw new PedidoException(PedidoErrorCode.InventarioNoEncontrado,
                            $"No existe inventario para el producto {pedido.Codigo} en la sede.");

                    inv.Cantidad -= pedido.Stock;

                    var valorUnitarioVenta = pedido.VVenta*pedido.Stock;
                    var valorUnitarioNeto = pedido.VNeto*pedido.Stock;
                    var ivaPorcentaje = pedido.IvaPorcentaje ?? 0m;

                    var ivaLinea = valorUnitarioVenta * (ivaPorcentaje / 100m);
                    var subTotalConIva = valorUnitarioVenta + ivaLinea;

                    pedido.IdVenta = idVenta;
                    pedido.InfopdvId = infoPdvId;
                    pedido.FechaRegistro = DateTime.Now;
                    pedido.VUnidad = valorUnitarioNeto;
                    pedido.IvaValor = ivaLinea;
                    pedido.SubTotal = subTotalConIva;

                    totalVenta += pedido.SubTotal;

                    _dbContext.Pedidos.Add(pedido);
                }

                // ✅ Actualizar Ventas.Total (esto es lo que te está faltando)
                var venta = await _dbContext.Ventas.FirstOrDefaultAsync(v => v.IdVenta == idVenta);
                if (venta == null)
                    throw new PedidoException(PedidoErrorCode.VentaNoExiste, $"La venta {idVenta} no existe.");

                venta.Total = totalVenta;

                await _dbContext.SaveChangesAsync();
                // Recalcular total real
                var totalReal = await _dbContext.Pedidos
                    .Where(p => p.IdVenta == idVenta)
                    .SumAsync(p => p.SubTotal);

                venta.Total = totalReal;

                await _dbContext.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }


        public async Task<bool> AgregarPedidoAVentaAsync(Pedidos pedido)
        {
            try
            {
                _dbContext.Pedidos.Add(pedido);

                // Actualizar el total en la venta
                var venta = await _dbContext.Ventas.FirstOrDefaultAsync(v => v.IdVenta == pedido.IdVenta);
                if (venta != null)
                {
                    venta.Total += (int)pedido.SubTotal;
                }

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public void ActualizarEstadoFacturas()
        {
            var facturasVencidas = _dbContext.Factura
                .Where(f => f.FechaEmision.AddDays(7) <= DateTime.Now && f.EstadoFactura == "Emitida")
                .ToList();

            foreach (var factura in facturasVencidas)
            {
                factura.EstadoFactura = "Cerrada";
            }

            _dbContext.SaveChanges();
        }
        public List<Factura> ObtenerFacturasFechaDescendente()
        {
            return _dbContext.Factura
                .Include(f => f.Venta)
                .OrderByDescending(f => f.FechaEmision)
                .ToList();
        }
        public async Task<DetalleVentaViewModel> ObtenerVentaConPedidos(int idVenta)
        {
            var venta = await _dbContext.Ventas
                .Include(v => v.Pedidos)
                .FirstOrDefaultAsync(v => v.IdVenta == idVenta);

            if (venta == null) return null;

            int? idFactura = null;
            string numeroFactura = null;

            if (venta.EstadoVenta == "Facturada")
            {
                var factura = await _dbContext.Factura
                    .FirstOrDefaultAsync(f => f.IdVenta == idVenta);

                if (factura != null)
                {
                    idFactura = factura.IdFactura;
                    numeroFactura = factura.NumeroFactura;
                }
            }

            // ✅ Cliente por consulta aparte
            var cliente = await _dbContext.Clientes
                .FirstOrDefaultAsync(c => c.IdCliente == venta.IdCliente);

            if (cliente == null && venta.CedulaCliente > 0)
            {
                cliente = await _dbContext.Clientes
                    .FirstOrDefaultAsync(c => c.CedulaCliente == venta.CedulaCliente);
            }

            // ✅ CxC por consulta aparte (si existe)
            var cxc = await _dbContext.CxcVentas
                .Where(c => c.IdVenta == idVenta)
                .OrderByDescending(c => c.IdCxc)
                .FirstOrDefaultAsync();

            return new DetalleVentaViewModel
            {
                Venta = venta,
                IdFactura = idFactura,
                NumeroFactura = numeroFactura,

                Cliente = cliente,

                SaldoPendienteCredito = cxc?.SaldoPendiente ?? 0,
                FechaVencimientoCredito = cxc?.FechaVencimiento,
                EstadoCxc = cxc?.EstadoCxc
            };
        }
        public async Task GuardarVentaActualizada(Ventas venta, decimal total)
        {
            if (venta == null)
                throw new ArgumentNullException(nameof(venta), "La venta no puede ser nula.");

            if (venta.Pedidos == null || !venta.Pedidos.Any())
                throw new Exception("La venta no tiene productos asociados.");


            // 2. Actualizar el campo Total en la venta
            venta.Total = total;

            // 3. Guardar cambios en la base de datos
            _dbContext.Ventas.Update(venta);
            await _dbContext.SaveChangesAsync();
        }
        public async Task GuardarFacturaAsync(Factura factura)
        {
            _dbContext.Factura.Add(factura);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("⚠️ Error al guardar: " + ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine("🔍 Inner: " + ex.InnerException.Message);
                throw;
            }
        }
        public async Task<List<Factura>> ObtenerFacturasConVentaCliente()
        {
            return await _dbContext.Factura
                .Include(f => f.Venta)
                .ThenInclude(v => v.Pedidos)
                .OrderByDescending(f => f.FechaEmision)
                .ToListAsync();
        }
        public async Task<DetalleFacturaViewModel> ObtenerFacturaConDetalle(int idFactura, string empresaId)
        {
            var factura = await _dbContext.Factura
                .Include(f => f.Venta)
                    .ThenInclude(v => v.Pedidos)
                        .ThenInclude(p => p.Producto)
                .FirstOrDefaultAsync(f => f.IdFactura == idFactura);

            if (factura == null) return null;

            var cliente = await _dbContext.Clientes
                .FirstOrDefaultAsync(c => c.IdCliente == factura.Venta.IdCliente);

            if (cliente == null && factura.Venta.CedulaCliente > 0)
            {
                cliente = await _dbContext.Clientes
                    .FirstOrDefaultAsync(c => c.CedulaCliente == factura.Venta.CedulaCliente);
            }

            var empresa = await _dbContext.Empresas
                .FirstOrDefaultAsync(e => e.Id_empresa == empresaId);

            return new DetalleFacturaViewModel
            {
                Factura = factura,
                Cliente = cliente,
                Empresa = empresa
            };
        }
        public async Task<bool> AnularFacturaAsync(int idFactura)
        {
            var factura = await _dbContext.Factura.FindAsync(idFactura);
            if (factura == null || factura.EstadoFactura == "Anulada")
                return false;

            factura.EstadoFactura = "Anulada";
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> EliminarFacturaAsync(int idFactura)
        {
            var factura = await _dbContext.Factura.FindAsync(idFactura);
            if (factura == null)
                return false;

            _dbContext.Factura.Remove(factura);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<Ventas> ObtenerVentaPorIdAsync(int id)
        {
            return await _dbContext.Ventas.FindAsync(id);
        }

        public async Task ActualizarVentaAsync(Ventas venta)
        {
            _dbContext.Ventas.Update(venta);
            await _dbContext.SaveChangesAsync();
        }
        public async Task ActualizarEstadoVentaAsync(int idVenta, string nuevoEstado)
        {
            var venta = await _dbContext.Ventas.FirstOrDefaultAsync(v => v.IdVenta == idVenta);
            if (venta != null)
            {
                venta.EstadoVenta = nuevoEstado;
                _dbContext.Ventas.Update(venta);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception($"No se encontró la venta con Id {idVenta} para actualizar su estado.");
            }
        }
        public async Task<Factura> ObtenerFacturaConAdicionesAsync(int IdFactura)
        {
            return await _dbContext.Factura
                .Include(f => f.Venta)
                .Include(f => f.Adiciones)
                .FirstOrDefaultAsync(f => f.IdFactura == IdFactura);
        }

        public async Task<bool> AgregarAdicionFacturaAsync(int IdFactura, decimal valor, string descripcion, int cedulaEmpleado, string EstadoAdicion)
        {
            var factura = await _dbContext.Factura.FindAsync(IdFactura);
            if (factura == null)
                return false;

            var adicion = new AdicionFactura
            {
                IdFactura = IdFactura,
                Valor = valor,
                Descripcion = descripcion,
                Fecha = DateTime.Now,
                Cedula = cedulaEmpleado
            };            

            _dbContext.AdicionFacturas.Add(adicion);
            _dbContext.Factura.Update(factura);
            await _dbContext.SaveChangesAsync();

            return true;
        }
        public List<AdicionFactura> ObtenerConceptosCompletos()
        {
            return _dbContext.AdicionFacturas.ToList();
        }
        public List<InventarioSede> ValidarProductoPorCodigo(string Codigo)
        {
            return _dbContext.InventarioSedes.Where(cd => cd.ProductoId == Codigo).ToList();
        }
        public async Task<List<InventarioSede>> BuscarProductosPorCodigo(string codigo)
        {
            return await _dbContext.Set<InventarioSede>()
                                 .Include(p => p.Producto)
                                 .Where(p => p.ProductoId.Contains(codigo) || p.Producto.NombreProducto.Contains(codigo))
                                 .OrderBy(p => p.Producto.NombreProducto)
                                 .Take(20)
                                 .ToListAsync();
        }
        public async Task<bool> EmitirFacturaAsync(int idVenta, string metodoPago)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var venta = await _dbContext.Ventas
                    .Include(v => v.Pedidos)
                    .FirstOrDefaultAsync(v => v.IdVenta == idVenta);

                if (venta == null)
                    return false;

                if (venta.EstadoVenta != "Confirmada")
                    throw new Exception("La venta no está aprobada para facturar.");

                // 1️⃣ Calcular valores
                decimal subTotal = venta.Pedidos.Sum(p => p.SubTotal);
                decimal iva = subTotal * 0.19m;
                decimal total = subTotal + iva;

                // 2️⃣ Crear factura
                var factura = new Factura
                {
                    NumeroFactura = await GenerarNumeroFacturaAsync("FAC"),
                    IdVenta = venta.IdVenta,
                    FechaEmision = DateTime.Now,
                    SubTotal = subTotal,
                    IVA = iva,
                    Total = total,
                    EstadoFactura = "Emitida"
                };

                _dbContext.Factura.Add(factura);

                // 3️⃣ Actualizar venta
                venta.EstadoVenta = "Facturada";
                venta.MetodoPago = metodoPago;
                venta.FechaEmisionFactura = DateTime.Now;

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        private async Task<string> GenerarNumeroFacturaAsync(string prefijo)
        {
            var fecha = DateTime.Now.ToString("yyyyMMdd");

            var consecutivoHoy = await _dbContext.Factura
                .CountAsync(f => f.NumeroFactura != null &&
                                 f.NumeroFactura.StartsWith($"{prefijo}-{fecha}-"));

            return $"{prefijo}-{fecha}-{(consecutivoHoy + 1):D4}";
        }
        public async Task CambiarEstadoVentaAsync(int idVenta, string nuevoEstado)
        {
            var venta = await _dbContext.Ventas.FirstOrDefaultAsync(v => v.IdVenta == idVenta);
            if (venta == null)
                throw new PedidoException(PedidoErrorCode.VentaNoExiste,
                    "La venta no existe.");

            // Reglas claras
            if (venta.EstadoVenta == "Facturada")
                throw new PedidoException(PedidoErrorCode.VentaNoModificable,
                    "Una venta facturada no puede modificarse.");

            if (venta.EstadoVenta == "Pendiente" && nuevoEstado != "Confirmada" && nuevoEstado != "Anulada")
                throw new PedidoException(PedidoErrorCode.EstadoInvalido,
                    "Estado no permitido desde Pendiente.");

            if (venta.EstadoVenta == "Confirmada" && nuevoEstado != "Anulada")
                throw new PedidoException(PedidoErrorCode.EstadoInvalido,
                    "Una venta confirmada solo puede anularse o facturarse.");

            venta.EstadoVenta = nuevoEstado;
            await _dbContext.SaveChangesAsync();
        }
        private async Task<int> ObtenerIdSedeDelUsuarioAsync(ClaimsPrincipal usuario)
        {
            var cedulaStr = usuario.FindFirst("Cedula")?.Value;
            if (!int.TryParse(cedulaStr, out int cedula))
                throw new PedidoException(PedidoErrorCode.CedulaNoValida,
                    "No se pudo obtener la cédula del usuario autenticado.");

            var idSede = await _dbContext.Sedeempleado
                .Where(se => se.Cedula == cedula)
                .Select(se => se.Id_sede)
                .FirstOrDefaultAsync();

            if (idSede == 0)
                throw new PedidoException(PedidoErrorCode.SedeNoEncontrada,
                    "No se encontró una sede asociada al usuario.");

            return idSede;
        }
        public async Task<List<ProductoVentaDto>> BuscarProductosPorNombreVentaAsync(string texto, ClaimsPrincipal usuario)
        {
            texto = (texto ?? "").Trim();
            if (texto.Length < 2)
                return new List<ProductoVentaDto>();

            // 1) Obtener idSede desde el usuario
            var cedulaStr = usuario.FindFirst("Cedula")?.Value;
            if (!int.TryParse(cedulaStr, out int cedula))
                throw new Exception("No se pudo obtener la cédula del usuario autenticado.");

            var idSede = await _dbContext.Sedeempleado
                .Where(se => se.Cedula == cedula)
                .Select(se => se.Id_sede)
                .FirstOrDefaultAsync();

            if (idSede == 0)
                throw new Exception("No se encontró una sede asociada al usuario.");

            // 2) Buscar productos (maestro) por texto
            var productosBase = await _dbContext.Productos
                .Where(p => p.Cod_Producto!.Contains(texto) || p.NombreProducto!.Contains(texto))
                .Select(p => new
                {
                    Codigo = p.Cod_Producto!,
                    Nombre = p.NombreProducto!
                })
                .Take(30)
                .ToListAsync();

            if (productosBase.Count == 0)
                return new List<ProductoVentaDto>();

            var codigos = productosBase.Select(x => x.Codigo).Distinct().ToList();

            // 3) Consultar inventario de ESA sede (valores reales)
            var inventarios = await _dbContext.InventarioSedes
                .Where(i => i.SedeId == idSede && codigos.Contains(i.ProductoId))
                .Select(i => new
                {
                    Codigo = i.ProductoId,
                    Stock = (decimal?)i.Cantidad,
                    i.VUnidad,
                    ValorVenta = i.PrecioUnitario // ✅ usar el precio real por sede
                })
                .ToListAsync();

            var invMap = inventarios.ToDictionary(x => x.Codigo, x => x);

            // 4) Armar DTO final combinando maestro + inventario sede
            var result = new List<ProductoVentaDto>();

            foreach (var p in productosBase)
            {
                invMap.TryGetValue(p.Codigo, out var inv);

                result.Add(new ProductoVentaDto
                {
                    codigo = p.Codigo,
                    nombre = p.Nombre,
                    stockDisponible = inv?.Stock,        // null si no hay inventario en esa sede
                    valorUnidad = inv?.VUnidad,          // real por sede
                    valorVenta = inv?.ValorVenta         // real por sede (PrecioUnitario)
                });
            }

            return result;
        }
        public async Task<bool> FacturarConPagosAsync(
        int idVenta,
        int idCliente,
        string metodoPagoFinal,
        decimal montoEfectivo,
        decimal montoTransferencia,
        decimal montoCredito,
        DateTime? fechaVencimientoCredito)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var venta = await _dbContext.Ventas
                    .Include(v => v.Pedidos)
                    .FirstOrDefaultAsync(v => v.IdVenta == idVenta);

                if (venta == null) return false;

                if (venta.IdCliente != idCliente)
                    throw new Exception("El cliente enviado no coincide con la venta.");

                if (venta.EstadoVenta == "Anulada")
                    throw new Exception("La venta está anulada.");

                if (venta.EstadoVenta == "Facturada")
                    throw new Exception("La venta ya está facturada.");

                if (venta.EstadoVenta != "Confirmada")
                    throw new Exception("La venta debe estar en estado Confirmada para registrar pagos y facturar.");

                if (montoCredito > 0 && !fechaVencimientoCredito.HasValue)
                    throw new Exception("Si hay crédito, debes ingresar la fecha de vencimiento.");

                decimal baseFactura = venta.Pedidos.Sum(p => p.VVenta * p.Stock);
                decimal iva = venta.Pedidos.Sum(p => p.IvaValor ?? 0m);
                decimal totalFactura = venta.Pedidos.Sum(p => p.SubTotal);

                // OJO: deja + iva solo si en la vista los montos ingresados NO incluyen IVA.
                decimal totalPagos = montoEfectivo + montoTransferencia + montoCredito;

                if (Math.Abs(totalPagos - totalFactura) > 0.01m)
                    throw new Exception("La suma de los pagos no coincide con el total de la factura.");

                var numeroFactura = await GenerarNumeroFacturaAsync("FAC");

                var factura = new Factura
                {
                    NumeroFactura = numeroFactura,
                    IdVenta = venta.IdVenta,
                    FechaEmision = DateTime.Now,
                    SubTotal = baseFactura,
                    IVA = iva,
                    Total = totalFactura,
                    EstadoFactura = "Emitida"
                };

                _dbContext.Factura.Add(factura);

                venta.EstadoVenta = "Facturada";
                venta.MetodoPago = metodoPagoFinal;
                venta.NumeroFactura = numeroFactura;
                venta.FechaEmisionFactura = factura.FechaEmision;
                venta.Total = totalFactura;

                await _dbContext.SaveChangesAsync();

                if (montoCredito > 0)
                {
                    var resultadoCxc = await _cxcService.CrearDesdeVentaAsync(
                        venta.IdVenta,
                        venta.IdCliente,
                        montoCredito,
                        fechaVencimientoCredito,
                        $"Crédito generado al facturar. Método: {metodoPagoFinal}"
                    );

                    if (!resultadoCxc.Ok)
                        throw new Exception(resultadoCxc.Mensaje);
                }

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<List<Ventas>> ObtenerVentasFiltradasAsync(ContextoAccesoDto ctx)
        {
            var query =
                from v in _dbContext.Ventas
                join p in _dbContext.Pedidos on v.IdVenta equals p.IdVenta
                join pdv in _dbContext.Infopdv on p.InfopdvId equals pdv.InfopdvId
                join s in _dbContext.Sede on pdv.Id_Sede equals s.Id_sede
                select new { Venta = v, Pedido = p, Pdv = pdv, Sede = s };

            if (!ctx.EsAdministradorLider)
            {
                query = query.Where(x =>
                    x.Venta.Cedula == ctx.Cedula &&
                    x.Pedido.InfopdvId == ctx.PdvId &&
                    x.Pdv.Id_Sede == ctx.SedeId &&
                    x.Sede.Id_empresa == ctx.EmpresaId);
            }

            return await query
                .Select(x => x.Venta)
                .Distinct()
                .OrderByDescending(v => v.FechaVenta)
                .ToListAsync();
        }
        public async Task<List<Factura>> ObtenerFacturasFiltradasAsync(ContextoAccesoDto ctx)
        {
            var query =
                from f in _dbContext.Factura
                join v in _dbContext.Ventas on f.IdVenta equals v.IdVenta
                join p in _dbContext.Pedidos on v.IdVenta equals p.IdVenta
                join pdv in _dbContext.Infopdv on p.InfopdvId equals pdv.InfopdvId
                join s in _dbContext.Sede on pdv.Id_Sede equals s.Id_sede
                select new { Factura = f, Venta = v, Pedido = p, Pdv = pdv, Sede = s };

            if (!ctx.EsAdministradorLider)
            {
                query = query.Where(x =>
                    x.Venta.Cedula == ctx.Cedula &&
                    x.Pedido.InfopdvId == ctx.PdvId &&
                    x.Pdv.Id_Sede == ctx.SedeId &&
                    x.Sede.Id_empresa == ctx.EmpresaId);
            }

            var idsFacturas = await query
                .Select(x => x.Factura.IdFactura)
                .Distinct()
                .ToListAsync();

            return await _dbContext.Factura
                .Where(f => idsFacturas.Contains(f.IdFactura))
                .Include(f => f.Venta)
                .ThenInclude(v => v.Pedidos)
                .OrderByDescending(f => f.FechaEmision)
                .ToListAsync();
        }

    }
}

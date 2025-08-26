using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using System.Security.Claims;

namespace Plataforma.Servicios.Implementacion
{
    public class OrdenServicioService : IOrdenServicioService
    {
        private readonly BaseAdmContext _dbContext;
        public OrdenServicioService(BaseAdmContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AgregarDispositivoAsync(Dispositivos dispositivo)
        {
            _dbContext.Dispositivos.Add(dispositivo);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<List<OrdenServicios>> ObtenerTodasAsync()
        {
            return await _dbContext.OrdenServicios
                .Include(o => o.Dispositivo)
                .ThenInclude(d => d.Cliente)
                .ToListAsync();
        }

        public async Task<OrdenServicios?> ObtenerPorIdAsync(int id)
        {
            return await _dbContext.OrdenServicios
                .Include(o => o.Dispositivo)
                .ThenInclude(d => d.Cliente)
                .FirstOrDefaultAsync(o => o.IdOrden == id);
        }

        public async Task<OrdenServicios> CrearAsync(OrdenServicios orden, string cedulaClaim)
        {
            orden.Cedula = int.Parse(cedulaClaim);
            orden.FechaIngreso = DateTime.Now;

            _dbContext.OrdenServicios.Add(orden);
            await _dbContext.SaveChangesAsync();

            return orden;
        }

        public async Task ActualizarAsync(OrdenServicios orden, int cedulaEmpleado)
        {
                var ordenExistente = await _dbContext.OrdenServicios
            .Include(o => o.Dispositivo)
            .ThenInclude(d => d.Cliente)
            .FirstOrDefaultAsync(o => o.IdOrden == orden.IdOrden);

                // Verifica si cambia a "Entregado"
                bool seEntrego = orden.Estado == "Entregado" && ordenExistente.Estado != "Entregado";

                ordenExistente.ProblemaReportado = orden.ProblemaReportado;
                ordenExistente.Estado = orden.Estado;
                ordenExistente.FechaIngreso = DateTime.Now;

                _dbContext.OrdenServicios.Update(ordenExistente);
                if (seEntrego)
                {
                    var venta = new Ventas
                    {
                        EstadoVenta = "Pendiente",
                        FechaVenta = DateTime.Now,
                        IdCliente = ordenExistente.Dispositivo.IdCliente,
                        Total = 0, // lo puedes ajustar según reglas
                        MetodoPago = $"Venta generada desde la orden #{orden.IdOrden}",
                        Cedula = cedulaEmpleado
                    };

                    _dbContext.Ventas.Add(venta);
                }

                await _dbContext.SaveChangesAsync();
        }
        public async Task<List<HistOrdSer>> ObtenerOrdenPorIdAsync(int idOrden)
        {
                return await _dbContext.HistOrdServ
                    .Where(o => o.IdOrden == idOrden)
                    .ToListAsync();
        }
        public async Task<(OrdenServicios Orden, bool MostrarAgregarProductos)> ObtenerOrdenYPermisosAsync(int idOrden)
        {
            var orden = await _dbContext.OrdenServicios
                .FirstOrDefaultAsync(o => o.IdOrden == idOrden);

            if (orden == null)
                throw new Exception("No se encontró la orden.");

            bool mostrarAgregarProductos =
                orden.Estado?.Equals("Ejecucion", StringComparison.OrdinalIgnoreCase) == true;

            return (orden, mostrarAgregarProductos);
        }
        public async Task CrearHistOrdenAsync(HistOrdSer baseHistorial, string[]? Cod_Producto, int cedula)
        {
            baseHistorial.FechaRegistro = DateTime.Now;
            baseHistorial.Cedula = cedula;

            // Normaliza lista de códigos
            var codigos = (Cod_Producto ?? Array.Empty<string>())
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim())
                .ToList();

            // Transacción para que historial y stock queden coherentes
            await using var tx = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if (codigos.Count > 0)
                {
                    // 1) Crear registros de historial (uno por código)
                    var registros = codigos.Select(c => new HistOrdSer
                    {
                        IdOrden = baseHistorial.IdOrden,
                        ReparacionDet = baseHistorial.ReparacionDet,
                        FechaRegistro = baseHistorial.FechaRegistro,
                        Cedula = baseHistorial.Cedula,
                        Cod_Producto = c
                    }).ToList();

                    _dbContext.HistOrdServ.AddRange(registros);

                    // 2) Descontar stock por cada ocurrencia del código
                    //    (si el mismo código viene 3 veces, descuenta 3)
                    var porCodigo = codigos
                        .GroupBy(x => x)
                        .ToDictionary(g => g.Key, g => g.Count());

                    // Trae solo los productos involucrados
                    var productos = await _dbContext.Productos
                        .Where(p => porCodigo.Keys.Contains(p.Cod_Producto))
                        .ToListAsync();

                    // (Opcional) códigos no encontrados
                    // var noEncontrados = porCodigo.Keys.Except(productos.Select(p => p.Cod_Producto)).ToList();

                    foreach (var prod in productos)
                    {
                        var desc = porCodigo[prod.Cod_Producto];

                        // Evita negativos; si prefieres lanzar error, cámbialo.
                        var nuevo = prod.CantidadProducto - desc;
                        prod.CantidadProducto = nuevo < 0 ? 0 : nuevo;
                    }

                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    // Sin productos: un historial con Cod_Producto = null
                    baseHistorial.Cod_Producto = null;
                    _dbContext.HistOrdServ.Add(baseHistorial);
                    await _dbContext.SaveChangesAsync();
                }

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
        public async Task ActualizarOrdenAsync(OrdenServicios orden, ClaimsPrincipal usuario)
        {
            var ordenExistente = await _dbContext.OrdenServicios
                .Include(o => o.Dispositivo)
                .FirstOrDefaultAsync(o => o.IdOrden == orden.IdOrden);

            if (ordenExistente == null)
                throw new Exception("La orden no existe.");

            // Verifica si pasa a Finalizada
            bool seFinaliza = orden.Estado == "Finalizada" && ordenExistente.Estado != "Finalizada";

            // Actualizar solo campos permitidos
            ordenExistente.Estado = orden.Estado;
            ordenExistente.Observaciones = orden.Observaciones;
            ordenExistente.IdDispositivo = orden.IdDispositivo;
            ordenExistente.ProblemaReportado = orden.ProblemaReportado;

            _dbContext.OrdenServicios.Update(ordenExistente);

            if (seFinaliza)
            {
                // Obtener cédula desde el claim
                var cedulaStr = usuario.FindFirst("Cedula")?.Value;
                if (!int.TryParse(cedulaStr, out int cedula))
                    throw new Exception("No se pudo obtener la cédula del usuario autenticado.");

                // Traer todos los items de HistOrdServ para esta orden
                var itemsHist = await _dbContext.HistOrdServ
                    .Where(h => h.IdOrden == orden.IdOrden && h.Cod_Producto != null)
                    .ToListAsync();

                decimal subtotal = 0;
                var pedidosNuevos = new List<Pedidos>();

                // Consultar IdSede e InfoPdvId una sola vez aquí, porque los necesitas si hay pedidos
                var idSede = await _dbContext.Sedeempleado
                    .Where(se => se.Cedula == cedula)
                    .Select(se => se.Id_sede)
                    .FirstOrDefaultAsync();

                if (idSede == 0)
                    throw new Exception("No se encontró una sede asociada al usuario.");

                var infoPdvId = await _dbContext.Infopdv
                    .Where(p => p.Id_Sede == idSede)
                    .Select(p => p.InfopdvId)
                    .FirstOrDefaultAsync();

                if (infoPdvId == 0)
                    throw new Exception("No se encontró un PDV válido para la sede.");

                foreach (var item in itemsHist)
                {
                    var prod = await _dbContext.Productos
                        .Where(p => p.Cod_Producto == item.Cod_Producto)
                        .Select(p => new
                        {
                            p.Cod_Producto,
                            p.ValorVentaProducto,
                            p.ValorNetoProducto
                        })
                        .FirstOrDefaultAsync();

                    if (prod != null)
                    {
                        subtotal += (decimal)(float)prod.ValorVentaProducto;

                        pedidosNuevos.Add(new Pedidos
                        {
                            Codigo = prod.Cod_Producto,
                            Stock = 1,
                            VNeto = (int)prod.ValorNetoProducto,
                            VVenta = (int)prod.ValorVentaProducto,
                            InfopdvId = infoPdvId,
                            FechaRegistro = DateTime.Now,
                            SubTotal = (decimal)prod.ValorVentaProducto
                        });
                    }
                }

                // Crear venta (aunque no haya productos, subtotal es 0)
                var venta = new Ventas
                {
                    EstadoVenta = "Pendiente",
                    FechaVenta = DateTime.Now,
                    IdCliente = ordenExistente.Dispositivo.IdCliente,
                    Total = subtotal,
                    MetodoPago = "Efectivo",
                    Cedula = cedula,
                    CedulaCliente = (int)ordenExistente.Dispositivo.CedulaCliente,
                    Conceptos = $"Venta generada desde la orden #{orden.IdOrden}"
                };

                _dbContext.Ventas.Add(venta);
                await _dbContext.SaveChangesAsync(); // Genera IdVenta

                // Asignar IdVenta y guardar pedidos solo si hay productos
                if (pedidosNuevos.Any())
                {
                    foreach (var pedido in pedidosNuevos)
                    {
                        pedido.IdVenta = venta.IdVenta;
                        _dbContext.Pedidos.Add(pedido);
                    }
                }
            }

            await _dbContext.SaveChangesAsync();
        }
        public async Task<List<Empleados>> ObtenerEmpleadosAsync()
        {
            return await _dbContext.Empleado.ToListAsync();
        }
        public async Task<OrdenServicios?> ActualizarOrdenTecnico(OrdenServicios orden)
        {
            var ordenExistente = await _dbContext.OrdenServicios
                .FirstOrDefaultAsync(o => o.IdOrden == orden.IdOrden);

            if (ordenExistente == null)
                return null;

            // 🔹 Actualizas solo lo que venga de la vista
            ordenExistente.Cedula = orden.Cedula;

            // No cambias los demás campos (quedan igualitos en BD)
            await _dbContext.SaveChangesAsync();

            return ordenExistente;
        }
        public async Task<OrdenServicioRowDTO> GetOrdenRowAsync(int idOrden)
        {
            return await _dbContext.OrdenServicios
                .Where(o => o.IdOrden == idOrden)
                .Select(o => new OrdenServicioRowDTO
                {
                    IdOrden = o.IdOrden,
                    FechaIngreso = o.FechaIngreso,
                    Cliente = o.Dispositivo.Cliente.NombreCliente,
                    Telefono = o.Dispositivo.Cliente.TelefonoCliente,
                    Password = o.Dispositivo.Clave,
                    Marca = o.Dispositivo.Marca,
                    Modelo = o.Dispositivo.Modelo,
                    Descripcion = o.ProblemaReportado,
                    Observacion = o.Observaciones,
                    Estado = o.Estado,
                    Cedula = o.Cedula
                })
                .FirstAsync();
        }
    }
}

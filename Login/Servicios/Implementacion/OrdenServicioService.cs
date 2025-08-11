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

        public async Task CrearAsync(OrdenServicios orden)
        {
            orden.FechaIngreso = DateTime.Now;
            _dbContext.OrdenServicios.Add(orden);
            await _dbContext.SaveChangesAsync();
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
            // Fecha y cedula para todos
            baseHistorial.FechaRegistro = DateTime.Now;
            baseHistorial.Cedula = cedula;

            if (Cod_Producto != null && Cod_Producto.Any(c => !string.IsNullOrWhiteSpace(c)))
            {
                // Si hay productos, guardamos un registro por cada uno
                var registros = Cod_Producto
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Select(c => new HistOrdSer
                    {
                        IdOrden = baseHistorial.IdOrden,
                        ReparacionDet = baseHistorial.ReparacionDet,
                        FechaRegistro = baseHistorial.FechaRegistro,
                        Cedula = baseHistorial.Cedula,
                        Cod_Producto = c
                    })
                    .ToList();

                _dbContext.HistOrdServ.AddRange(registros);
            }
            else
            {
                // Si no hay productos, guardamos solo uno con Cod_Producto null
                baseHistorial.Cod_Producto = null;
                _dbContext.HistOrdServ.Add(baseHistorial);
            }

            await _dbContext.SaveChangesAsync();
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
            ordenExistente.FechaIngreso = DateTime.Now;

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
                    MetodoPago = $"Venta generada desde la orden #{orden.IdOrden}",
                    Cedula = cedula,
                    CedulaCliente = ordenExistente.Dispositivo.CedulaCliente
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



    }
}

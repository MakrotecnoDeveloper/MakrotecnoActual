using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Servicios.Implementacion
{
    public class OrdenServicioService : IOrdenServicioService
    {
        private readonly BaseAdmContext _dbContext;
        public OrdenServicioService(BaseAdmContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AgregarDispositivoAsync(Dispositivo dispositivo)
        {
            _dbContext.Dispositivos.Add(dispositivo);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<List<OrdenServicio>> ObtenerTodasAsync()
        {
            return await _dbContext.OrdenServicios
                .Include(o => o.Dispositivo)
                .ThenInclude(d => d.Cliente)
                .ToListAsync();
        }

        public async Task<OrdenServicio?> ObtenerPorIdAsync(int id)
        {
            return await _dbContext.OrdenServicios
                .Include(o => o.Dispositivo)
                .ThenInclude(d => d.Cliente)
                .FirstOrDefaultAsync(o => o.IdOrden == id);
        }

        public async Task CrearAsync(OrdenServicio orden)
        {
            orden.FechaIngreso = DateTime.Now;
            _dbContext.OrdenServicios.Add(orden);
            await _dbContext.SaveChangesAsync();
        }

        public async Task ActualizarAsync(OrdenServicio orden, int cedulaEmpleado)
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
                        CedulaCliente = ordenExistente.Dispositivo.CedulaCliente,
                        Total = 0, // lo puedes ajustar según reglas
                        MetodoPago = $"Venta generada desde la orden #{orden.IdOrden}",
                        Cedula = cedulaEmpleado
                    };

                    _dbContext.Ventas.Add(venta);
                }

                await _dbContext.SaveChangesAsync();
        }
    }
}

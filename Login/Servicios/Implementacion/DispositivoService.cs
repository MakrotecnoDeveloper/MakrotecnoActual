using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Servicios.Implementacion
{
    public class DispositivoService : IDispositivoService
    {
        private readonly BaseAdmContext _dbContext;
        public DispositivoService(BaseAdmContext dbContext)
        {
            _dbContext = dbContext;
        }

            public async Task<List<TipoDispositivos>> ObtenerTipoDispositivos()
            {
                return await  _dbContext.TipoDispositivos.ToListAsync();
            }

            public async Task CrearDispositivoAsync(Dispositivos dispositivo, string cedulaClaim)
            {
                var cliente = await _dbContext.Cliente
                .Where(c => c.IdCliente == dispositivo.IdCliente)
                .Select(c => new { c.IdCliente, c.CedulaCliente })
                .FirstOrDefaultAsync();

                if (cliente == null)
                    throw new Exception("El cliente no existe.");

                // Asignar la cédula al dispositivo
                dispositivo.CedulaCliente = cliente.CedulaCliente;

                _dbContext.Dispositivos.Add(dispositivo);
                await _dbContext.SaveChangesAsync();

                var nuevaOrden = new OrdenServicios
                { 
                    IdDispositivo = dispositivo.IdDispositivo,
                    FechaIngreso = dispositivo.FechaIngreso,
                    ProblemaReportado = dispositivo.Detalle,
                    Estado = "Ingresada",
                    Observaciones = "Revision",
                    Cedula = int.Parse(cedulaClaim)
                };
                _dbContext.OrdenServicios.Add(nuevaOrden);
                await _dbContext.SaveChangesAsync();
            }

            public async Task<List<Dispositivos>> ObtenerDispositivosConClientesAsync()
            {
                return await _dbContext.Dispositivos
                    .Include(d => d.Cliente)
                    .ToListAsync();
            }

            public async Task<Dispositivos?> ObtenerPorIdAsync(int idDispositivo)
            {
                return await _dbContext.Dispositivos
                    .Include(d => d.Cliente)
                    .FirstOrDefaultAsync(d => d.IdDispositivo == idDispositivo);
            }
            public async Task ActualizarDispositivoAsync(Dispositivos dispositivo)
            {
                _dbContext.Dispositivos.Update(dispositivo);
                await _dbContext.SaveChangesAsync();
            }
    }

 }

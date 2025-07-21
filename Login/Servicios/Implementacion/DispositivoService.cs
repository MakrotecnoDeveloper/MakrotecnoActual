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

            public async Task CrearDispositivoAsync(Dispositivo dispositivo)
            {
                var cliente = await _dbContext.Cliente.FindAsync(dispositivo.CedulaCliente);
                if (cliente == null)
                    throw new Exception("El cliente no existe.");

                _dbContext.Dispositivos.Add(dispositivo);
                await _dbContext.SaveChangesAsync();
            }

            public async Task<List<Dispositivo>> ObtenerDispositivosConClientesAsync()
            {
                return await _dbContext.Dispositivos
                    .Include(d => d.Cliente)
                    .ToListAsync();
            }

            public async Task<Dispositivo?> ObtenerPorIdAsync(int idDispositivo)
            {
                return await _dbContext.Dispositivos
                    .Include(d => d.Cliente)
                    .FirstOrDefaultAsync(d => d.IdDispositivo == idDispositivo);
            }
            public async Task ActualizarDispositivoAsync(Dispositivo dispositivo)
            {
                _dbContext.Dispositivos.Update(dispositivo);
                await _dbContext.SaveChangesAsync();
            }
    }

 }

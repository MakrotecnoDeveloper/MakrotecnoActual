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
        public async Task AgregarDispositivoAsync(Dispositivo dispositivo)
        {
            _dbContext.Dispositivos.Add(dispositivo);
            await _dbContext.SaveChangesAsync();
        }
    }
}

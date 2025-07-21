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
    }
}

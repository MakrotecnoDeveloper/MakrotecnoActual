using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Servicios.Implementacion
{
    public class ReporteService : IReporteService
    {
        private readonly BaseAdmContext _dbContext;
        public ReporteService(BaseAdmContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ReporteFinancieroViewModel> GenerarReporteDelDiaAsync(DateTime fecha)
        {
            var movimientos = await _dbContext.FlujoCajas.Where(m => m.Fecha.Date == fecha.Date).ToListAsync();

            var ingresos = movimientos.Where(m => m.TipoMovimiento == "Ingreso").Sum(m => m.Monto);
            var costos = movimientos.Where(m => m.TipoMovimiento == "Egreso").Sum(m => m.Monto);
            var utilidad = ingresos - costos;

            return new ReporteFinancieroViewModel
            {
                Fecha = fecha,
                Ingresos = ingresos,
                Costos = costos,
                Utilidad = utilidad,
                BalanceCaja = ingresos - costos,
                Movimientos = movimientos
            };
        }
    }
}
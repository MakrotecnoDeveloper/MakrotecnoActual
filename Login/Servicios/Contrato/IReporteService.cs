using Plataforma.Models;

namespace Plataforma.Servicios.Contrato
{
    public interface IReporteService
    {
        Task<ReporteFinancieroViewModel> GenerarReporteDelDiaAsync(DateTime fecha);
    }
}

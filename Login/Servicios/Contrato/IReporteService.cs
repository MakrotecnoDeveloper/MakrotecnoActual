using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IReporteService
    {
        Task<List<ReporteItem>> GenerarReporte(DateTime fechaInicio, DateTime fechaFin, string tipoReporte);
    }
}
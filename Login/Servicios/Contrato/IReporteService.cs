using Plataforma.Models;

namespace Plataforma.Servicios.Contrato
{
    public interface IReporteService
    {
        Task<ReporteFinancieroViewModel> GenerarReporteDelDiaAsync(DateTime fecha);
        (decimal totalSubTotal, decimal totalVNeto) TraerTotalesPorRangoYServicio(DateTime fechaInicio, DateTime fechaFin, int idServicio);
        List<ConceptoServicioVM> TraerServiciosUnicos(DateTime fechaInicio, DateTime fechaFin);
        List<ConceptoServicioVM> TraerUtilidadPorServicios(DateTime fechaInicio, DateTime fechaFin);
    }
}

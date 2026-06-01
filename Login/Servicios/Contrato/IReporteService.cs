using Plataforma.Models;
using Plataforma.Models.ViewModels.Reportes;
using Plataforma.ViewModels.Reportes;

namespace Plataforma.Servicios.Contrato
{
    public interface IReporteService
    {
        Task<ReporteGeneralVm> ObtenerVistaGeneralAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<byte[]> ExportarVistaGeneralExcelAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<ReporteVentasViewModel> ObtenerReporteVentasAsync(
            ReporteVentasFiltroViewModel filtros,
            int pagina,
            int registrosPorPagina
        );
        Task<CentroReportesViewModel> ObtenerCentroReportesAsync(
            CentroReportesFiltroViewModel filtros,
            int pagina,
            int registrosPorPagina
        );
        Task<CentroReportesViewModel> ObtenerCentroReportesAvanzadosAsync(
            CentroReportesFiltroViewModel filtros,
            int pagina,
            int registrosPorPagina
        );
    }
}

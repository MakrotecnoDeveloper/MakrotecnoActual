using Plataforma.Models;
using Plataforma.ViewModels.Reportes;

namespace Plataforma.Servicios.Contrato
{
    public interface IReporteService
    {
        Task<ReporteGeneralVm> ObtenerVistaGeneralAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<byte[]> ExportarVistaGeneralExcelAsync(DateTime fechaInicio, DateTime fechaFin);
    }
}

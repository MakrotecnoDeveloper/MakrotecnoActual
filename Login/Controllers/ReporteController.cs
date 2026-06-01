using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Servicios.Contrato;
using Plataforma.Models.ViewModels.Reportes;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Controllers
{
    [Authorize]
    public class ReporteController : Controller
    {
        private readonly IReporteService _reporteService;

        public ReporteController(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? fechaInicio, DateTime? fechaFin)
        {
            var desde = fechaInicio?.Date ?? DateTime.Today;
            var hasta = fechaFin?.Date ?? DateTime.Today;

            var model = await _reporteService.ObtenerVistaGeneralAsync(desde, hasta);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ExportarExcel(DateTime fechaInicio, DateTime fechaFin)
        {
            var bytes = await _reporteService.ExportarVistaGeneralExcelAsync(fechaInicio, fechaFin);

            var nombre = $"ReporteGeneral_{fechaInicio:yyyyMMdd}_{fechaFin:yyyyMMdd}.xlsx";

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                nombre);
        }

        [HttpGet]
        public async Task<IActionResult> ReporteVentas(
            ReporteVentasFiltroViewModel filtros,
            int pagina = 1,
            int registrosPorPagina = 20)
        {
            var vm = await _reporteService.ObtenerReporteVentasAsync(
                filtros,
                pagina,
                registrosPorPagina
            );

            return View(vm);
        }
        [HttpGet]
        public async Task<IActionResult> CentroReportes(
            CentroReportesFiltroViewModel filtros,
            int pagina = 1,
            int registrosPorPagina = 20)
        {
            var vm = await _reporteService.ObtenerCentroReportesAsync(
                filtros,
                pagina,
                registrosPorPagina
            );

            return View(vm);
        }
        [HttpGet]
        public async Task<IActionResult> CentroReportesAvanzados(
            CentroReportesFiltroViewModel filtros,
            int pagina = 1,
            int registrosPorPagina = 20)
        {
            var vm = await _reporteService.ObtenerCentroReportesAvanzadosAsync(
                filtros,
                pagina,
                registrosPorPagina
            );

            return View("CentroReportes", vm);
        }

    }
}
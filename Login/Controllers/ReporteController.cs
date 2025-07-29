using Microsoft.AspNetCore.Mvc;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Controllers
{
    public class ReporteController : Controller
    {
        private readonly IReporteService _reporteService;
        public ReporteController(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }
        public async Task<IActionResult> Diario()
        {
            var reporte = await _reporteService.GenerarReporteDelDiaAsync(DateTime.Today);
            return View(reporte);
        }
    }
}
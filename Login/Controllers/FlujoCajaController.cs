using Microsoft.AspNetCore.Mvc;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using Plataforma.Servicios.Implementacion;

namespace Plataforma.Controllers
{
    public class FlujoCajaController : Controller
    {
        private readonly IFlujoCajaService _flujocajaService;
        public FlujoCajaController(IFlujoCajaService flujocajaService)
        {
            _flujocajaService = flujocajaService;
        }
        public async Task<IActionResult> Index()
        {
            var movimientos = await _flujocajaService.ObtenerMovimientosPorFechaAsync(DateTime.Today);
            return View(movimientos);
        }
        [HttpGet]
        public async Task<IActionResult> RegistrarCierre()
        {
            var totalFacturado = await _flujocajaService.ObtenerTotalFacturadoHoyAsync();

            var model = new CierreCajaViewModel
            {
                Fecha = DateTime.Today,
                TotalFacturado = totalFacturado
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> RegistrarCierre(CierreCajaViewModel model)
        {
            var cedulaStr = User.FindFirst("Cedula")?.Value;
            if (!int.TryParse(cedulaStr, out int cedula))
            {
                ViewBag.ErrorMessage = "Cédula inválida o no autenticada.";
                return View("Index", model);
            }

            var (exito, error) = await _flujocajaService.RegistrarCierreAsync(model, cedula);

            if (!exito)
            {
                ViewBag.ErrorMessage = error;
                return View("Index", model);
            }

            return RedirectToAction("RegistrarCierre");
        }
    }
}
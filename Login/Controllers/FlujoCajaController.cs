using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Controllers
{
    public class FlujoCajaController : Controller
    {
        private readonly IFlujoCajaService _flujocajaService;
        private readonly BaseAdmContext _dbContext;
        public FlujoCajaController(IFlujoCajaService flujocajaService, BaseAdmContext dbContext)
        {
            _flujocajaService = flujocajaService;
            _dbContext = dbContext;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = await _flujocajaService.ObtenerResumenDeHoyAsync();
            return View(vm);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> RegistrarCierre()
        {
            var model = new CierreCajaViewModel();

            // 1) Traer conceptos del día y precargar la tabla
            model.Conceptos = await _flujocajaService.ObtenerConceptosDelDiaAsync();

            // 2) (Opcional) Calcular TotalFacturado con la suma de VNeto de todos los pedidos del día
            var inicio = DateTime.Today;
            var fin = inicio.AddDays(1);
            model.TotalFacturado = await _dbContext.Pedidos
                .Where(p => p.FechaRegistro >= inicio && p.FechaRegistro < fin)
                .SumAsync(p => (decimal?)p.VNeto) ?? 0m;

            return View(model);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarCierre(CierreCajaViewModel model)
        {
            // 1) Validar autenticación/cedula
            var cedulaStr = User.FindFirst("Cedula")?.Value;
            if (!int.TryParse(cedulaStr, out int cedula))
            {
                ViewBag.ErrorMessage = "Cédula inválida o no autenticada.";
                // Devuelve la misma vista del formulario, no "Index"
                return View("RegistrarCierre", model);
            }

            // 2) Validar modelo antes de ir al servicio
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Por favor corrige los errores del formulario.";
                return View("RegistrarCierre", model);
            }

            try
            {
                var (exito, error) = await _flujocajaService.RegistrarCierreAsync(model, cedula);

                if (!exito)
                {
                    ViewBag.ErrorMessage = error ?? "No fue posible registrar el cierre.";
                    return View("RegistrarCierre", model);
                }

                TempData["Ok"] = "Cierre registrado correctamente.";
                // Redirección PRG para evitar doble envío
                return RedirectToAction(nameof(RegistrarCierre));
            }
            catch (Exception ex)
            {
                // Loggea ex si tienes logger
                ViewBag.ErrorMessage = "Ocurrió un error al registrar el cierre.";
                return View("RegistrarCierre", model);
            }
        }
    }
}
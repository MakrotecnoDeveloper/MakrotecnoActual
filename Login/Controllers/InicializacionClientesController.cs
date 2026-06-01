using MakroTecno.Models.ViewModels.InicializacionClientes;
using MakroTecno.Services.InicializacionClientes;
using Microsoft.AspNetCore.Mvc;

namespace MakroTecno.Controllers
{
    public class InicializacionClientesController : Controller
    {
        private readonly IInicializacionClienteService _inicializacionService;

        public InicializacionClientesController(IInicializacionClienteService inicializacionService)
        {
            _inicializacionService = inicializacionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? nit)
        {
            var model = await _inicializacionService.ObtenerPanelAsync(nit);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearCliente(CrearClienteInicialViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Revisa los campos obligatorios antes de continuar.";
                return RedirectToAction(nameof(Index), new { nit = model.IdEmpresa });
            }

            var resultado = await _inicializacionService.CrearClienteInicialAsync(model);

            if (!resultado.Ok)
            {
                TempData["Error"] = resultado.Mensaje;
                return RedirectToAction(nameof(Index), new { nit = model.IdEmpresa });
            }

            TempData["Success"] = resultado.Mensaje;
            return RedirectToAction(nameof(Index), new { nit = model.IdEmpresa });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarPlan(string idEmpresa, int idPlan)
        {
            var resultado = await _inicializacionService.CambiarPlanAsync(idEmpresa, idPlan);

            TempData[resultado.Ok ? "Success" : "Error"] = resultado.Mensaje;

            return RedirectToAction(nameof(Index), new { nit = idEmpresa });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivarModulo(string idEmpresa, int idModulo)
        {
            var resultado = await _inicializacionService.ActivarModuloAsync(idEmpresa, idModulo);

            TempData[resultado.Ok ? "Success" : "Error"] = resultado.Mensaje;

            return RedirectToAction(nameof(Index), new { nit = idEmpresa });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DesactivarModulo(string idEmpresa, int idModulo)
        {
            var resultado = await _inicializacionService.DesactivarModuloAsync(idEmpresa, idModulo);

            TempData[resultado.Ok ? "Success" : "Error"] = resultado.Mensaje;

            return RedirectToAction(nameof(Index), new { nit = idEmpresa });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearModulo(CrearModuloViewModel model)
        {
            var resultado = await _inicializacionService.CrearModuloAsync(model);

            TempData[resultado.Ok ? "Success" : "Error"] = resultado.Mensaje;

            return RedirectToAction(nameof(Index), new { nit = model.IdEmpresa });
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Plataforma.Models.ViewModels.Cxc;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Controllers
{
    [Authorize]
    public class CxcController : Controller
    {
        private readonly ICxcService _cxcService;

        public CxcController(ICxcService cxcService)
        {
            _cxcService = cxcService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? texto, string? estado)
        {
            var model = await _cxcService.ObtenerListadoAsync(texto, estado);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var model = await _cxcService.ObtenerDetalleAsync(id);

            if (model == null)
            {
                TempData["ErrorMessage"] = "La cuenta por cobrar no existe.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> RegistrarPago(int idCxc)
        {
            var detalle = await _cxcService.ObtenerDetalleAsync(idCxc);

            if (detalle == null)
            {
                TempData["ErrorMessage"] = "La cuenta por cobrar no existe.";
                return RedirectToAction(nameof(Index));
            }

            var metodos = await _cxcService.ObtenerMetodosPagoAsync();

            ViewBag.MetodosPago = metodos.Select(x => new SelectListItem
            {
                Value = x.IdMetodo.ToString(),
                Text = x.Metodo
            }).ToList();

            var model = new RegistrarPagoCxcViewModel
            {
                IdCxc = idCxc,
                FechaPago = DateTime.Now
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarPago(RegistrarPagoCxcViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var metodos = await _cxcService.ObtenerMetodosPagoAsync();
                ViewBag.MetodosPago = metodos.Select(x => new SelectListItem
                {
                    Value = x.IdMetodo.ToString(),
                    Text = x.Metodo
                }).ToList();

                return View(model);
            }

            var resultado = await _cxcService.RegistrarPagoAsync(model);

            if (!resultado.Ok)
            {
                TempData["ErrorMessage"] = resultado.Mensaje;
                return RedirectToAction(nameof(Detalle), new { id = model.IdCxc });
            }

            TempData["SuccessMessage"] = "Pago registrado correctamente.";
            return RedirectToAction(nameof(Detalle), new { id = model.IdCxc });
        }
    }
}
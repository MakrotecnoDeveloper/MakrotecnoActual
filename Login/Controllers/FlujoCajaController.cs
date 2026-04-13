using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Models.Dto.Pedido;
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
        private ContextoAccesoDto ObtenerContextoAcceso()
        {
            return new ContextoAccesoDto
            {
                Cedula = int.Parse(User.FindFirst("Cedula")?.Value ?? "0"),
                EmpresaId = User.FindFirst("EmpresaId")?.Value ?? "",
                SedeId = int.Parse(User.FindFirst("SedeId")?.Value ?? "0"),
                PdvId = int.Parse(User.FindFirst("PdvId")?.Value ?? "0"),
                NombreRol = User.FindFirst("NombreRol")?.Value ?? ""
            };
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
        public async Task<IActionResult> RegistrarCierre(DateTime? fecha = null)
        {
            var ctx = ObtenerContextoAcceso();
            var fechaTrabajo = (fecha ?? DateTime.Today).Date;

            var model = new CierreCajaViewModel
            {
                Fecha = fechaTrabajo,
                Conceptos = await _flujocajaService.ObtenerConceptosDelDiaAsync(ctx, fechaTrabajo),
                TotalFacturado = await _flujocajaService.ObtenerTotalFacturadoAsync(ctx, fechaTrabajo)
            };

            ViewBag.FechaTrabajo = fechaTrabajo.ToString("dd/MM/yyyy");
            return View(model);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarCierre(CierreCajaViewModel model)
        {
            var ctx = ObtenerContextoAcceso();

            if (ctx.Cedula <= 0 || ctx.PdvId <= 0 || ctx.SedeId <= 0 || string.IsNullOrWhiteSpace(ctx.EmpresaId))
            {
                ViewBag.ErrorMessage = "No fue posible identificar el contexto de empresa, sede o PDV.";
                return View("RegistrarCierre", model);
            }

            if (!ModelState.IsValid)
            {
                model.Conceptos = await _flujocajaService.ObtenerConceptosDelDiaAsync(ctx, model.Fecha.Date);
                model.TotalFacturado = await _flujocajaService.ObtenerTotalFacturadoAsync(ctx, model.Fecha.Date);
                ViewBag.FechaTrabajo = model.Fecha.ToString("dd/MM/yyyy");
                ViewBag.ErrorMessage = "Por favor corrige los errores del formulario.";
                return View("RegistrarCierre", model);
            }

            try
            {
                var (exito, error) = await _flujocajaService.RegistrarCierreAsync(model, ctx);

                if (!exito)
                {
                    model.Conceptos = await _flujocajaService.ObtenerConceptosDelDiaAsync(ctx, model.Fecha.Date);
                    model.TotalFacturado = await _flujocajaService.ObtenerTotalFacturadoAsync(ctx, model.Fecha.Date);
                    ViewBag.FechaTrabajo = model.Fecha.ToString("dd/MM/yyyy");
                    ViewBag.ErrorMessage = error ?? "No fue posible registrar el cierre.";
                    return View("RegistrarCierre", model);
                }

                TempData["Ok"] = "Cierre registrado correctamente.";
                return RedirectToAction(nameof(RegistrarCierre), new { fecha = model.Fecha.ToString("yyyy-MM-dd") });
            }
            catch
            {
                model.Conceptos = await _flujocajaService.ObtenerConceptosDelDiaAsync(ctx, model.Fecha.Date);
                model.TotalFacturado = await _flujocajaService.ObtenerTotalFacturadoAsync(ctx, model.Fecha.Date);
                ViewBag.FechaTrabajo = model.Fecha.ToString("dd/MM/yyyy");
                ViewBag.ErrorMessage = "Ocurrió un error al registrar el cierre.";
                return View("RegistrarCierre", model);
            }
        }
    }
}
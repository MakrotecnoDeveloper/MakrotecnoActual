using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Models.Dto.Pedido;
using Plataforma.Servicios.Contrato;
using System.Globalization;

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

            // IMPORTANTE: reconstruir los valores manualmente desde Request.Form
            model.Fecha = DateTime.TryParse(Request.Form["Fecha"], out var fecha)
                ? fecha.Date
                : DateTime.Today;

            model.Efectivo = ParseDecimalEstricto(Request.Form["EfectivoVisible"]);
            model.Transferencia = ParseDecimalEstricto(Request.Form["TransferenciaVisible"]);
            model.GastoEfectivo = ParseDecimalEstricto(Request.Form["GastoEfectivoVisible"]);
            model.GastoTransferencia = ParseDecimalEstricto(Request.Form["GastoTransferenciaVisible"]);

            model.Conceptos = LeerConceptosDesdeRequest();

            // Limpiar el ModelState para que tome estos valores corregidos
            ModelState.Clear();
            TryValidateModel(model);

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
            catch (Exception ex)
            {
                ViewBag.FechaTrabajo = model.Fecha.ToString("dd/MM/yyyy");
                ViewBag.ErrorMessage = $"Ocurrió un error al registrar el cierre. Detalle: {ex.Message}";
                return View("RegistrarCierre", model);
            }
        }
        private decimal ParseDecimalEstricto(string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return 0m;

            var raw = valor.Trim().Replace(" ", "");

            // Caso: 12.000,00  -> 12000.00
            if (raw.Contains(",") && raw.Contains("."))
            {
                raw = raw.Replace(".", "");
                raw = raw.Replace(",", ".");
            }
            // Caso: 12000,00 -> 12000.00
            else if (raw.Contains(","))
            {
                raw = raw.Replace(",", ".");
            }
            // Caso: 12000.00 -> queda igual

            if (decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                return result;

            return 0m;
        }
        private List<ConceptoServicioVM> LeerConceptosDesdeRequest()
        {
            var conceptos = new List<ConceptoServicioVM>();
            int index = 0;

            while (Request.Form.ContainsKey($"Conceptos[{index}].NombreServicio") ||
                   Request.Form.ContainsKey($"Conceptos[{index}].IdServicio") ||
                   Request.Form.ContainsKey($"Conceptos[{index}].TotalSubTotal") ||
                   Request.Form.ContainsKey($"Conceptos[{index}].TotalVNeto"))
            {
                int.TryParse(Request.Form[$"Conceptos[{index}].IdServicio"], out var idServicio);

                conceptos.Add(new ConceptoServicioVM
                {
                    IdServicio = idServicio,
                    NombreServicio = Request.Form[$"Conceptos[{index}].NombreServicio"].ToString(),
                    TotalSubTotal = ParseDecimalEstricto(Request.Form[$"Conceptos[{index}].TotalSubTotal"]),
                    TotalVNeto = ParseDecimalEstricto(Request.Form[$"Conceptos[{index}].TotalVNeto"])
                });

                index++;
            }

            return conceptos;
        }
    }
}
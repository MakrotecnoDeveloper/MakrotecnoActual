// Controllers/ImportacionController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Plataforma.Models;
using Plataforma.Services;

namespace Plataforma.Controllers
{
    [Authorize]
    public class ImportacionController : Controller
    {
        private readonly IImportacionService _importacionService;

        public ImportacionController(IImportacionService importacionService)
        {
            _importacionService = importacionService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new ImportacionProductosViewModel
            {
                IdEmpresa = User.FindFirst("EmpresaId")?.Value ?? "1234193470-6",
                IdCatePro = 1035,
                IdProveedor = 24,
                IdUnidad = 1,
                Iva = 0,
                MargenPorcentaje = 12,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Procesar(ImportacionProductosViewModel model)
        {
            model.IdEmpresa = User.FindFirst("EmpresaId")?.Value ?? "1234193470-6";

            var usuario = User.Identity?.Name ?? User.FindFirst("Cedula")?.Value ?? "Sistema";

            ResultadoImportacionDto resultado;

            if (model.TipoEntrada == "Texto")
            {
                resultado = await _importacionService.ProcesarTextoAsync(model, usuario);
            }
            else if (model.TipoEntrada == "Excel")
            {
                resultado = await _importacionService.ProcesarExcelAsync(model, usuario);
            }
            else
            {
                TempData["Error"] = "Debe seleccionar el tipo de entrada.";
                return RedirectToAction(nameof(Index));
            }

            return View("Preview", resultado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirmar(int idImportacion, string tipoOperacion, string camposOrdenadosJson)
        {
            var campos = new List<string>();

            if (!string.IsNullOrWhiteSpace(camposOrdenadosJson))
            {
                campos = JsonConvert.DeserializeObject<List<string>>(camposOrdenadosJson);
            }

            var resultado = await _importacionService.ConfirmarImportacionAsync(
                idImportacion,
                tipoOperacion,
                campos
            );

            TempData["Mensaje"] = resultado.Mensaje;

            return View("Preview", resultado);
        }
    }
}
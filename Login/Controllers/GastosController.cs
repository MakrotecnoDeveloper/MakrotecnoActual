using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using Plataforma.Servicios.Implementacion;

namespace Plataforma.Controllers
{
    public class GastosController : Controller
    {
        private readonly IGastosService _gastosService;
        public GastosController(IGastosService gastosService)
        {
            _gastosService = gastosService;
        }
        [HttpGet]
        public IActionResult Index() => View();

        // --- API JSON para la vista ---

        [HttpGet]
        public async Task<IActionResult> Listar(int year, int month)
        {
            var data = await _gastosService.ListarPorMesAsync(year, month);
            return Json(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([FromBody] GastosMensuales input)
        {
            if (input == null) return BadRequest();
            var creado = await _gastosService.CrearAsync(input);
            return Json(creado);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [FromBody] GastosMensuales input)
        {
            var editado = await _gastosService.EditarAsync(id, input);
            if (editado == null) return NotFound();
            return Json(editado);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var ok = await _gastosService.EliminarAsync(id);
            return ok ? Ok() : NotFound();
        }
        [HttpGet]
        public async Task<IActionResult> Obtener(int id)
        {
            var g = await _gastosService.ObtenerAsync(id);
            return g is null ? NotFound() : Json(g);
        }
    }
}

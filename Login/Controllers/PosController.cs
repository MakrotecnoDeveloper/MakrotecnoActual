using Microsoft.AspNetCore.Mvc.Rendering;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Models.ViewModels.Pos;

namespace Plataforma.Controllers
{
    [Authorize]
    public class PosController : Controller
    {
        private readonly IPosService _posService;
        public PosController(IPosService posService)
        {
            _posService = posService;
        }
        [HttpGet]
        public async Task<IActionResult> POS()
        {
            var model = await _posService.ObtenerPantallaAsync(User);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> BuscarProductos(string? texto, string? categoria, int pagina = 1, int tamanoPagina = 24)
        {
            var productos = await _posService.BuscarProductosAsync(texto, categoria, pagina, tamanoPagina, User);
            return Json(productos);
        }

        [HttpGet]
        public async Task<IActionResult> BuscarProductoPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return BadRequest(new { mensaje = "Debe ingresar un código." });

            var producto = await _posService.BuscarProductoPorCodigoAsync(codigo, User);

            if (producto == null)
                return NotFound(new { mensaje = "Producto no encontrado en esta sede." });

            return Json(producto);
        }

        [HttpPost]
        public async Task<IActionResult> Facturar([FromBody] PosCrearVentaRequest request)
        {
            if (request == null)
                return BadRequest(new { mensaje = "La solicitud es inválida." });

            var resultado = await _posService.FacturarVentaPosAsync(request, User);

            if (!resultado.Ok)
                return BadRequest(resultado);

            return Json(resultado);
        }
    }
}
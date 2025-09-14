using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using System.Security.Claims;

namespace Plataforma.Controllers
{
    public class GananciaController : Controller
    {
        private readonly IGananciaService _gananciaService;
        public GananciaController(IGananciaService gananciaService)
        {
            _gananciaService = gananciaService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var ganancias = await _gananciaService.ListarGananciasAsync();
            return View(ganancias);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GananciaDelDia()
        {
            var cedulaClaim = User.FindFirst("Cedula")?.Value;

            if (!int.TryParse(cedulaClaim, out int cedulaUsuario))
            {
                return Unauthorized();
            }

            var ganancia = await _gananciaService.ObtenerGananciaDelDiaAsync(cedulaUsuario);

            if (ganancia != null)
            {
                await _gananciaService.GuardarGananciaDelDiaAsync(ganancia);
            }

            return View(ganancia);
        }
    }
}
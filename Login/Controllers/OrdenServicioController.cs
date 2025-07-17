using Microsoft.AspNetCore.Mvc;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Controllers
{
    public class OrdenServicioController : Controller
    {
        private readonly IDispositivoService _dispositivoService;
        public OrdenServicioController(IDispositivoService dispositivoService)
        {
            _dispositivoService = dispositivoService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Dispositivo() 
        {
            return View();
        }
        public IActionResult OrdenesServicio()
        {
            return View();
        }
        public IActionResult SeguimientoServicio()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CrearDispositivo([FromBody] Dispositivo dispositivo)
        {
            Console.WriteLine(dispositivo);
            if (ModelState.IsValid)
            {
                await _dispositivoService.AgregarDispositivoAsync(dispositivo);
                return Ok();
            }
            return BadRequest();
        }
    }
}

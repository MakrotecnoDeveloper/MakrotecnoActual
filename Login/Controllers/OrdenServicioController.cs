using Microsoft.AspNetCore.Mvc;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Controllers
{
    public class OrdenServicioController : Controller
    {
        private readonly IDispositivoService _dispositivoService;
        private readonly IOrdenServicioService _ordenServicioService;
        public OrdenServicioController(IDispositivoService dispositivoService, IOrdenServicioService ordenServicioService)
        {
            _dispositivoService = dispositivoService;
            _ordenServicioService = ordenServicioService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Dispositivos()
        {
            var dispositivos = await _dispositivoService.ObtenerDispositivosConClientesAsync();
            return View(dispositivos);
        }

        [HttpPost]
        public async Task<IActionResult> CrearDispositivo([FromBody] Dispositivo dispositivo)
        {
            try
            {
                await _dispositivoService.CrearDispositivoAsync(dispositivo);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var dispositivo = await _dispositivoService.ObtenerPorIdAsync(id);
            if (dispositivo == null)
                return NotFound();

            return View(dispositivo);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dispositivo = await _dispositivoService.ObtenerPorIdAsync(id);
            if (dispositivo == null)
                return NotFound();

            return View(dispositivo);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Dispositivo dispositivo)
        {
            if (!ModelState.IsValid)
                return View(dispositivo);

            await _dispositivoService.ActualizarDispositivoAsync(dispositivo);
            TempData["Success"] = "Dispositivo actualizado correctamente.";
            return RedirectToAction("Dispositivos");
        }
        [HttpGet]
        public async Task<IActionResult> OrdenesServicio()
        {
            var ordenes = await _ordenServicioService.ObtenerTodasAsync();
            return View("OrdenesServicio", ordenes);
        }

        [HttpGet]
        public IActionResult CreateOrden()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrden(OrdenServicio orden)
        {
            await _ordenServicioService.CrearAsync(orden);
            return RedirectToAction("OrdenesServicio");
        }

        [HttpGet]
        public async Task<IActionResult> EditOrden(int id)
        {
            var orden = await _ordenServicioService.ObtenerPorIdAsync(id);
            if (orden == null)
                return NotFound();

            return View(orden);
        }

        [HttpPost]
        public async Task<IActionResult> EditOrden(OrdenServicio orden)
        {
            //return View(orden);
            var cedulaClaim = User.FindFirst("Cedula")?.Value;
            if (int.TryParse(cedulaClaim, out int cedulaEmpleado))
            {
                await _ordenServicioService.ActualizarAsync(orden, cedulaEmpleado);
            }
            return RedirectToAction("OrdenesServicio");
        }

        [HttpGet]
        public async Task<IActionResult> DetailsOrden(int id)
        {
            var orden = await _ordenServicioService.ObtenerPorIdAsync(id);
            if (orden == null)
                return NotFound();

            return View(orden);
        }
    }
}

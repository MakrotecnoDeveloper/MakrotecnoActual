using Microsoft.AspNetCore.Mvc;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using Plataforma.Servicios.Implementacion;

namespace Plataforma.Controllers
{
    public class OrdenServicioController : Controller
    {
        private readonly IDispositivoService _dispositivoService;
        private readonly IOrdenServicioService _ordenServicioService;
        private readonly ITercerosService _tercerosService;
        private readonly IProductoService _productoService;
        public OrdenServicioController(IDispositivoService dispositivoService, IOrdenServicioService ordenServicioService, ITercerosService tercerosService, IProductoService productoService)
        {
            _dispositivoService = dispositivoService;
            _ordenServicioService = ordenServicioService;
            _tercerosService = tercerosService;
            _productoService = productoService;
        }
        public async Task<IActionResult> Index()
        {
            var model = new OrdenesServicioViewModel
            {
                TipoDispositivos = await _dispositivoService.ObtenerTipoDispositivos(),
                Dispositivos = await _dispositivoService.ObtenerDispositivosConClientesAsync(),
                Clientes = await _tercerosService.ObtenerClientes(),
                OrdenServicios = await _ordenServicioService.ObtenerTodasAsync(),
                Proveedores = await _tercerosService.ObtenerProveedoresAsync()
            };
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Dispositivos()
        {
            var model = new OrdenesServicioViewModel
            {
                TipoDispositivos = await _dispositivoService.ObtenerTipoDispositivos(),
                Dispositivos = await _dispositivoService.ObtenerDispositivosConClientesAsync()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CrearDispositivo([FromBody] Dispositivos dispositivo)
        {
            try
            {
                var cedulaClaim = User.FindFirst("Cedula")?.Value;
                await _dispositivoService.CrearDispositivoAsync(dispositivo, cedulaClaim);
                return Ok(new { success = true, message = "Dispositivo creado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
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
        public async Task<IActionResult> Edit(Dispositivos dispositivo)
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
            var model = new OrdenesServicioViewModel
            {
                OrdenServicios = await _ordenServicioService.ObtenerTodasAsync()
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult CreateOrden()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrden(OrdenServicios orden)
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
        public async Task<IActionResult> EditOrden(OrdenServicios orden)
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
        [HttpGet]
        public async Task<IActionResult> DetalleHistOrden(int id)
        {
            var orden = await _ordenServicioService.ObtenerOrdenPorIdAsync(id);
            if (orden == null)
                return NotFound();

            return PartialView("_DetalleOrden", orden);
        }
        [HttpGet]
        public async Task<IActionResult> CrearHistOrden(int id)
        {
            try
            {
                var (orden, mostrarAgregarProductos) =
                    await _ordenServicioService.ObtenerOrdenYPermisosAsync(id);

                var model = new OrdenesServicioViewModel
                {
                    IdOrden = orden.IdOrden,
                    Estado = orden.Estado,
                    MostrarAgregarProductos = mostrarAgregarProductos
                };

                return PartialView("_FormNuevaOrden", model);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CrearHistOrden(HistOrdSer model, string[]? Cod_Producto)
        {
            var orden = await _ordenServicioService.ObtenerPorIdAsync(model.IdOrden);
            if (orden == null)
                return NotFound("La orden no existe");

            await _ordenServicioService.CrearHistOrdenAsync(model, Cod_Producto, orden.Cedula);
            return Ok();
        }
        [HttpGet]
        public IActionResult Buscar(string searchTerm)
        {
            int categoriaTerm = 0;
            var productos = _productoService.BuscarProductos(searchTerm, categoriaTerm);

            var resultado = productos.Select(p => new {
                id = p.Cod_Producto,
                text = $"{p.Cod_Producto}"
            });

            return Json(resultado);
        }
        [HttpGet]
        public async Task<IActionResult> ModOrden(int id)
        {
            var orden = await _ordenServicioService.ObtenerPorIdAsync(id);
            if (orden == null)
                return NotFound();

            return PartialView("_EditarOrden", orden);
        }
        [HttpPost]
        public async Task<IActionResult> EditarOrden(OrdenServicios model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _ordenServicioService.ActualizarOrdenAsync(model, User);
            return Ok();
        }
    }
}

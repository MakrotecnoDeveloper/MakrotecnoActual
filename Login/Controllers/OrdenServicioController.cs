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
                var dispositivoCreado = await _dispositivoService.CrearDispositivoAsync(dispositivo, cedulaClaim);

                return Ok(new 
                { 
                    success = true, 
                    message = "Dispositivo creado correctamente",
                    dispositivo = new 
                    {
                        idDispositivo = dispositivoCreado.IdDispositivo,
                        imei = dispositivoCreado.IMEI,
                        detalle = dispositivoCreado.Detalle,
                        idCliente = dispositivoCreado.IdCliente,
                        nombreCliente = dispositivoCreado.Cliente?.NombreCliente,
                        fechaCliente = dispositivoCreado.FechaIngreso
                    }
                });
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
        [HttpPost]
        public async Task<IActionResult> CrearOrden([FromBody] OrdenServicios orden)
        {
            var cedulaClaim = User.FindFirst("Cedula")?.Value;
            var nuevaOrden = await _ordenServicioService.CrearAsync(orden, cedulaClaim);
            var dto = await _ordenServicioService.GetOrdenRowAsync(nuevaOrden.IdOrden);
            return Ok(new {
                    success = true,
                    message = $"Orden creada exitosamente #{nuevaOrden.IdOrden}",
                    orden = dto
                });
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
            /*if (!ModelState.IsValid)
                return BadRequest(ModelState);*/

            await _ordenServicioService.ActualizarOrdenAsync(model, User);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> AsignarEmpleado(int id)
        {
            try
            {
                var empleados = await _ordenServicioService.ObtenerEmpleadosAsync();

                var model = new OrdenesServicioViewModel
                {
                    Empleados = empleados,
                    IdOrden = id
                    
                };

                return PartialView("_FormAsignarEmpleado", model);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AsignarTecnico(OrdenServicios model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _ordenServicioService.ActualizarOrdenTecnico(model);

            if (updated == null)
                return NotFound("Orden no encontrada");

            return Ok(new { success = true, message = $"Empleado asignado a la orden #{updated.IdOrden}" });
        }
    }
}

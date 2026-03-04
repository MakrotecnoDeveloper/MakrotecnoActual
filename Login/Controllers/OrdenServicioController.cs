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
            var cedulaClaimStr = User.FindFirst("Cedula")?.Value;
            var rolClaim = User.FindFirst("NombreRol")?.Value;

            int? cedula = null;
            if (int.TryParse(cedulaClaimStr, out var c))
                cedula = c;

            // Trae SOLO las 10 últimas según rol
            var ordenes = await _ordenServicioService.ObtenerUltimas10Async(
                rolClaim ?? "",
                cedula
            );

            var model = new OrdenesServicioViewModel
            {
                TipoDispositivos = await _dispositivoService.ObtenerTipoDispositivos(),
                Dispositivos = await _dispositivoService.ObtenerDispositivosConClientesAsync(),
                Clientes = await _tercerosService.ObtenerClientes(),
                OrdenServicios = ordenes,
                Proveedores = await _tercerosService.ObtenerProveedoresAsync(),
                Sedeempleados = await _ordenServicioService.ObtenerEmpleadoSedeAsync(),
            };

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> BuscarOrdenPorId(int idOrden)
        {
            var rol = User.FindFirst("NombreRol")?.Value ?? "";
            var cedulaStr = User.FindFirst("Cedula")?.Value;

            int? cedula = null;
            if (int.TryParse(cedulaStr, out var c)) cedula = c;

            var orden = await _ordenServicioService.ObtenerPorIdAsync(idOrden);
            if (orden == null)
                return Json(new { success = false, message = "No existe una orden con ese ID." });

            var esAdmin = string.Equals(rol, "Administrador", StringComparison.OrdinalIgnoreCase);

            if (!esAdmin)
            {
                if (cedula == null)
                    return Json(new { success = false, message = "No se pudo validar tu cédula." });

                // OJO: si la orden está sin técnico (Cedula NULL), un técnico NO debería verla
                if (orden.Cedula == null || orden.Cedula != cedula.Value)
                    return Json(new { success = false, message = "No tienes permiso para ver esa orden." });
            }

            return Json(new
            {
                success = true,
                orden = new OrdenServicioRowDTO
                {
                    IdOrden = orden.IdOrden,
                    FechaIngreso = orden.FechaIngreso,
                    Cliente = orden.Dispositivo?.Cliente?.NombreCliente ?? "",
                    Telefono = orden.Dispositivo?.Cliente?.TelefonoCliente ?? "",
                    Password = orden.Dispositivo?.Clave ?? "",
                    Marca = orden.Dispositivo?.Marca ?? "",
                    Modelo = orden.Dispositivo?.Modelo ?? "",
                    Descripcion = orden.ProblemaReportado ?? "",
                    Observacion = orden.Observaciones ?? "",
                    Estado = orden.Estado ?? "",
                    Cedula = orden.Cedula
                }
            });
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
                var (orden, mostrarAgregarProductos) = await _ordenServicioService.ObtenerOrdenYPermisosAsync(id);
                var traerProveedores = await _tercerosService.ObtenerProveedoresAsync();

                var model = new OrdenesServicioViewModel
                {
                    IdOrden = orden.IdOrden,
                    Estado = orden.Estado,
                    MostrarAgregarProductos = mostrarAgregarProductos,
                    Proveedores = traerProveedores
                };

                return PartialView("_FormNuevaOrden", model);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CrearHistOrden(
        HistOrdSer model,
        string[]? Cod_Producto,
        int[] Stock,
        decimal[]? ValorRepuesto,
        string[]? Condicion,
        string[]? Tipo,
        int[] Proveedor
            )
        {
            var orden = await _ordenServicioService.ObtenerPorIdAsync(model.IdOrden);
            if (orden == null)
                return NotFound("La orden no existe");

            await _ordenServicioService.CrearHistOrdenAsync(model, Cod_Producto, Stock, ValorRepuesto, Condicion, Tipo, Proveedor, (int)orden.Cedula);
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
            bool procesado = await _ordenServicioService.ActualizarOrdenAsync(model, User);
            if(!procesado)
            {
                return Json(new { ok = true, mensaje = "La orden fue rechazada correctamente" });
            }
                return Json(new { ok = true, mensaje = "Orden actualizada con exito" });
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
        [HttpGet]
        public async Task<IActionResult> ObtenerFiltros()
        {
            // Rol y cédula del usuario logueado
            var rol = User.FindFirst("NombreRol")?.Value ?? "";
            var cedulaStr = User.FindFirst("Cedula")?.Value ?? "0";
            int cedula = int.TryParse(cedulaStr, out var c) ? c : 0;

            // Estados existentes en BD (distinct)
            var estados = await _ordenServicioService.ObtenerEstadosExistentesAsync();

            // Técnicos (admin: todos, técnico: solo él)
            var tecnicos = await _ordenServicioService.ObtenerTecnicosParaFiltroAsync(rol, cedula);

            return Ok(new
            {
                success = true,
                estados,
                tecnicos,
                rol,
                cedula
            });
        }

        [HttpGet]
        public async Task<IActionResult> FiltrarOrdenes(string estado = "__ALL__", int tecnico = 0)
        {
            var rol = User.FindFirst("NombreRol")?.Value ?? "";
            var cedulaStr = User.FindFirst("Cedula")?.Value ?? "0";
            int cedulaUsuario = int.TryParse(cedulaStr, out var c) ? c : 0;

            // Regla: si es técnico, SIEMPRE se filtra por él (ignore tecnico que venga)
            if (rol != "Administrador")
                tecnico = cedulaUsuario;

            // Si tecnico viene 0 o __ALL__ -> admin no filtra por tecnico
            int? tecnicoFiltro = (rol == "Administrador" && tecnico > 0) ? tecnico : (int?)null;

            // Si estado viene __ALL__ -> no filtra por estado
            string? estadoFiltro = (estado != "__ALL__") ? estado : null;

            // Trae las últimas 10 ya filtradas
            var ordenes = await _ordenServicioService.ObtenerUltimas10FiltradasAsync(estadoFiltro, tecnicoFiltro);

            // Mapea a DTO para pintar la tabla
            var dto = ordenes.Select(o => new OrdenServicioRowDTO
            {
                IdOrden = o.IdOrden,
                FechaIngreso = o.FechaIngreso,
                Cliente = o.Dispositivo?.Cliente?.NombreCliente ?? "",
                Telefono = o.Dispositivo?.Cliente?.TelefonoCliente ?? "",
                Password = o.Dispositivo?.Clave ?? "",
                Marca = o.Dispositivo?.Marca ?? "",
                Modelo = o.Dispositivo?.Modelo ?? "",
                Descripcion = o.ProblemaReportado ?? "",
                Observacion = o.Observaciones ?? "",
                Estado = o.Estado ?? "",
                Cedula = o.Cedula,
                // si quieres mostrar total:
                // Total = o.ValorPago ?? 0
            }).ToList();

            return Ok(new { success = true, ordenes = dto });
        }
    }
}

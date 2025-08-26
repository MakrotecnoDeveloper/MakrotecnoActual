using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Controllers
{
    public class InicioController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IInicioService _inicioService;
        public InicioController(IUsuarioService usuarioService, IInicioService inicioService)
        {
            _usuarioService = usuarioService;
            _inicioService = inicioService;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var cedulaClaim = User.FindFirst("Cedula");
            if (cedulaClaim != null && int.TryParse(cedulaClaim.Value, out int cedula))
            {
                int idPDVActual = _usuarioService.TraerUltimoIDPdv(cedula);
                var totalFactXDia = await _usuarioService.TraerFactXDia(cedula, idPDVActual);
                var viewModel = totalFactXDia.FirstOrDefault();
                return View(viewModel);
            }
            else
            {
                var mensaje = "El claim 'Cedula' no existe o la conversión falló.";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }

        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CuentasProximas(int id)
        {
            var cuentasProximas = await _usuarioService.ObtenerCuentasProximas(id);
            return PartialView("_CuentasProximas", cuentasProximas);
        }
        [Authorize]
        [HttpPost]
        public IActionResult ListarUsuarios(int cedula, string password)
        {
            Empleados usuario_buscar = _usuarioService.GetUsuarios(cedula, password);
            if (usuario_buscar == null)
            {
                var mensaje = "No se encontró ningún usuario con las credenciales especificadas. (SGE)";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }
            return View(usuario_buscar);
        }
        [Authorize]
        [HttpPost]
        public JsonResult EstadoPDV(int estadopdv, int idPDV)
        {
            var cedulaClaim = User.FindFirst("Cedula");
            if (cedulaClaim != null && int.TryParse(cedulaClaim.Value, out int cedula))
            {
                //Console.WriteLine("Punto de Venta: " + idPDV + " Estado del punto de venta: " + estadopdv);
                if (estadopdv < 0 || estadopdv > 2)
                {
                    var mensaje = "El estado del PDV debe ser 0, 1 o 2.";
                    return Json(new { success = false, message = mensaje });
                }
                var estado = _usuarioService.ValidarExistenteIdPDV(idPDV, cedula);
                Console.WriteLine("Punto de Venta: " + idPDV + " Estado ultimo asignado: " +  estado);
                if (estado != null)
                {
                    if (estado == 1 && estadopdv == 1)
                    {
                        var mensaje = "Ya hay una apertura de PDV registrada en el sistema.";
                        return Json(new { success = false, message = mensaje });
                    }
                    else if (estado == 0 && estadopdv == 0)
                    {
                        var mensaje = "Ya hay un cierre de PDV registrado en el sistema.";
                        return Json(new { success = false, message = mensaje });
                    }
                    else if (estado == 2 && estadopdv == 2)
                    {
                        var mensaje = "El PDV ya está en mantenimiento.";
                        return Json(new { success = false, message = mensaje });
                    }
                }
                    var agregarEstadoPDV = _usuarioService.AgregarEstadoPDV(estadopdv, idPDV, cedula);
                    if (agregarEstadoPDV == null)
                    {
                        var mensaje = "Error al actualizar el estado del PDV.";
                        return Json(new { success = false, message = mensaje });
                    }
                    else
                    {
                        return Json(new { success = true, message = "Estado del PDV actualizado correctamente." });
                    }
            }
            return null;
        }
        [HttpGet]
        public async Task<IActionResult> Ventas30(int dias = 30, int? idPdv = null, CancellationToken ct = default)
        {
            if (dias <= 0 || dias > 365) dias = 30;
            var data = await _inicioService.VentasUltimosDiasAsync(dias, idPdv, ct);
            Console.WriteLine("Esta es la data " + data);
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> VentasPorServicio(DateTime desde, DateTime hasta, int? idPdv = null, CancellationToken ct = default)
        {
            if (desde == default || hasta == default || hasta <= desde)
            {
                var d = DateTime.Today.AddDays(-29);
                var h = DateTime.Today.AddDays(1);
                desde = d; hasta = h;
            }
            var data = await _inicioService.VentasPorServicioAsync(desde, hasta, idPdv, ct);
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> OSAbiertasTop(int take = 8, CancellationToken ct = default)
        {
            if (take <= 0 || take > 100) take = 8;
            var data = await _inicioService.OSAbiertasTopAsync(take, ct);
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> StockBajo(int take = 8, int minimo = 5, CancellationToken ct = default)
        {
            if (take <= 0 || take > 100) take = 8;
            if (minimo < 0) minimo = 0;
            var data = await _inicioService.StockBajoAsync(take, minimo, ct);
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> ComprasRecientes(int take = 8, CancellationToken ct = default)
        {
            if (take <= 0 || take > 100) take = 8;
            var data = await _inicioService.ComprasRecientesAsync(take, ct);
            return Json(data);
        }
    }
}

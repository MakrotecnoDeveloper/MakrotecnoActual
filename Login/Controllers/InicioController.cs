using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Controllers
{
    public class InicioController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        public InicioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }
        [Authorize]
        public IActionResult Index()
        {
            var cedulaClaim = User.FindFirst("Cedula");
            if (cedulaClaim != null && int.TryParse(cedulaClaim.Value, out int cedula))
            {
                int idPDVActual = _usuarioService.TraerUltimoIDPdv(cedula);
                var totalFactXDia = _usuarioService.TraerFactXDia(cedula, idPDVActual);
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
            Empleado usuario_buscar = _usuarioService.GetUsuarios(cedula, password);
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
    }
}

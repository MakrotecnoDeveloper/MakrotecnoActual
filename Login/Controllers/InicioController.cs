using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public IActionResult Index()
        {
            var cedulaClaim = User.FindFirst("Cedula");
            int cedula = 0;
            if(cedulaClaim != null && int.TryParse(cedulaClaim.Value, out cedula)) 
            {
                var totalFactXDia = _usuarioService.TraerFactXDia(cedula);

                ViewBag.NombrePDV = TempData["NombrePDV"] ?? "No hay nombre de PDV disponible";
                ViewBag.IdPDV = TempData["IdPDV"];
                var viewModel = totalFactXDia.FirstOrDefault();
                return View(viewModel);
            }else {
                var mensaje = "El claim 'Cedula' no existe o la conversión falló.";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }
            
        }
        [HttpGet]
        public async Task<IActionResult> CuentasProximas(int id)
        {
            var cuentasProximas = await _usuarioService.ObtenerCuentasProximas(id);
            return PartialView("_CuentasProximas", cuentasProximas);
        }
        [HttpPost]
        public async Task<IActionResult> ListarUsuarios(int cedula, string password)
        {
            Empleado usuario_buscar = _usuarioService.GetUsuarios(cedula, password);
            if(usuario_buscar == null)
            {
                var mensaje = "No se encontró ningún usuario con las credenciales especificadas. (SGE)";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }
            return View(usuario_buscar);
        }
        [HttpPost]
        public JsonResult EstadoPDV(int estadopdv, int idPDV)
        {
            // Validar si estadopdv es 0, 1 o 2
            if (estadopdv < 0 || estadopdv > 2)
            {
                var mensaje = "El estado del PDV debe ser 0, 1 o 2.";
                return Json(new { success = false, message = mensaje });
            }
            var syncPDV = _usuarioService.ValidarExistenteIdPDV(idPDV);
            // Comprobar si el idPDV existe en la tabla SyncPDV

            if (syncPDV != null)
            {
                // Validar el estado actual
                if (syncPDV.estado == 1 && estadopdv == 1)
                {
                    var mensaje = "Ya hay una apertura de PDV registrada en el sistema.";
                    return Json(new { success = false, message = mensaje });
                }
                else if (syncPDV.estado == 0 && estadopdv == 0)
                {
                    var mensaje = "Ya hay un cierre de PDV registrado en el sistema.";
                    return Json(new { success = false, message = mensaje });
                }
                else if (syncPDV.estado == 2 && estadopdv == 2)
                {
                    var mensaje = "El PDV ya está en mantenimiento.";
                    return Json(new { success = false, message = mensaje });
                }
            }

            // Si no hay conflictos, procede a agregar o actualizar el estado
            var agregarEstadoPDV = _usuarioService.AgregarEstadoPDV(estadopdv, idPDV);

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
    }
}

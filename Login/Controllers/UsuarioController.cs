using Microsoft.AspNetCore.Mvc;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }
        public IActionResult Index()
        {
            var empleado = _usuarioService.ObtenerUsuarios();
            return View(empleado);
        }
        public IActionResult RegistrarEmpleado()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RegistrarDBEmpleado(int cedula, string nombre, string apellido, string genero, string correo, string rh, string celular, string contrasena)
        {
            try
            {
                var empleados = _usuarioService.RegistrarEmpleado(cedula, nombre, apellido, genero, correo, rh, celular, contrasena);
                return RedirectToAction("RegistrarEmpleado");
            }
            catch (Exception ex)
            {
                // Manejar la excepción, por ejemplo, podrías devolver una vista de error con un mensaje personalizado.
                return View("Error", ex.Message);
            }
        }
    }
}

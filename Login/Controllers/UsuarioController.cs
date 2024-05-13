using Microsoft.AspNetCore.Mvc;
using Plataforma.Servicios.Contrato;
using Plataforma.Servicios.Implementacion;

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
        public IActionResult Editar(int id)
        {
            var usuarioEncontrado = _usuarioService.BuscarUsuario(id);
            if (usuarioEncontrado.Any())
            {
                // Oculta la tabla de productos y muestra la tabla temporal
                return View(usuarioEncontrado);
            }
            else
            {
                // Producto no encontrado, maneja la lógica adecuada
                Console.WriteLine("No hay productos con ese codigo referenciado");
                return View("Index");
            }
        }
        [HttpPost]
        public IActionResult EditarEmpleado(int cedula, string nombre, string apellido, string genero, string correo, string rh, string celular, string contrasena)
        {
            // Llama al método EditarProducto del servicio de productos
            _usuarioService.EditarEmpleado(cedula, nombre, apellido, genero, correo, rh, celular, contrasena);

            // Redirige a la acción que deseas después de editar el producto
            return RedirectToAction("Index"); // Por ejemplo, redirigir a la página de inicio del controlador de productos
        }
        public IActionResult Cargos()
        {
            var cargos = _usuarioService.ObtenerCargos();
            return View(cargos);
        }
    }
}

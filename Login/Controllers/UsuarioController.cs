using Microsoft.AspNetCore.Mvc;
using Plataforma.Servicios.Contrato;
using Plataforma.Servicios.Implementacion;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public IActionResult FormCargos()
        {
            var empresas = _usuarioService.ObtenerEmpresas();
            return View(empresas);
        }
        [HttpPost]
        public IActionResult InsertarTabla(string nombreCargo, string descripcionCargo, string id_empresa)
        {
            try
            {
                var insertarCargo = _usuarioService.InsertarCargos(nombreCargo, descripcionCargo, id_empresa);
                return RedirectToAction("Cargos");
            }
            catch (Exception ex)
            {
                // Manejar la excepción, por ejemplo, podrías devolver una vista de error con un mensaje personalizado.
                return View("Error", ex.Message);
            }
        }
        public IActionResult Empresas()
        {
            var empresas = _usuarioService.ObtenerEmpresas();
            return View(empresas);
        }
        public IActionResult FormEmpresas()
        {
            return View();
        }
        [HttpPost]
        public IActionResult InsertarEmpresa(string nit, string nombreEmpresa, string pais, string calle, string carrera, string ciudad, string departamento, string indicativo, string numero)
        {
            try
            {
                var insertarEmpresa = _usuarioService.InsertarEmpresa(nit, nombreEmpresa, pais, calle, carrera, ciudad, departamento, indicativo, numero);
                return RedirectToAction("Empresas");
            }
            catch (Exception ex)
            {
                // Manejar la excepción, por ejemplo, podrías devolver una vista de error con un mensaje personalizado.
                return View("Error", ex.Message);
            }
        }
        public IActionResult Sedes()
        {
            var empresas = _usuarioService.ObtenerSedes();
            return View(empresas);
        }
        public IActionResult FormSedes()
        {
            var empresas = _usuarioService.ObtenerEmpresas();
            return View(empresas);
        }
        [HttpPost]
        public IActionResult InsertarSedes(string id_empresa, string nombreSede, string ciudad, string direccion, string telefono)
        {
            try
            {
                var insertarSede = _usuarioService.InsertarSede(id_empresa, nombreSede, ciudad, direccion, telefono);
                return RedirectToAction("Sedes");
            }
            catch (Exception ex)
            {
                // Manejar la excepción, por ejemplo, podrías devolver una vista de error con un mensaje personalizado.
                return View("Error", ex.Message);
            }
        }
        public IActionResult EmpleadoEmpresa()
        {
            var empresas = _usuarioService.ObtenerEmpresas();
            return View(empresas);
        }
        [HttpGet]
        public IActionResult FormEmpleadoEmpresa(string id_empresa)
        {
            var empleado = _usuarioService.ObtenerUsuarios();
            ViewBag.id_empresa = id_empresa;
            return View(empleado);
        }
        [HttpPost]
        public IActionResult InsertarEE(string idEmpresa, int cedula)
        {
            try
            {
                var insertarEmpleadoEmpresa = _usuarioService.InsertarEmpleadoEmpresa(idEmpresa, cedula);
                return RedirectToAction("EmpleadoEmpresa");
            }
            catch (Exception ex)
            {
                // Manejar la excepción, por ejemplo, podrías devolver una vista de error con un mensaje personalizado.
                return View("Error", ex.Message);
            }
        }

    }
}

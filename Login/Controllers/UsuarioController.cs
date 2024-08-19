using Login.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
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
            if(_usuarioService.validarEmpleado(cedula))
            {
                var mensaje = "Error: Ya existe un empleado con la misma cédula";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }else
            {
                _usuarioService.RegistrarEmpleado(cedula, nombre, apellido, genero, correo, rh, celular, contrasena);
                return RedirectToAction("RegistrarEmpleado");
            }
        }
        public IActionResult Editar(int id)
        {
            var usuarioEncontrado = _usuarioService.BuscarUsuario(id);
            if (usuarioEncontrado.Any())
            {
                return View(usuarioEncontrado);
            }
            else
            {
                var mensaje = "Error: No hay productos con ese codigo referenciado";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }
        }
        public IActionResult FormEmpleadoCompania(int cedula)
        {
            var empresaEncontrada = _usuarioService.ObtenerEmpresas();
            if (empresaEncontrada.Any())
            {
                ViewBag.cedula = cedula;
                return View(empresaEncontrada);
            }
            else
            {
                var mensaje = "Error: No hay empresas anexadas al sistema";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }
        }
        [HttpPost]
        public IActionResult EditarEmpleado(int cedula, string nombre, string apellido, string genero, string correo, string rh, string celular, string contrasena)
        {
            if (cedula < 0 || nombre == null || apellido == null || genero == null || correo == null || rh == null || celular == null || contrasena == null)
            {
                var mensaje = "Error: Todos los campos deben tener un valor. No se permiten valores nulos.";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }else
            {
                _usuarioService.EditarEmpleado(cedula, nombre, apellido, genero, correo, rh, celular, contrasena);
                return RedirectToAction("Index");
            }
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
                _usuarioService.InsertarCargos(nombreCargo, descripcionCargo, id_empresa);
                return RedirectToAction("Cargos");
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
                _usuarioService.InsertarEmpresa(nit, nombreEmpresa, pais, calle, carrera, ciudad, departamento, indicativo, numero);
                return RedirectToAction("Empresas");
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
                var insertarSede = _usuarioService.InsertarSede(id_empresa, nombreSede, ciudad, direccion, telefono);
                return RedirectToAction("Sedes");
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
        public IActionResult InsertarEE(string id_empresa, int cedula)
        {
                _usuarioService.InsertarEmpleadoEmpresa(id_empresa, cedula);
                return RedirectToAction("Index");
        }
        public IActionResult EmpleadoSedes()
        {
            var empresaSedeViewModel = _usuarioService.EmpleadoSede();
            return View(empresaSedeViewModel);
        }
        public IActionResult ValidarCedula(int cedula)
        {
            var empleado = _usuarioService.ValidarCedula(cedula);
            if (empleado != null)
            {
                //Dato no existe
                string? idEmpresa = _usuarioService.ObtenerIdEmpresa(cedula);
                if (string.IsNullOrEmpty(idEmpresa))
                {
                    var mensaje = "Error: La cedula " + cedula + " No esta sincronizado con una empresa";
                    TempData["ErrorMessage"] = mensaje;
                    return RedirectToAction("Error", "Errores");
                }else
                {
                    var sede = _usuarioService.ObtenerSedePorEmpleado(cedula);
                    var sedes = _usuarioService.ObtenerSedes(idEmpresa);
                    var cargos = _usuarioService.ObtenerCargos(idEmpresa);

                    if (sede == true)
                    {
                        ViewBag.Cedula = cedula;
                        ViewBag.IdEmpresa = idEmpresa;
                        ViewBag.Sedes = sedes;
                        ViewBag.Cargos = cargos;
                        return View("SeleccionarSedeYCargo");
                    }
                    else
                    {
                        var mensaje = "Error: La cedula ya tiene sede asignada.";
                        TempData["ErrorMessage"] = mensaje;
                        return RedirectToAction("Error", "Errores");
                    }
                }

            }
            else
            {
                var mensaje = "Error: No existe el empleado";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }
        }
        [HttpPost]
        public IActionResult GuardarSedeYCargo(int cedula, int idSede, int idCargo)
        {
            _usuarioService.InsertarSedeEmpleado(cedula, idSede, idCargo);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult GetSedes(string empresaId)
        {
            var sedes = _usuarioService.GetSedesByEmpresaId(empresaId);
            return Json(sedes);
        }
    }
}

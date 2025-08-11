using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
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
        public async Task<IActionResult> Index()
        {
            var empleado = await _usuarioService.ObtenerUsuarios();
            return View(empleado);
        }
        public IActionResult _AgregarEmpleado()
        {
            return PartialView(new Empleados());
        }

        [HttpPost]
        public async Task<IActionResult> AgregarEmpleado(Empleados emp)
        {
            await _usuarioService.InsertarEmpleado(emp);
            return Ok();
        }

        public async Task<IActionResult> _VerEmpleado(int cedula)
        {
            var empleado = await _usuarioService.ObtenerPorCedula(cedula);
            return PartialView(empleado);
        }

        public async Task<IActionResult> _EditarEmpleado(int cedula)
        {
            var empleado = await _usuarioService.ObtenerPorCedula(cedula);
            return PartialView(empleado);
        }

        [HttpPost]
        public async Task<IActionResult> EditarEmpleado(Empleados emp)
        {
            await _usuarioService.ActualizarEmpleado(emp);
            return Ok();
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
        [Authorize]
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
                var insertCargo = _usuarioService.ValidarCargo(nombreCargo);
                if(insertCargo != null)
                {
                    var mensaje = "Error: Ya existe este cargo.";
                    TempData["ErrorMessage"] = mensaje;
                    return RedirectToAction("Error", "Errores");
                }else
                {
                    _usuarioService.InsertarCargos(nombreCargo, descripcionCargo, id_empresa);
                    return RedirectToAction("Cargos");
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
            var verificarExisEmpresa = _usuarioService.ValidarExistenciaEmpresa(nit);
            if(verificarExisEmpresa != null)
            {
                var mensaje = "Error: Ya existe esta empresa.";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }else
            {
                _usuarioService.InsertarEmpresa(nit, nombreEmpresa, pais, calle, carrera, ciudad, departamento, indicativo, numero);
                return RedirectToAction("Empresas");
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
            var verificarExisSede = _usuarioService.ValidarExistenciaSede(nombreSede);
            if(verificarExisSede != null)
            {
                var mensaje = "Error: Ya existe esta sede.";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }else
            {
                var insertarSede = _usuarioService.InsertarSede(id_empresa, nombreSede, ciudad, direccion, telefono);
                return RedirectToAction("Sedes");
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
        public IActionResult InsertarEE(string id_empresa, int cedula)
        {
            var ValidarExisEmpleadoEmpresa = _usuarioService.ValidarExisEmpleadoEmpresa(id_empresa, cedula);
            if(ValidarExisEmpleadoEmpresa != null)
            {
                var mensaje = "Error: Ya existe la relación entre este empleado y esta empresa.";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }
            else
            {
                _usuarioService.InsertarEmpleadoEmpresa(id_empresa, cedula);
                return RedirectToAction("Index");
            }
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
        public IActionResult FormCrearCliente() 
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddCliente(int cedulaCliente, string nombreCliente, string empresaCliente, string ciudadCliente, string telefonoCliente, string correoCliente, string direccionCliente)
        {
            var validacionInserClient = _usuarioService.InsertAddClient(cedulaCliente, nombreCliente, empresaCliente, ciudadCliente, telefonoCliente, correoCliente, direccionCliente);
            if(validacionInserClient)
            {
                return RedirectToAction("FormCrearCliente");
            }
            else
            {
                var mensaje = "Error: Se presento problemas con la informacion suministrada.";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }
        }
        public IActionResult FormCrearProveedor()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddProveedor(string nit, string razonSocial, string direccion, string celular, string correo)
        {
            var validacionInserProveedor = _usuarioService.InsertAddProveedor(nit, razonSocial, direccion, celular, correo);
            if (validacionInserProveedor)
            {
                return RedirectToAction("FormCrearProveedor");
            }
            else
            {
                var mensaje = "Error: Se presento problemas con la informacion suministrada.";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }
        }
        public IActionResult TblVisuCliente()
        {
            var visualizarClientes = _usuarioService.ServVisuaCliente();
            if(visualizarClientes == null || !visualizarClientes.Any())
            {
                var mensaje = "Error: No se encontraron clientes";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }
            return View(visualizarClientes);
        }
        public IActionResult TblVisuProveedor()
        {
            var visualizarClientes = _usuarioService.ServVisuaProveedor();
            if (visualizarClientes == null || !visualizarClientes.Any())
            {
                var mensaje = "Error: No se encontraron clientes";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }
            return View(visualizarClientes);
        }
        public IActionResult CrearPDV()
        {
            var traerSedes = _usuarioService.ObtenerSedes();
            return View(traerSedes);
        }
        [HttpPost]
        public async Task<IActionResult> Crear(string nombrePDV, int idSede)
        {
            if (string.IsNullOrWhiteSpace(nombrePDV) || idSede == 0)
            {
                TempData["Error"] = "Todos los campos son obligatorios.";
                var sedes = _usuarioService.ObtenerSedes();
                return View("CrearPuntoVenta", sedes);
            }

            var nuevoPDV = new Infopdv
            {
                NombreInfoPDV = nombrePDV,
                Id_Sede = idSede
            };

            var resultado = await _usuarioService.CrearPuntoVentaAsync(nuevoPDV);

            if (resultado)
            {
                TempData["Success"] = "Punto de Venta creado correctamente.";
                return RedirectToAction("CrearPDV");
            }
            else
            {
                TempData["Error"] = "Error al guardar el Punto de Venta.";
                var sedes = _usuarioService.ObtenerSedes();
                return View("CrearPDV", sedes);
            }
        }
    }
}

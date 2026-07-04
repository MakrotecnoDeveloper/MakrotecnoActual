using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Models.ViewModels.Usuario;
using Plataforma.Servicios.Contrato;
using Plataforma.Servicios.Implementacion;

namespace Plataforma.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly BaseAdmContext _dbContext;
        private readonly IUsuarioService _usuarioService;
        public UsuarioController(IUsuarioService usuarioService, BaseAdmContext dbContext)
        {
            _usuarioService = usuarioService;
            _dbContext = dbContext;
        }
        [Authorize]
        [HttpGet]
        //Vista para visualizar los empleados
        public async Task<IActionResult> Index()
        {
            var empleado = await _usuarioService.ObtenerUsuarios();
            return View(empleado);
        }
        [HttpGet]
        public IActionResult _AgregarEmpleado()
        {
            return PartialView();
        }
        [Authorize]
        [HttpPost]
        //POST para agregar empleado
        public async Task<IActionResult> AgregarEmpleado(Empleados emp)
        {
            await _usuarioService.InsertarEmpleado(emp);
            return Ok();
        }
        [Authorize]
        [HttpGet]
        //Vista parcial para visualizar la informacion del empleado
        public async Task<IActionResult> _VerEmpleado(int cedula)
        {
            var empleado = await _usuarioService.ObtenerPorCedula(cedula);
            return PartialView(empleado);
        }
        [Authorize]
        [HttpGet]
        //Vista parcial para editar un empleado
        public async Task<IActionResult> _EditarEmpleado(int cedula)
        {
            var empleado = await _usuarioService.ObtenerPorCedula(cedula);
            return PartialView(empleado);
        }
        [Authorize]
        [HttpPost]
        //Vista para editar empleado.
        public async Task<IActionResult> EditarEmpleado(Empleados emp)
        {
            await _usuarioService.ActualizarEmpleado(emp);
            return Ok();
        }
        [Authorize]
        [HttpGet]
        //Vista donde se visualiza los cargos
        public IActionResult Cargos()
        {
            var cargos = _usuarioService.ObtenerCargos();
            return View(cargos);
        }
        [Authorize]
        [HttpGet]
        //Vista donde se visualiza el formulario de cargos
        public IActionResult FormCargos()
        {
            var empresas = _usuarioService.ObtenerEmpresas();
            return View(empresas);
        }
        [Authorize]
        [HttpPost]
        //POST para agregar los cargos.
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
        [Authorize]
        [HttpGet]
        //Vista para visualizar las empresas
        public IActionResult Empresas()
        {
            var empresas = _usuarioService.ObtenerEmpresas();
            return View(empresas);
        }
        [Authorize]
        [HttpGet]
        public IActionResult FormEmpresas()
        {
            var actividades = _usuarioService.ObtenerActividadesEconomicas();
            ViewBag.Actividades = actividades;

            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult InsertarEmpresa(
            string nit,
            string nombreEmpresa,
            string pais,
            string calle,
            string carrera,
            string ciudad,
            string departamento,
            string indicativo,
            string numero,
            int actividadEconomicaId
        )
        {
            var verificarExisEmpresa = _usuarioService.ValidarExistenciaEmpresa(nit);

            if (verificarExisEmpresa != null)
            {
                TempData["ErrorMessage"] = "Error: Ya existe esta empresa.";
                return RedirectToAction("Error", "Errores");
            }
            else
            {
                string estado = "Activo";

                _usuarioService.InsertarEmpresa(
                    nit,
                    nombreEmpresa,
                    pais,
                    calle,
                    carrera,
                    ciudad,
                    departamento,
                    indicativo,
                    numero,
                    estado,
                    actividadEconomicaId
                );

                return RedirectToAction("Empresas");
            }
        }
        [Authorize]
        [HttpGet]
        //Vista para visualizar las sedes de una empresa
        public IActionResult Sedes()
        {
            var empresas = _usuarioService.ObtenerSedes();
            return View(empresas);
        }
        [Authorize]
        [HttpGet]
        //Vista-Formulario para agregar una sede
        public IActionResult FormSedes()
        {
            var empresas = _usuarioService.ObtenerEmpresas();
            return View(empresas);
        }
        [Authorize]
        [HttpPost]
        //POST para insertar una sede
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
        [Authorize]
        [HttpGet]
        //Vista para visualizar las empresas y agregar empleado a dichas empresas
        public IActionResult EmpleadoEmpresa()
        {
            var empresas = _usuarioService.ObtenerEmpresas();
            return View(empresas);
        }
        [Authorize]
        [HttpGet]
        //Vista-Formulario para hacer sincronizacion entre empresa-empleado
        public async Task<IActionResult> FormEmpleadoEmpresa(string id_empresa)
        {
            var empleado = await _usuarioService.ObtenerUsuarios();
            ViewBag.id_empresa = id_empresa;
            return View(empleado);
        }
        [Authorize]
        [HttpPost]
        //POST para insertar la sincronizacion entre empresa-empleado
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
        [Authorize]
        [HttpGet]
        //Vista para ver los usuarios con sedes y validar una cedula si esta en una sede o no
        public IActionResult EmpleadoSedes()
        {
            var empresaSedeViewModel = _usuarioService.EmpleadoSede();
            return View(empresaSedeViewModel);
        }
        [HttpPost]
        public IActionResult ValidarCedula(int cedula)
        {
            if (cedula <= 0)
            {
                TempData["ErrorMessage"] = "Debe ingresar una cédula válida.";
                return RedirectToAction("Error", "Errores");
            }

            // 1. Validar si el usuario ya tiene sede/cargo configurado
            /*var existeSedeEmpleado = _dbContext.Sedeempleado
                .Any(x => x.Cedula == cedula);

            if (existeSedeEmpleado)
            {
                // Aquí dejas tu flujo actual cuando sí existe.
                // Por ejemplo:
                return RedirectToAction("Index");
            }*/

            // 2. Buscar las empresas asociadas al empleado en empleadoempresa
            var empresasEmpleado = _dbContext.EmpleadoEmpresa
                .Where(x => x.Cedula == cedula && x.Id_empresa != null)
                .Select(x => x.Id_empresa!)
                .Distinct()
                .ToList();

            if (!empresasEmpleado.Any())
            {
                TempData["ErrorMessage"] = "El empleado no tiene empresas asociadas.";
                return RedirectToAction("Error", "Errores");
            }

            // 3. Traer sedes de esas empresas
            var sedes = (from s in _dbContext.Sede
                         join e in _dbContext.Empresas
                            on s.Id_empresa equals e.Id_empresa
                         where s.Id_empresa != null
                            && empresasEmpleado.Contains(s.Id_empresa)
                         select new SedeEmpresaVm
                         {
                             IdSede = s.Id_sede,
                             NombreSede = s.NombreSede ?? "",
                             IdEmpresa = s.Id_empresa ?? "",
                             NombreEmpresa = e.Nombre ?? ""
                         })
                .ToList();

            // 4. Traer cargos de esas empresas
            var cargos = _dbContext.TipoCargo
                .Where(c => c.Id_empresa != null && empresasEmpleado.Contains(c.Id_empresa))
                .Select(c => new CargoEmpresaVm
                {
                    IdCargo = c.Id_tipo,
                    NombreCargo = c.NombreCargo ?? "",
                    DescripcionCargo = c.DescripcionCargo ?? "",
                    IdEmpresa = c.Id_empresa ?? ""
                })
                .ToList();

            if (!sedes.Any())
            {
                TempData["ErrorMessage"] = "No hay sedes asociadas a las empresas del empleado.";
                return RedirectToAction("Error", "Errores");
            }

            if (!cargos.Any())
            {
                TempData["ErrorMessage"] = "No hay cargos asociados a las empresas del empleado.";
                return RedirectToAction("Error", "Errores");
            }

            // 5. Enviar todo a la vista
            var vm = new SeleccionarSedeYCargoVm
            {
                Cedula = cedula,
                Sedes = sedes,
                Cargos = cargos
            };

            return View("SeleccionarSedeYCargo", vm);
        }
        [Authorize]
        [HttpPost]
        public IActionResult GuardarSedeYCargo(int Cedula, int IdSede, int IdCargo)
        {
            if (Cedula <= 0 || IdSede <= 0 || IdCargo <= 0)
            {
                TempData["ErrorMessage"] = "Debe seleccionar una sede y un cargo válidos.";
                return RedirectToAction("Error", "Errores");
            }

            // 1. Validar que la sede exista
            var sede = _dbContext.Sede
                .FirstOrDefault(s => s.Id_sede == IdSede);

            if (sede == null)
            {
                TempData["ErrorMessage"] = "La sede seleccionada no existe.";
                return RedirectToAction("Error", "Errores");
            }

            // 2. Validar que el cargo exista
            var cargo = _dbContext.TipoCargo
                .FirstOrDefault(c => c.Id_tipo == IdCargo);

            if (cargo == null)
            {
                TempData["ErrorMessage"] = "El cargo seleccionado no existe.";
                return RedirectToAction("Error", "Errores");
            }

            // 3. Validar que el cargo pertenezca a la misma empresa de la sede
            if (!string.Equals(cargo.Id_empresa, sede.Id_empresa, StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "El cargo seleccionado no pertenece a la empresa de la sede.";
                return RedirectToAction("Error", "Errores");
            }

            // 4. Validar que el empleado sí tenga esa empresa asociada en empleadoempresa
            var empleadoTieneEmpresa = _dbContext.EmpleadoEmpresa.Any(x =>
                x.Cedula == Cedula &&
                x.Id_empresa == sede.Id_empresa);

            if (!empleadoTieneEmpresa)
            {
                TempData["ErrorMessage"] = "El empleado no tiene asociada la empresa de la sede seleccionada.";
                return RedirectToAction("Error", "Errores");
            }

            // 5. Evitar duplicados en sedeempleado
            var yaExiste = _dbContext.Sedeempleado.Any(x =>
                x.Cedula == Cedula &&
                x.Id_sede == IdSede);

            if (yaExiste)
            {
                TempData["ErrorMessage"] = "El empleado ya tiene esta sede configurada.";
                return RedirectToAction("Error", "Errores");
            }

            // 6. Guardar sede y cargo
            var sedeEmpleado = new Sedeempleado
            {
                Cedula = Cedula,
                Id_sede = IdSede,
                Id_cargo = IdCargo
            };

            _dbContext.Sedeempleado.Add(sedeEmpleado);
            _dbContext.SaveChanges();

            TempData["SuccessMessage"] = "Sede y cargo asignados correctamente.";

            return RedirectToAction("Index");
        }
        [Authorize]
        [HttpGet]
        //Vista-Formulario para insertar un cliente
        public IActionResult FormCrearCliente() 
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        //POST para insertar un cliente
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
        [Authorize]
        [HttpGet]
        //Vista-Formulario para insertar un proveedor
        public IActionResult FormCrearProveedor()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        //POST para insertar un proveedor
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
        [Authorize]
        [HttpGet]
        //Vista para ver los clientes.
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
        [Authorize]
        [HttpGet]
        //Vista para ver los proveedores.
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
        [Authorize]
        [HttpGet]
        //Vista-Formulario para actualizar un proveedor
        public async Task<IActionResult> ModProveedor(int id)
        {
            var proveedor = await _usuarioService.ObtenerPorIdAsyncProveedor(id);
            if (proveedor == null)
                return NotFound();

            return PartialView("_EditarProveedor", proveedor);
        }
        [Authorize]
        [HttpPost]
        //POST para actualizar el proveedor
        public async Task<IActionResult> EditarProveedor(Proveedores model)
        {
            await _usuarioService.ActualizarProveedorAsync(model);
            return Ok();
        }
        [Authorize]
        [HttpGet]
        //Vista para registrar punto de venta y sincronizar empleado-punto de venta.
        public IActionResult CrearPDV()
        {
            var traerSedes = _usuarioService.ObtenerSedes();
            return View(traerSedes);
        }
        [Authorize]
        [HttpGet]
        //Vista-Formulario para generar la sincronizacion entre empleado-pdv
        public async Task<IActionResult> _AsignarUserPDV()
        {
            var cedula = User.Claims.FirstOrDefault(c => c.Type == "Cedula")?.Value;

            if (string.IsNullOrEmpty(cedula))
                return BadRequest("Usuario logueado no tiene cédula en los claims.");

            var vm = await _usuarioService.ObtenerDatosAsignacion(cedula);

            return PartialView("_AsignarUserPDV", vm);
        }
        [Authorize]
        [HttpPost]
        //POST para insertar la sincronizacion entre empleado-pdv
        public async Task<IActionResult> AsignarEmpleadoAPDV(string cedulaEmpleado, int idPdv)
        {
            if (string.IsNullOrEmpty(cedulaEmpleado) || idPdv == 0)
                return BadRequest("Datos incompletos.");

            var asignacion = new Syncpdv
            {
                InfopdvId = idPdv,
                Estado = 1,
                FechaEstado = DateTime.Now,
                Cedula = int.Parse(cedulaEmpleado)
            };

            _dbContext.Syncpdv.Add(asignacion);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("CrearPDV");
        }
        [Authorize]
        [HttpPost]
        //POST para insertar una pdv
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
        [Authorize]
        [HttpGet]
        //Vista-Formulario para modificar un cliente
        public async Task<IActionResult> ModCliente(int id)
        {
            var cliente = await _usuarioService.ObtenerPorIdAsync(id);
            if (cliente == null)
                return NotFound();

            return PartialView("_EditarCliente", cliente);
        }
        [Authorize]
        [HttpPost]
        //POST para modificar un cliente
        public async Task<IActionResult> EditarCliente(Clientes model)
        {
            /*if (!ModelState.IsValid)
                return BadRequest(ModelState);*/

            await _usuarioService.ActualizarClienteAsync(model);
            return Ok();
        }
    }
}

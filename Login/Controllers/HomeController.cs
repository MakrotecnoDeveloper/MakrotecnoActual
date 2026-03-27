using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using Plataforma.Servicios.Implementacion;
using System.Net.Mail;

using System.Security.Claims;

namespace Plataforma.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IProductoService _productoservice;
        public HomeController(IUsuarioService usuarioService, IProductoService productoservice)
        {
            _usuarioService = usuarioService;
            _productoservice = productoservice;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            var TraerServicios = _usuarioService.ServTraerServicios();
            return View(TraerServicios);
            //return View("Belleza/Index");
        }
        [AllowAnonymous]
        public IActionResult Nosotros()
        {
            //return View();
            return View("Belleza/Nosotros");
        }
        [AllowAnonymous]
        public async Task<IActionResult> Agendar()
        {
            //return View();
            return View("Belleza/Agendar");
        }
        [AllowAnonymous]
        public IActionResult Servicios()
        {
            //return View();
            return View("Belleza/Servicios");
        }
        [AllowAnonymous]
        public IActionResult Contacto()
        {
            //return View();
            return View("Belleza/Contacto");
        }
        [AllowAnonymous]
        public IActionResult Portal()
        {
            return View();
        }
        [AllowAnonymous]
        public async Task<IActionResult> PreAgendamiento(int id)
        {
            var productosTraidosXBD = await _usuarioService.ConsultarCatProductos(id);
            //1. Mandar el ID al modelo, y que procese: 
            return View("Belleza/PreAgendamiento", productosTraidosXBD);
        }
        [AllowAnonymous]
        public IActionResult PoliticasPrivacidad()
        {
            //return View();
            return View("Belleza/PoliticasPrivacidad");
        }
        [AllowAnonymous]
        public IActionResult ServicioDomicilio()
        {
            //return View();
            return View("Belleza/ServicioDomicilio");
        }
        [AllowAnonymous]
        public IActionResult FAQS()
        {
            //return View();
            return View("Belleza/FAQS");
        }
        [AllowAnonymous]
        public IActionResult Empleos()
        {
            //return View();
            return View("Belleza/Empleos");
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
		public IActionResult ValidarPDV(int cedula, string password)
		{
            if(string.IsNullOrWhiteSpace(password) || cedula <= 0) 
            {
                var mensaje = "Error: No ingreso usuario y/o contraseña, revisar porfavor. (SGE)";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }
            var validarUsuario = _usuarioService.GetUsuarios(cedula, password);
            if (validarUsuario != null)
            {
                var validarDispoSede = _usuarioService.ServValidarDisponSede(cedula);
                if (!validarDispoSede)
                {
                    var mensaje = "Error: Empleado no se encuentra configurado.";
                    TempData["ErrorMessage"] = mensaje;
                    return RedirectToAction("Error", "Errores");
                }else
                {
                    var varValidarPDV = _usuarioService.FunValidarPDV(cedula);
                    if(varValidarPDV.Any())
                    {
                        ViewBag.Cedula = cedula;
                        ViewBag.Password = password;

                        return View(varValidarPDV);
                    }else
                    {
                        var mensaje = "Error: No hay puntos de venta configurados con este empleado.";
                        TempData["ErrorMessage"] = mensaje;
                        return RedirectToAction("Error", "Errores");
                    }

                    
                }
            }else
            {
                var mensaje = "Error: Correo o Contraseña no existe, validar informacion nuevamente.";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }
                

		}
        [HttpPost]
        public IActionResult ValidacionLogin(int cedula, string password, int selectedPDV)
        {
                var validarUsuario = _usuarioService.GetUsuarios(cedula, password);
				var claims = new List<Claim>() {
					new Claim("Cedula", validarUsuario.Cedula.ToString()),
					new Claim("Nombre", validarUsuario.Nombre),
					new Claim("Apellido", validarUsuario.Apellido),
					new Claim("Genero", validarUsuario.Genero),
					new Claim("Correo", validarUsuario.Correo),
					new Claim("RH", validarUsuario.Rh),
					new Claim("Celular", validarUsuario.Celular),
					new Claim("Contrasena", validarUsuario.Contrasena),
					};
				int rolEmpleado = _usuarioService.ObtenerRolPermisos(validarUsuario.Cedula);
				if (rolEmpleado > 0)
				{
					claims.Add(new Claim("Rol", rolEmpleado.ToString()));
					var nombreCargo = _usuarioService.ObtenerNombreRolPermisos(rolEmpleado);
					if (nombreCargo != null)
					{
                        claims.Add(new Claim("NombreRol", nombreCargo.NombreCargo));
                        claims.Add(new Claim("CargoId",   nombreCargo.TipoCargo.ToString()));
                        claims.Add(new Claim("EmpresaId", nombreCargo.IdEmpresa.ToString()));
                        claims.Add(new Claim("NombreEmpresa", nombreCargo.NombreEmpresa));
                }
					else
					{
						var mensaje = "Error: El nombre del cargo no esta asignado desde el Sistema Gestor de Empleados (SGE)";
						TempData["ErrorMessage"] = mensaje;
						return RedirectToAction("Error", "Errores");
					}
				}
				else
				{
					var mensaje = "Error: No tiene un cargo (ID) asignado en el Sistema Gestor de Empleados (SGE)";
					TempData["ErrorMessage"] = mensaje;
					return RedirectToAction("Error", "Errores");
				}
				var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
				var principal = new ClaimsPrincipal(identity);

				HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
					new AuthenticationProperties()
					{
						ExpiresUtc = DateTime.UtcNow.AddMinutes(60),
						AllowRefresh = true,
						IsPersistent = false
					});
				int cedulaEmpleado = validarUsuario.Cedula;
				string correoEmpleado = validarUsuario.Correo;
				int estado = 1;
                var varNombrePDV = _usuarioService.SeleccionarNombrePDV(selectedPDV);
                int idPDV = varNombrePDV.InfopdvId;
                _usuarioService.InsertarLogLogin(cedulaEmpleado, correoEmpleado, estado, idPDV);
                return RedirectToAction("Index", "Inicio");
		}
		[Authorize]
		public IActionResult Logout()
		{
            var cedula = User.FindFirst("Cedula")?.Value;
            var correoEmpleado = User.FindFirst("Correo")?.Value;
            int estado = 0;
            if (int.TryParse(cedula, out int cedulaEmpleado) && !string.IsNullOrEmpty(correoEmpleado))
            {
                var idPDV = _usuarioService.TraerUltimoIDPdv(cedulaEmpleado);
                if(idPDV <= 0)
                {
                    var mensaje = "Error: No hay inicio de sesion para esta PDV";
                    TempData["ErrorMessage"] = mensaje;
                    return RedirectToAction("Error", "Errores");
                }else
                {
                    //Console.WriteLine("Se va a ingresar el cierre de sesion");
                    _usuarioService.InsertarLogLogin(cedulaEmpleado, correoEmpleado, estado, idPDV);
                }
            }
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			return RedirectToAction("Login", "Home");
		}
        [AllowAnonymous]
        public IActionResult StocksTienda()
        {
            var traerProductosAbarrotes = _usuarioService.ProductosAbarrotes();
            return View(traerProductosAbarrotes);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ModificarProInventario(string id, string campo, string newVal)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(campo) || string.IsNullOrEmpty(newVal))
            {
                return BadRequest("Parámetros inválidos.");
            }

            try
            {
                
                bool resultado = await _usuarioService.ActualizarProductoAsync(id, campo, newVal);

                if (resultado)
                {
                    return Ok("Actualización exitosa.");
                }
                else
                {
                    return StatusCode(500, "Error al actualizar el producto.");
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> InsertarProInventario(string codigoProducto, string nombreProducto, int cantidadProducto, decimal? valorNetoProductoFloat, decimal? valorVentaProductoFloat, int valorUnidadInt, string id_empresa, int categoria, int estado, string ubicacion, int IdProveedor)
        {
            try
            {

                bool resultado = await _usuarioService.InsertProInventario(codigoProducto, nombreProducto, cantidadProducto, valorNetoProductoFloat, valorVentaProductoFloat, valorUnidadInt, id_empresa, categoria, estado, ubicacion, IdProveedor);

                if (resultado)
                {
                    return Ok("Inserccion exitosa.");
                }
                else
                {
                    return StatusCode(500, "Error al insertar el producto.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> EliminarProductoXID(string id)
        {
            try
            {
                bool resultado = await _usuarioService.EliminarProductoXIdAsync(id);

                if (resultado)
                {
                    return Ok("Producto inhabilitado de manera correcta.");
                }
                else
                {
                    return StatusCode(500, "Error al inhabilitar el producto.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [AllowAnonymous]
        [HttpGet("Home/Tienda/{idServicio}/{sedeId?}")]
        public IActionResult Tienda(int idServicio, int? sedeId)
        {
            var categorias = _productoservice.ObtenerCategoriaProductos(idServicio);
            var sedes = _productoservice.ObtenerSedes();

            int sedeFinal = sedeId ?? 0;
            List<ProductoTiendaDTO> productos = new();

            if (sedeFinal == 0)
            {
                foreach (var sede in sedes)
                {
                    var productosTmp = _productoservice.ObtenerProductosPorServicioYSede(idServicio, sede.Id_sede);
                    if (productosTmp != null && productosTmp.Count > 0)
                    {
                        sedeFinal = sede.Id_sede;
                        productos = productosTmp;
                        break;
                    }
                }
            }
            else
            {
                productos = _productoservice.ObtenerProductosPorServicioYSede(idServicio, sedeFinal) ?? new List<ProductoTiendaDTO>();
            }

            var vm = new ProductosCategoriaViewModel
            {
                CategoriaProductos = categorias ?? new(),
                Productos = productos ?? new(),
                Servicio = idServicio,
                Sedes = sedes ?? new(),
                SedeSeleccionada = sedeFinal
            };

            return View(vm);
        }
        [AllowAnonymous]
        [HttpGet("/producto/ver/{id}", Name = "ProductoPublico")]
        public IActionResult VisualizarProducto(string id)
        {
            var vm = _productoservice.ObtenerProductoPublico(id);

            if (vm == null)
                return NotFound();

            return View(vm);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SendContactMessage(ContactFormModel model)
        {
                // Procesar el archivo adjunto
                string attachmentPath = null;
                if (model.Attachment != null)
                {
                    string uploadsFolder = Path.Combine("wwwroot", "uploads");
                    Directory.CreateDirectory(uploadsFolder);
                    attachmentPath = Path.Combine(uploadsFolder, Path.GetFileName(model.Attachment.FileName));

                    using (var stream = new FileStream(attachmentPath, FileMode.Create))
                    {
                        await model.Attachment.CopyToAsync(stream);
                    }
                }

                // Enviar correo
                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress("pruebasp714@gmail.com");
                    mailMessage.To.Add("makrotecnocomercio@gmail.com");
                    mailMessage.Subject = "Nuevo mensaje de contacto";
                    mailMessage.Body = $"Nombre: {model.FirstName} {model.LastName}\nCorreo: {model.Email}\nMensaje:\n{model.Message}";
                    if (attachmentPath != null)
                    {
                        if(model.Attachment.Length > 5 * 1024 * 1024) 
                        {
                            var mensaje = "El archivo no debe superar los 5 MB";
                            TempData["ErrorMessage"] = mensaje;
                            return RedirectToAction("Error", "Errores");
                        }
                        else
                        {
                            string fileExtension = Path.GetExtension(model.Attachment.FileName).ToLowerInvariant();
                            if(fileExtension != ".pdf") 
                            {
                                var mensaje = "Solo se permiten archivos PDF";
                                TempData["ErrorMessage"] = mensaje;
                                return RedirectToAction("Error", "Errores");
                            }else
                            {
                                using(var stream = new MemoryStream())
                                {
                                    await model.Attachment.CopyToAsync(stream);
                                    byte[] fileBytes = stream.ToArray();
                                    if(!(fileBytes.Length > 4 && fileBytes[0] == 0x25 && fileBytes[1] == 0x50 && fileBytes[2] == 0x44 && fileBytes[3] == 0x46))
                                    { 
                                        var mensaje = "El archivo no es un PDF Valido";
                                        TempData["ErrorMessage"] = mensaje;
                                        return RedirectToAction("Error", "Errores");
                                    }else
                                    {
                                        mailMessage.Attachments.Add(new Attachment(attachmentPath));
                                    }
                                }
                            }
                        }
                        
                    }

                    using (var smtpClient = new SmtpClient("smtp.gmail.com"))
                    {
                        smtpClient.Credentials = new System.Net.NetworkCredential("pruebasp714@gmail.com", "rhccjuoouoeobuac");
                        smtpClient.EnableSsl = true;
                        await smtpClient.SendMailAsync(mailMessage);
                    }
                }

                TempData["SuccessMessage"] = "¡Mensaje enviado correctamente!";
                return RedirectToAction("Index");
        }
        [Authorize]
        [HttpGet]
        public IActionResult HabilitarCategoriasWeb()
        {
            var modelo = _productoservice.ObtenerCategoriasWeb();
            return View(modelo);
        }
        [Authorize]
        [HttpPost]
        public IActionResult HabilitarCategoriasWeb(List<CategoriaWebEstadoViewModel> modelo)
        {
                _productoservice.ActualizarEstadoWebCategorias(modelo);
                TempData["Mensaje"] = "Configuración actualizada correctamente.";
                return RedirectToAction(nameof(HabilitarCategoriasWeb));
        }
        //Funcionalidades de belleza
        [Authorize]
        [HttpGet]
        public IActionResult ClientesAgendados()
        {
            var varClientesAgendados = _usuarioService.AgendamientosServicios();
            return View(varClientesAgendados);
        }
        [Authorize]
        [HttpPost]
        public IActionResult GuardarEdicion(Agendamientos model)
        {
            bool resultado = _usuarioService.GuardarEdicionServicio(model);

            if (resultado)
            {
                return Json(new { success = true, mensaje = "Servicio editado correctamente." });
            }
            else
            {
                return Json(new { success = false, mensaje = "No se pudo editar el servicio." });
            }
        }
        [Authorize]
        [HttpGet]
        public IActionResult EditarServicio(int id)
        {
            var servicio = _usuarioService.ObtenerAgendamientoPorId(id);
            if (servicio != null)
            {
                return PartialView("_EditarServicioPartial", servicio);
            }

            return NotFound();
        }
        [Authorize]
        [HttpPost]
        public IActionResult AprobarServicio(int id)
        {
            bool resultado = _usuarioService.AprobarAgendamiento(id);

            return Json(new
            {
                success = resultado,
                mensaje = resultado ? "Servicio aprobado correctamente." : "No se pudo aprobar el servicio."
            });
        }
        [Authorize]
        [HttpPost]
        public IActionResult EliminarServicio(int id)
        {
            bool resultado = _usuarioService.EliminarAgendamiento(id);

            return Json(new
            {
                success = resultado,
                mensaje = resultado ? "Servicio eliminado correctamente." : "No se pudo eliminar el servicio."
            });
        }
        [HttpPost]
        public async Task<IActionResult> ValidarFecha(string fecha, string codProducto, string nombreCliente, string celularCliente)
        {
            if (!DateTime.TryParse(fecha, out DateTime fechaSeleccionada))
            {
                return Json(new { success = false, mensaje = "Fecha inválida." });
            }

            if (string.IsNullOrEmpty(nombreCliente) || string.IsNullOrEmpty(celularCliente))
            {
                return Json(new { success = false, mensaje = "Debe ingresar Nombre y Celular del Cliente." });
            }

            bool existe = await _usuarioService.ExisteFechaAsync(codProducto, fechaSeleccionada);
            if (existe)
            {
                return Json(new { success = false, mensaje = "La fecha ya está ocupada, selecciona otra." });
            }

            bool registrado = await _usuarioService.RegistrarAgendamientoAsync(codProducto, fechaSeleccionada, nombreCliente, celularCliente);
            if (registrado)
            {
                return Json(new { success = true, mensaje = "Fecha agendada correctamente." });
            }
            else
            {
                return Json(new { success = false, mensaje = "Error al registrar la fecha." });
            }
        }


    }
}
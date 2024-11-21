using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Servicios.Contrato;

using System.Security.Claims;

namespace Plataforma.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        public HomeController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
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
                var varValidarPDV = _usuarioService.FunValidarPDV(cedula);

                ViewBag.Cedula = cedula;
                ViewBag.Password = password;

                return View(varValidarPDV);
            }else
            {
                var mensaje = "Error: Correo o Contraseña no existe, validar informacion nuevamente.";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }
                

		}
        [HttpPost]
        public IActionResult validacionLogin(int cedula, string password, int selectedPDV)
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
					string nombreCargo = _usuarioService.ObtenerNombreRolPermisos(rolEmpleado);
					if (nombreCargo != null)
					{
						claims.Add(new Claim("NombreRol", nombreCargo));
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
                    Console.WriteLine("Se va a ingresar el cierre de sesion");
                    _usuarioService.InsertarLogLogin(cedulaEmpleado, correoEmpleado, estado, idPDV);
                }
            }
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			return RedirectToAction("Login", "Home");
		}
        public IActionResult StocksTienda()
        {
            var traerProductosAbarrotes = _usuarioService.ProductosAbarrotes();
            return View(traerProductosAbarrotes);
        }
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
        [HttpPost]
        public async Task<IActionResult> InsertarProInventario(string nombreProducto, int cantidadProducto, float valorNetoProductoFloat, float valorVentaProductoFloat, int valorUnidadInt, string id_empresa, string categoria, int estado, string ubicacion)
        {
            try
            {

                bool resultado = await _usuarioService.InsertProInventario(nombreProducto, cantidadProducto, valorNetoProductoFloat, valorVentaProductoFloat, valorUnidadInt, id_empresa, categoria, estado, ubicacion);

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


    }
}
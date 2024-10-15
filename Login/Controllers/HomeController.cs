using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using System.Data;
using System.Security.Claims;

namespace Plataforma.Controllers
{
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
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
		public IActionResult ValidarPDV(int cedula, string password)
		{
            if(password == null || cedula < 0) 
            {
                var mensaje = "Error: No ingreso usuario y/o contraseña, revisar porfavor. (SGE)";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }
			var varValidarPDV = _usuarioService.funValidarPDV(cedula);

			// Almacenar cedula y password temporalmente
			ViewBag.Cedula = cedula;
			ViewBag.Password = password;

			return View(varValidarPDV); // Pasamos las PDV obtenidas a la vista
		}
		[HttpPost]
        public IActionResult validacionLogin(int cedula, string password, int selectedPDV)
        {
            var validarUsuario = _usuarioService.GetUsuarios(cedula, password);
            if (validarUsuario != null)
			{
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
                var varNombrePDV = _usuarioService.seleccionarNombrePDV(selectedPDV);
                // Asignar el nombre al ViewBag si el objeto no es nulo
                if (varNombrePDV != null)
                {
                    // Acceder a la propiedad 'Name' del objeto varNombrePDV
                    TempData["NombrePDV"] = varNombrePDV.Name;
                    TempData["IdPDV"] = varNombrePDV.Id;
                }
                else
                {
                    TempData["NombrePDV"] = "PDV no encontrada";
                }
                _usuarioService.InsertarLogLogin(cedulaEmpleado, correoEmpleado, estado);
                return RedirectToAction("Index", "Inicio");
			}
			else
			{
				var mensaje = "Error: Correo o Contraseña no existe, validar informacion nuevamente.";
				TempData["ErrorMessage"] = mensaje;
				return RedirectToAction("Error", "Errores");
			}
		}
		[Authorize]
		public async Task<IActionResult> Logout()
		{
            var cedula = User.FindFirst("Cedula")?.Value;
            var correoEmpleado = User.FindFirst("Correo")?.Value;
            int estado = 0;
            if (int.TryParse(cedula, out int cedulaEmpleado) && !string.IsNullOrEmpty(correoEmpleado))
            {
                var logins = await _usuarioService.InsertarLogLogin(cedulaEmpleado, correoEmpleado, estado);
            }
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			return RedirectToAction("Login", "Home");
		}
        public IActionResult InventarioTienda()
        {
            var traerProductosAbarrotes = _usuarioService.ProductosAbarrotes();
            return View(traerProductosAbarrotes);
        }
        [HttpPost]
        public async Task<IActionResult> modificarProInventario(string id, string campo, string newVal)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(campo) || string.IsNullOrEmpty(newVal))
            {
                return BadRequest("Parámetros inválidos.");
            }

            try
            {
                // Llamar al servicio para actualizar el valor
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
                // Manejo de excepciones
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> insertarProInventario(string nombreProducto, int cantidadProducto, float valorNetoProductoFloat, float valorVentaProductoFloat, int valorUnidadInt, string id_empresa, string categoria, int estado, string ubicacion)
        {
            try
            {
                // Llamar al servicio para actualizar el valor
                bool resultado = await _usuarioService.insertProInventario(nombreProducto, cantidadProducto, valorNetoProductoFloat, valorVentaProductoFloat, valorUnidadInt, id_empresa, categoria, estado, ubicacion);

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
                // Manejo de excepciones
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> eliminarProductoXID(string id)
        {
            try
            {
                // Llamar al servicio para actualizar el valor
                bool resultado = await _usuarioService.eliminarProductoXIdAsync(id);

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
                // Manejo de excepciones
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }


    }
}
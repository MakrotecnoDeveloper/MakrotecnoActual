using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [HttpPost]
		public async Task<IActionResult> Login(string correo, string password)
		{
			if (correo == null || password == null)
			{
                var mensaje = "No mando informacion alguna, revisar nuevamente.";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }
			else
			{

				var validarUsuarioTask = _usuarioService.GetUsuarios(correo, password);
				var validarUsuario = await validarUsuarioTask; // Espera a que la tarea se complete

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
					if(rolEmpleado > 0)
					{
                        claims.Add(new Claim("Rol", rolEmpleado.ToString()));
						string nombreCargo = _usuarioService.ObtenerNombreRolPermisos(rolEmpleado);
						if(nombreCargo != null)
						{
                            claims.Add(new Claim("NombreRol", nombreCargo));
                        }else
                        {
                            var mensaje = "Error: El nombre del cargo no esta asignado desde el Sistema Gestor de Empleados (SGE)";
                            TempData["ErrorMessage"] = mensaje;
                            return RedirectToAction("Error", "Errores");
                        }
                    }else
                    {
                        var mensaje = "Error: No tiene un cargo (ID) asignado en el Sistema Gestor de Empleados (SGE)";
                        TempData["ErrorMessage"] = mensaje;
                        return RedirectToAction("Error", "Errores");
                    }
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
						var principal = new ClaimsPrincipal(identity);

						await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
							new AuthenticationProperties()
							{
								ExpiresUtc = DateTime.UtcNow.AddMinutes(60),
								AllowRefresh = true,
								IsPersistent = false
							});
					int cedulaEmpleado = validarUsuario.Cedula;
					string correoEmpleado = validarUsuario.Correo;
					int estado = 1;
                    var logins = await _usuarioService.InsertarLogLogin(cedulaEmpleado, correoEmpleado, estado);
                    // Redirige a la acción "Index" del controlador "Inicio"
                    return RedirectToAction("Index", "Inicio");
				}
				else
				{
                    var mensaje = "Error: Correo o Contraseña no existe, validar informacion nuevamente.";
                    TempData["ErrorMessage"] = mensaje;
                    return RedirectToAction("Error", "Errores");
                }
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

	}
}
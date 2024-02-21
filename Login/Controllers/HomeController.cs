using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
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
				return View("Error/SinDatos");
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

					var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
					var principal = new ClaimsPrincipal(identity);

					await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
						new AuthenticationProperties()
						{
							ExpiresUtc = false == true ? DateTime.UtcNow.AddMonths(2) : DateTime.UtcNow.AddMinutes(60),
							AllowRefresh = true,
							IsPersistent = false
						});

					// Redirige a la acción "Index" del controlador "Inicio"
					return RedirectToAction("Index", "Inicio");
				}
				else
				{
					return View("Error/ProblemasDatos");
				}
			}
		}
		public IActionResult Logout()
        {
            return View();
        }
        
    }
}
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            if(correo == null || password == null)
            {
                return View("Error/SinDatos");
            }else
            {
                var validarUsuario = _usuarioService.GetUsuarios(correo, password);
                if(validarUsuario != null)
                {
                    var claims = new List<Claim>() {
                    new Claim("Cedula",""),
                    new Claim("Nombre",""),
                    new Claim("Apellido",""),
                    new Claim("Genero",""),
                    new Claim("Correo",""),
                    new Claim("RH",""),
                    new Claim("Celular",""),
                    new Claim("Contrasena",""),
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
                    return View("Inicio/Index");
                }else
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
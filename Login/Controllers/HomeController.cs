using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Servicios.Contrato;

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
        public IActionResult Login(string correo, string contrasena)
        {
            if(correo == null || contrasena == null)
            {
                return View("Error/SinDatos");
            }else
            {
                var validarUsuario = _usuarioService.GetUsuarios(correo, contrasena);
                if(validarUsuario != null)
                {
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
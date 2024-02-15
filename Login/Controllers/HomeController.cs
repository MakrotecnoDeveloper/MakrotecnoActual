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
            string Email = correo;
            string Password = contrasena;
            if(Email == null || Password == null)
            {
                return View("Error/SinDatos");
            }else
            {
                var validarUsuario = _usuarioService.GetUsuarios(correo, contrasena);

            }
            return View();
        }
        public IActionResult Logout()
        {
            return View();
        }
        
    }
}
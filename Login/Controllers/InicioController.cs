using Microsoft.AspNetCore.Mvc;
using Plataforma.Models;
using Plataforma.Recursos;
using Plataforma.Servicios.Contrato;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace Plataforma.Controllers
{
    public class InicioController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        public InicioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Registrarse()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Registrarse(Usuario modelo)
        {
            modelo.Clave = Utilidades.EncriptarClave(modelo.Clave);

            Usuario usuario_creado = await _usuarioService.SaveUsuario(modelo);

            if (usuario_creado.IdUsuario > 0)
                return RedirectToAction("iniciarSesion","Inicio");
            ViewData["Mensaje"] = "No se pudo crear el usuario";
            return View();
        }

        public IActionResult IniciarSesion()
        {
            return View();
        }
        public IActionResult ListarUsuarios() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ListarUsuarios(string correo, string password)
        {
            Usuario usuario_buscar = await _usuarioService.GetUsuarios(correo, password);
            return View(usuario_buscar);
        }

        [HttpPost]
        public async Task<IActionResult> IniciarSesion(string correo, string password)
        {
            //Para encriptar la clave se usaria Utilidades.EncriptarClave(password)
            Usuario usuario_encontrado = await _usuarioService.GetUsuarios(correo,password);

            if(usuario_encontrado == null)
            {
                ViewData["Mensaje"] = "No se encontraron coincidencias";
                return View();
            }


            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuario_encontrado.NombreUsuario)
            };


            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true
            };

            await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    properties
                );
            return RedirectToAction("Index", "Home");
        }
    }
}

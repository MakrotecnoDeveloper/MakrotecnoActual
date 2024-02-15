using Microsoft.AspNetCore.Mvc;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

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
        public IActionResult ListarUsuarios() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ListarUsuarios(string correo, string password)
        {
            Empleado usuario_buscar = await _usuarioService.GetUsuarios(correo, password);
            return View(usuario_buscar);
        }
    }
}

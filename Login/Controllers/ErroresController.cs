using Microsoft.AspNetCore.Mvc;

namespace Plataforma.Controllers
{
    public class ErroresController : Controller
    {
        public IActionResult Error()
        {
            var mensaje = TempData["ErrorMessage"] as string;
            return View("Error", mensaje);
        }
    }
}
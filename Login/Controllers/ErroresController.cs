using Microsoft.AspNetCore.Mvc;

namespace Plataforma.Controllers
{
    public class ErroresController : Controller
    {
        public IActionResult Error()
        {
            var mensaje = TempData["ErrorMessage"] as string;

            if (mensaje.Contains("Stock insuficiente"))
            {
                return View("ErrorStock", mensaje);  // Vista especial para error de stock
            }

            return View("Error", mensaje);  // Vista genérica
        }

    }
}
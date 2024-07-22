using Microsoft.AspNetCore.Mvc;

public class ErroresController : Controller
{
    public IActionResult Error()
    {
        var mensaje = TempData["ErrorMessage"] as string;
        return View("Error", mensaje);
    }
    [Route("Errores/Denied")]
    public IActionResult Denied()
    {
        return View(); // Asegúrate de tener una vista correspondiente
    }
    public IActionResult ManejarErrores()
    {
        // Recupera el mensaje de TempData
        var mensajeError = TempData["ErrorMensaje"] as string;

        // Pasa el mensaje a la vista
        ViewBag.ErrorMensaje = mensajeError;

        // Resto de la lógica del controlador...
        return View();
    }
}
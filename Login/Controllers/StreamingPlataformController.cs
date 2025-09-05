using Microsoft.AspNetCore.Mvc;
using Plataforma.Servicios.Contrato;
using Plataforma.Servicios.Implementacion;

namespace Plataforma.Controllers
{
    public class StreamingPlataformController : Controller
    {

        public readonly IStreamingPlataformService _streamingPlatformService;

        public StreamingPlataformController(IStreamingPlataformService streamingPlatformService)
        {
            _streamingPlatformService = streamingPlatformService;
        }
        public IActionResult Index()
        {
            return View();
        }
        //Configurar vistas 

        /*Visualizacion de  Recargas de Plataformas */
        public IActionResult FormPlataforma()
        {
            var traerPlataformasExistentes = _streamingPlatformService.TraerPlataformasExistentes();
            return View(traerPlataformasExistentes);
        }


        // Obtener datos de clientes relacionados con una suscripción
        [HttpGet]
        public async Task<IActionResult> GetDatosSuscripcion(int suscripcionId)
        {
            var datos = await _streamingPlatformService.ObtenerDatosSuscripcion(suscripcionId);
            return Json(datos);
        }
        [HttpGet]
        public async Task<IActionResult> GetDatosPlataforma(int suscripcionId)
        {
            var datos = await _streamingPlatformService.ObtenerDatosPlataforma(suscripcionId);
            return Json(datos);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var resultado = await _streamingPlatformService.EliminarClienteAsync(id);
            if (resultado)
            {
                return Ok(new { message = "Cliente eliminado con éxito." });
            }
            return BadRequest(new { message = "Error al eliminar el cliente o cliente no encontrado." });
        }

        [HttpPost]
        public async Task<IActionResult> EditarEstadoCta(int id, int estado, int idCliente)
        {
            Console.WriteLine("IDCLIENTEPLATAFORMA: " + id + " ESTADO: " + estado + " IDCLIENTE " + idCliente);
            await _streamingPlatformService.ActualizarCliente(id, estado, idCliente);
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult InsertPlataforma(int idPlataforma, string plataformas, string descripcion, int valorventa, int valorneto, 
            DateTime fechaInipago, DateTime fechaFinpago, int cantidad, string correo, string contrasena, int cedula, int estado)
        {
            if (string.IsNullOrEmpty(descripcion) || valorventa <= 0 || valorneto <= 0 || fechaInipago == DateTime.MinValue 
                || fechaFinpago == DateTime.MinValue || cantidad <= 0 || string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena) || cedula <= 0 || estado <= 0)
            {
                var mensaje = "Error: Hay campos sin informacion digitada, revisar todo lo llenado.";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }
            else
            {
                _streamingPlatformService.InserPlataformaService(idPlataforma, descripcion, valorventa, valorneto, fechaInipago, fechaFinpago, cantidad, correo, contrasena, cedula, estado);
                return View("FormPlataforma");
            }
        }
        public IActionResult FormInserClienPlatf()
        {
            var traerPlataformas = _streamingPlatformService.SuscripcionesActivas();
            return View(traerPlataformas);
        }
        public IActionResult InsertVentClientPltf(string nombrecliente, string celularcliente, string correo, 
            string contrasena, int idPltfSuscripcion, int cantidad, string ppm, DateTime feciniplat, 
            DateTime fecfinplat, int valorventa, int valorneto, int cedula, int estado, string clave)
        {
            _streamingPlatformService.ServicioInsertarVentClientPlataforma(nombrecliente, celularcliente, correo, contrasena, idPltfSuscripcion, cantidad, ppm, feciniplat, fecfinplat, valorventa, valorneto, cedula, estado, clave);
            return RedirectToAction("formInserClienPlatf", "Producto");
        }
        public IActionResult FormVisuPlatf()
        {
            var searchPlataform = _streamingPlatformService.TraerPlataformasExistentes();
            return View(searchPlataform);
        }
        public IActionResult FormVisuCta()
        {
            var searchPlataform = _streamingPlatformService.TraerPlataformasExistentes();
            return View(searchPlataform);
        }
        [HttpGet]
        public async Task<IActionResult> GetSuscripcionesActivas(int plataformaId)
        {
            var suscripciones = await _streamingPlatformService.ObtenerSuscripcionesActivas(plataformaId);
            return Json(suscripciones);
        }


    }
}

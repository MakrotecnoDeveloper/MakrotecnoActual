using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Models;
using Plataforma.Models.Dto.Streaming;
using Plataforma.Servicios.Contrato;
using Plataforma.Servicios.Implementacion;

namespace Plataforma.Controllers
{
    public class StreamingController : Controller
    {
        private readonly IStreamingService _streamingservice;
        public StreamingController(IStreamingService streamingservice)
        {
            _streamingservice = streamingservice;
        }
        /*Visualizacion de  Recargas de Plataformas */
        public IActionResult FormPlataforma()
        {
            var traerPlataformasExistentes = _streamingservice.TraerPlataformasExistentes();
            return View(traerPlataformasExistentes);
        }
        [HttpPost]
        public IActionResult InsertPlataforma(int idPlataforma, string plataformas, string descripcion, int valorventa, int valorneto, DateTime fechaInipago, DateTime fechaFinpago, int cantidad, string correo, string contrasena, int cedula, int estado)
        {
            if (string.IsNullOrEmpty(descripcion) || valorventa <= 0 || valorneto <= 0 || fechaInipago == DateTime.MinValue || fechaFinpago == DateTime.MinValue || cantidad <= 0 || string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena) || cedula <= 0 || estado <= 0)
            {
                var mensaje = "Error: Hay campos sin informacion digitada, revisar todo lo llenado.";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }
            else
            {
                _streamingservice.InserPlataformaService(idPlataforma, descripcion, valorventa, valorneto, fechaInipago, fechaFinpago, cantidad, correo, contrasena, cedula, estado);
                return View("FormPlataforma");
            }
        }
        public IActionResult FormInserClienPlatf()
        {
            var traerPlataformas = _streamingservice.SuscripcionesActivas();
            return View(traerPlataformas);
        }
        public IActionResult InsertVentClientPltf(string nombrecliente, string celularcliente, string correo, string contrasena, int idPltfSuscripcion, int cantidad, string ppm, DateTime feciniplat, DateTime fecfinplat, int valorventa, int valorneto, int cedula, int estado, string clave)
        {
            _streamingservice.ServicioInsertarVentClientPlataforma(nombrecliente, celularcliente, correo, contrasena, idPltfSuscripcion, cantidad, ppm, feciniplat, fecfinplat, valorventa, valorneto, cedula, estado, clave);
            return RedirectToAction("formInserClienPlatf", "Producto");
        }
        public IActionResult FormVisuPlatf()
        {
            var searchPlataform = _streamingservice.TraerPlataformasExistentes();
            return View(searchPlataform);
        }
        public IActionResult FormVisuCta()
        {
            var searchPlataform = _streamingservice.TraerPlataformasExistentes();
            return View(searchPlataform);
        }
        [HttpGet]
        public async Task<IActionResult> GetSuscripcionesActivas(int plataformaId)
        {
            var suscripciones = await _streamingservice.ObtenerSuscripcionesActivas(plataformaId);
            return Json(suscripciones);
        }

        // Obtener datos de clientes relacionados con una suscripción
        [HttpGet]
        public async Task<IActionResult> GetDatosSuscripcion(int suscripcionId)
        {
            var datos = await _streamingservice.ObtenerDatosSuscripcion(suscripcionId);
            return Json(datos);
        }
        [HttpGet]
        public async Task<IActionResult> GetDatosPlataforma(int suscripcionId)
        {
            var datos = await _streamingservice.ObtenerDatosPlataforma(suscripcionId);
            return Json(datos);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var resultado = await _streamingservice.EliminarClienteAsync(id);
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
            await _streamingservice.ActualizarCliente(id, estado, idCliente);
            return Json(new { success = true });
        }
        [HttpGet]
        public IActionResult ConsultaCuentasStreaming()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ConsultarCuentasStreamingJson(
        string? termino,
        DateTime? fechaInicio,
        DateTime? fechaFin,
        string? indicador)
        {
            try
            {
                var filtro = new FiltroCuentasStreamingDTO
                {
                    Termino = termino,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    Indicador = indicador
                };

                var cuentas = await _streamingservice.ConsultarCuentasStreamingAsync(filtro);

                return Json(new
                {
                    ok = true,
                    cuentas
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    ok = false,
                    mensaje = "Error al consultar cuentas de streaming.",
                    detalle = ex.Message
                });
            }
        }
        [HttpGet]
        public async Task<IActionResult> EstructuraPorPlataforma()
        {
            var plataformas = await _streamingservice.ObtenerPlataformasAsync();

            return View(plataformas);
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerEstructuraPorPlataforma(int idPlataforma)
        {
            try
            {
                var datos = await _streamingservice.ObtenerEstructuraPorPlataformaAsync(idPlataforma);

                return Json(new
                {
                    ok = true,
                    datos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    ok = false,
                    mensaje = "Error al obtener la estructura de la plataforma.",
                    detalle = ex.Message
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> ActualizarCuentaPlan([FromBody] ActualizarCuentaPlanDTO model)
        {
            try
            {
                var resultado = await _streamingservice.ActualizarCuentaPlanAsync(model);

                if (!resultado.ok)
                {
                    return BadRequest(new
                    {
                        ok = false,
                        mensaje = resultado.mensaje
                    });
                }

                return Json(new
                {
                    ok = true,
                    mensaje = resultado.mensaje
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    ok = false,
                    mensaje = "Error al actualizar la cuenta / plan.",
                    detalle = ex.Message
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> AsignarClienteACuenta([FromBody] AsignarClienteACuentaDTO model)
        {
            try
            {
                var resultado = await _streamingservice.AsignarClienteACuentaAsync(model);

                if (!resultado.ok)
                {
                    return BadRequest(new
                    {
                        ok = false,
                        mensaje = resultado.mensaje
                    });
                }

                return Json(new
                {
                    ok = true,
                    mensaje = resultado.mensaje
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    ok = false,
                    mensaje = "Error al asignar el cliente a la cuenta / plan.",
                    detalle = ex.Message
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> ActualizarPerfilCuenta([FromBody] ActualizarPerfilCuentaDTO model)
        {
            try
            {
                var resultado = await _streamingservice.ActualizarPerfilCuentaAsync(model);

                if (!resultado.ok)
                {
                    return BadRequest(new
                    {
                        ok = false,
                        mensaje = resultado.mensaje
                    });
                }

                return Json(new
                {
                    ok = true,
                    mensaje = resultado.mensaje
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    ok = false,
                    mensaje = "Error al actualizar el perfil.",
                    detalle = ex.Message
                });
            }
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using Plataforma.Servicios.Implementacion;

namespace Plataforma.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IProductoService _productoservice;
        public ProductoController(IProductoService productoservice)
        {
            _productoservice = productoservice;
        }
        public IActionResult Index()
        {
            var productos = _productoservice.ObtenerProductos();
            return View(productos);
        }
        public IActionResult Insertar() 
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Insertar(string id_empresa, string codigo, string descripcion, float valorNeto, float valorVenta, int stock, string categoria)
        {

            if (ModelState.IsValid)
            {
                // Lógica para agregar el producto usando _productoService
                var resultado = await _productoservice.AgregarProductoAsync(id_empresa, codigo, descripcion, valorNeto, valorVenta, stock, categoria);

                if (resultado)
                {
                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }
        [Authorize]
        [HttpGet]
        public IActionResult Buscar(string searchTerm, string categoriaTerm)
        {
            if (string.IsNullOrEmpty(searchTerm)) {
                searchTerm = "";
            }else
            {
                categoriaTerm = "";
            }
            var productosEncontrados = _productoservice.BuscarProductos(searchTerm, categoriaTerm);
            return PartialView("_TablaProductos", productosEncontrados);
        }
        [HttpGet]
        public IActionResult BuscarSinStock(string searchTerm, string categoriaTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = "";
            }
            else
            {
                categoriaTerm = "";
            }
            var productosSinStock = _productoservice.SinStock(searchTerm, categoriaTerm);
            return PartialView("_TablaProductos", productosSinStock);
        }
        [HttpGet]
        public IActionResult BuscarProximosSinStock(string searchTerm, string categoriaTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = "";
            }
            else
            {
                categoriaTerm = "";
            }
            var productosEncontrados = _productoservice.BuscarProSinStock(searchTerm, categoriaTerm);
            return PartialView("_TablaProductos", productosEncontrados);
        }
        public IActionResult Editar(string id)
        {
            string categoriaTerm = "";
            var editarProducto = _productoservice.BuscarProductos(id, categoriaTerm);
            foreach (var producto in editarProducto)
            {
                // Realiza acciones con cada producto, por ejemplo:
                Console.WriteLine($"ID: {producto.Cod_Producto}, Nombre: {producto.NombreProducto}");
            }
            if (editarProducto.Any())
            {
                // Oculta la tabla de productos y muestra la tabla temporal
                return View(editarProducto);
            }
            else
            {
                // Producto no encontrado, maneja la lógica adecuada
                Console.WriteLine("No hay productos con ese codigo referenciado");
                return View("Index");
            }
        }
        [HttpPost]
        public IActionResult EditarProducto(string codigo, string nombreProducto, float valorNeto, float valorVenta, int valorUnidad, int cantidad, string categoria, string idEmpresa, int estado)
        {
            // Llama al método EditarProducto del servicio de productos
            _productoservice.EditarProducto(codigo, nombreProducto, valorNeto, valorVenta, valorUnidad, cantidad, categoria, idEmpresa, estado);

            // Redirige a la acción que deseas después de editar el producto
            return RedirectToAction("Index"); // Por ejemplo, redirigir a la página de inicio del controlador de productos
        }
        public IActionResult Stock(string id, int cantidad, int opcion)
        {
            var productosActualizados = _productoservice.EditarStock(id, cantidad, opcion);
            if (productosActualizados.Any())
            {
                return View("Index", productosActualizados);
            }
            else
            {
                ViewBag.Mensaje = "Producto no encontrado";
                return View("_Mensaje");
            }
        }
        public IActionResult Eliminar(string id)
        {
            _productoservice.EliminarProducto(id);
            return RedirectToAction("Index");
        }
        public IActionResult VisualizarProducto(string id)
        {
            string categoriaTerm = "";
            string searchTerm = id;
            var traerProductos = _productoservice.BuscarProductos(searchTerm, categoriaTerm);
            return View(traerProductos);
        }
        /*Visualizacion de  Recargas de Plataformas */
        public IActionResult formPlataforma()
        {
            var traerPlataformasExistentes = _productoservice.traerPlataformasExistentes();
            return View(traerPlataformasExistentes);
        }
        [HttpPost]
        public IActionResult insertPlataforma(int idPlataforma, string plataformas, string descripcion, int valorventa, int valorneto, DateTime fechaInipago, DateTime fechaFinpago, int cantidad, string correo, string contrasena, int cedula, int estado) 
        {
            if(string.IsNullOrEmpty(descripcion) || valorventa <= 0 || valorneto <= 0 || fechaInipago == DateTime.MinValue || fechaFinpago == DateTime.MinValue || cantidad <= 0 || string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena) || cedula <= 0 || estado <= 0)
            {
                var mensaje = "Error: Hay campos sin informacion digitada, revisar todo lo llenado.";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }else
            {
                _productoservice.inserPlataformaService(idPlataforma, descripcion, valorventa, valorneto, fechaInipago, fechaFinpago, cantidad, correo, contrasena, cedula, estado);
                return View("formPlataforma");
            }
        }
        public IActionResult formInserClienPlatf()
        {
            var traerPlataformas = _productoservice.SuscripcionesActivas();
            return View(traerPlataformas);
        }
        public IActionResult insertVentClientPltf(string nombrecliente, string celularcliente, string correo, string contrasena, int idPltfSuscripcion, int cantidad, string ppm, DateTime fecfinplat, int valorventa, int valorneto, int cedula, int estado, string clave)
        {
            DateTime feciniplat = DateTime.Now; 
            _productoservice.servicioInsertarVentClientPlataforma(nombrecliente, celularcliente, correo, contrasena, idPltfSuscripcion, cantidad, ppm, feciniplat, fecfinplat, valorventa, valorneto, cedula, estado, clave);
            return RedirectToAction("formInserClienPlatf", "Producto");
        }
        public IActionResult formVisuPlatf()
        {
            var searchPlataform = _productoservice.traerPlataformasExistentes();
            return View(searchPlataform);
        }
        public IActionResult formVisuCta()
        {
            var searchPlataform = _productoservice.traerPlataformasExistentes();
            return View(searchPlataform);
        }
        [HttpGet]
        public async Task<IActionResult> GetSuscripcionesActivas(int plataformaId)
        {
                var suscripciones = await _productoservice.ObtenerSuscripcionesActivas(plataformaId);
                return Json(suscripciones);
        }

        // Obtener datos de clientes relacionados con una suscripción
        [HttpGet]
        public async Task<IActionResult> GetDatosSuscripcion(int suscripcionId)
        {
            var datos = await _productoservice.ObtenerDatosSuscripcion(suscripcionId);
            return Json(datos);
        }
        [HttpGet]
        public async Task<IActionResult> GetDatosPlataforma(int suscripcionId)
        {
            var datos = await _productoservice.ObtenerDatosPlataforma(suscripcionId);
            return Json(datos);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var resultado = await _productoservice.EliminarClienteAsync(id);
            if (resultado)
            {
                return Ok(new { message = "Cliente eliminado con éxito." });
            }
            return BadRequest(new { message = "Error al eliminar el cliente o cliente no encontrado." });
        }
        [HttpPost]
        public async Task<IActionResult> EditarEstadoCta(int id, int estado, int idCliente)
        {
                await _productoservice.ActualizarCliente(id, estado, idCliente);
                return Json(new { success = true });
        }
        //Visualizar productos existentes para vender en la pagina inicial
        public IActionResult productosExistentesVenta()
        {
            var traerProductosExistentes = _productoservice.ObtenerProductosInventarioWeb();
            return View("../Home/productosExistentesVenta", traerProductosExistentes);
        }
        public IActionResult traerProductoXCategoria(string categoria)
        {
            var productosTraidos = _productoservice.traerProductosXCategoria(categoria);
            return PartialView("../Home/_ProductosParciales", productosTraidos);
        }

        /*CHATGPT*/
        [HttpGet]
        public IActionResult Chat()
        {
            return View(new ChatViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> MsjChatGPT(string Mensaje)
        {
            if (string.IsNullOrEmpty(Mensaje))
            {
                return BadRequest("El mensaje no puede estar vacío.");
            }

            try
            {
                // Buscar productos basados en el mensaje
                var productos = await _productoservice.BuscarProductosAsync(Mensaje);

                // Generar una respuesta usando OpenAI
                var respuesta = await _productoservice.GenerarRespuestaAsync(Mensaje, productos);

                // Crear un modelo para la vista parcial
                var model = new ChatViewModel
                {
                    Mensaje = Mensaje,
                    Respuesta = respuesta
                };

                // Devolver la vista parcial con la respuesta generada
                return PartialView("_ChatMessages", model);
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }

    }
}

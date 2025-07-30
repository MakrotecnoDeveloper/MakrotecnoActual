using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            var model = new ProductosCategoriaViewModel
            {
                Productos = _productoservice.ObtenerProductos(),
                CategoriaProductos = _productoservice.ObtenerCategorias()
            };

            return View(model);
        }

        // Para la búsqueda asincrónica
        public IActionResult ProductosPorCategoria(int idCategoria)
        {
            var productos = _productoservice.ObtenerProductosPorCategoria(idCategoria);
            return PartialView("_TablaProductos", productos);
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerCategoriaProductos(int idServicio)
        {
            var categorias = await _productoservice.ObtenerCategoriasPorServicio(idServicio);

            return Json(categorias.Select(c => new {
                idCateProducto = c.IdCateProducto,
                descripcion = c.Descripcion
            }));
        }
        [HttpGet]
        public async Task<IActionResult> Insertar()
        {
            var model = new ProductoInsertarViewModel
            {
                Servicios = await _productoservice.ObtenerServicios(),
                Categorias = new List<CategoriaProductos>() // o datos reales si los tienes
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Insertar(string id_empresa, string codigo, string descripcion, float valor_neto, float valor_unitario, decimal stock, int categorias)
        {

            if (ModelState.IsValid)
            {
                // Lógica para agregar el producto usando _productoService
                var resultado = await _productoservice.AgregarProductoAsync(id_empresa, codigo, descripcion, valor_neto, valor_unitario, stock, categorias);

                if (resultado)
                {
                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }
        [Authorize]
        [HttpGet]
        public IActionResult Buscar(string searchTerm, int categoriaTerm)
        {
            if (string.IsNullOrEmpty(searchTerm)) {
                searchTerm = "";
            }else
            {
                categoriaTerm = 0;
            }
            var productosEncontrados = _productoservice.BuscarProductos(searchTerm, categoriaTerm);
            return PartialView("_TablaProductos", productosEncontrados);
        }
        [HttpGet]
        public IActionResult BuscarSinStock(string searchTerm, int categoriaTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = "";
            }
            else
            {
                categoriaTerm = 0;
            }
            var productosSinStock = _productoservice.SinStock(searchTerm, categoriaTerm);
            return PartialView("_TablaProductos", productosSinStock);
        }
        [HttpGet]
        public IActionResult BuscarProximosSinStock(string searchTerm, int categoriaTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = "";
            }
            else
            {
                categoriaTerm = 0;
            }
            var productosEncontrados = _productoservice.BuscarProSinStock(searchTerm, categoriaTerm);
            return PartialView("_TablaProductos", productosEncontrados);
        }
        public IActionResult Editar(string id)
        {
            int categoriaTerm = 0;
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
        public IActionResult EditarProducto(string codigo, string nombreProducto, float valorNeto, float valorVenta, int valorUnidad, int cantidad, int categoria, string idEmpresa, int estado)
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
            int categoriaTerm = 0;
            string searchTerm = id;
            var traerProductos = _productoservice.BuscarProductos(searchTerm, categoriaTerm);
            return View(traerProductos);
        }
        /*Visualizacion de  Recargas de Plataformas */
        public IActionResult FormPlataforma()
        {
            var traerPlataformasExistentes = _productoservice.TraerPlataformasExistentes();
            return View(traerPlataformasExistentes);
        }
        [HttpPost]
        public IActionResult InsertPlataforma(int idPlataforma, string plataformas, string descripcion, int valorventa, int valorneto, DateTime fechaInipago, DateTime fechaFinpago, int cantidad, string correo, string contrasena, int cedula, int estado) 
        {
            if(string.IsNullOrEmpty(descripcion) || valorventa <= 0 || valorneto <= 0 || fechaInipago == DateTime.MinValue || fechaFinpago == DateTime.MinValue || cantidad <= 0 || string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena) || cedula <= 0 || estado <= 0)
            {
                var mensaje = "Error: Hay campos sin informacion digitada, revisar todo lo llenado.";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }else
            {
                _productoservice.InserPlataformaService(idPlataforma, descripcion, valorventa, valorneto, fechaInipago, fechaFinpago, cantidad, correo, contrasena, cedula, estado);
                return View("FormPlataforma");
            }
        }
        public IActionResult FormInserClienPlatf()
        {
            var traerPlataformas = _productoservice.SuscripcionesActivas();
            return View(traerPlataformas);
        }
        public IActionResult InsertVentClientPltf(string nombrecliente, string celularcliente, string correo, string contrasena, int idPltfSuscripcion, int cantidad, string ppm, DateTime feciniplat, DateTime fecfinplat, int valorventa, int valorneto, int cedula, int estado, string clave)
        {
            _productoservice.ServicioInsertarVentClientPlataforma(nombrecliente, celularcliente, correo, contrasena, idPltfSuscripcion, cantidad, ppm, feciniplat, fecfinplat, valorventa, valorneto, cedula, estado, clave);
            return RedirectToAction("formInserClienPlatf", "Producto");
        }
        public IActionResult FormVisuPlatf()
        {
            var searchPlataform = _productoservice.TraerPlataformasExistentes();
            return View(searchPlataform);
        }
        public IActionResult FormVisuCta()
        {
            var searchPlataform = _productoservice.TraerPlataformasExistentes();
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
            //Console.WriteLine("IDCLIENTEPLATAFORMA: " + id + " ESTADO: " + estado + " IDCLIENTE " + idCliente);
                await _productoservice.ActualizarCliente(id, estado, idCliente);
                return Json(new { success = true });
        }
        //fin plataformas de streaming
        //Visualizar productos existentes para vender en la pagina inicial
        [HttpGet]
        public IActionResult ProductosExistentesVenta(int IdServicio)
        {
            var traerCategoriasExistentes = _productoservice.ObtenerCategoriaProductos(IdServicio);
            return View("../Home/productosExistentesVenta", traerCategoriasExistentes);
        }
        public IActionResult TraerProductoXCategoria(int categoria)
        {
            var productosTraidos = _productoservice.TraerProductosXCategoria(categoria);
            return PartialView("../Home/_ProductosParciales", productosTraidos);
        }
        [HttpGet]
        public async Task<IActionResult> CategoriaProductos()
        {
            var servicios = await _productoservice.ObtenerServiciosAsync();

            var viewModel = new CategoriaProductosViewModel
            {
                Servicios = servicios
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCate(CategoriaProductosViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Servicios = await _productoservice.ObtenerServiciosAsync();
                return View(model);
            }

            var categoria = new CategoriaProductos
            {
                Descripcion = model.Descripcion,
                IdServicio = model.IdServicio
            };

            var exito = await _productoservice.CrearCategoriaAsync(categoria);

            if (exito)
            {
                TempData["Mensaje"] = "Categoría creada correctamente.";
                return RedirectToAction("CategoriaProductos"); // o una vista de categorías
            }

            ModelState.AddModelError("", "No se pudo crear la categoría.");
            model.Servicios = await _productoservice.ObtenerServiciosAsync();
            return View(model);
        }
        public async Task<IActionResult> CreateService()
        {
            var servicios = await _productoservice.ObtenerServicios();
            return View(servicios);
        }

        [HttpGet]
        public IActionResult FormCrearServicio()
        {
            return PartialView("_FormCrearServicio", new Servicio());
        }

        [HttpPost]
        public async Task<IActionResult> CrearServicio(Servicio servicio)
        {
            if (ModelState.IsValid)
            {
                var nuevoServicio = await _productoservice.CrearServicio(servicio);
                return PartialView("_ServicioRow", nuevoServicio);
            }

            return BadRequest("Error al crear servicio");
        }
    }
}

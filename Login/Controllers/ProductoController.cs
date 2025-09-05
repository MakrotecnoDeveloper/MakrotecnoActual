using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using Plataforma.Servicios.Implementacion;
using System.Threading.Tasks;

namespace Plataforma.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IProductoService _productoservice;
        private readonly IFacturaService _facturaService;
        private readonly ILogger<ProductoService> _logger;

        public ProductoController(IProductoService productoservice, IFacturaService facturaService ,ILogger<ProductoService> logger)
        {
            _productoservice = productoservice;
            _facturaService = facturaService;
            _logger = logger;
        }
        public IActionResult Index()
        {
            var productos = _productoservice.ObtenerProductos();
            return View(productos);
        }
        public IActionResult Insertar() 
        {
            int IdServicio = 1;
            var traerCategoriasExistentes = _productoservice.ObtenerCategoriaProductos(IdServicio);
            return View(traerCategoriasExistentes);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Insertar(string id_empresa, string codigo, string descripcion, float valorNeto, float valorVenta, decimal stock, int categoria)
        public async Task<IActionResult> Insertar(Producto model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    // Lógica para agregar el producto usando _productoService
                   // var resultado = await _productoservice.AgregarProductoAsync(id_empresa, codigo, descripcion, valorNeto, valorVenta, stock, categoria);
                    var resultado = await _productoservice.AgregarProductoAsync(model);

                    if (resultado)
                    {
                        return Json(new { success = true });
                    }
                }

                return Json(new { success = false });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, msj = ex.Message });

            }

        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuscarProdctoFiltro(string searchTerm, int categoriaTerm, int type)
        {
            try
            {
                //Usar FindList para buscar productos por nombre o codigo
                if (string.IsNullOrEmpty(searchTerm) || categoriaTerm == 0)
                    throw new Exception("Termino de busqueda no valido, debe " +
                        "ingresar un codigo o nombre de producto o categoria.");
                

                List<Producto> productosEncontrados = new List<Producto>();

                productosEncontrados = await _productoservice
                           .FindListByFunction(p => p.Cod_Producto == searchTerm || 
                           p.IdCatepro == categoriaTerm
                           && p.Estado == 1);


                switch (type)
                {
                    case 1: // Buscar Codigo Producto 
                        productosEncontrados = productosEncontrados.Where(p => p.Cod_Producto == searchTerm).ToList();
                        break;
                    case 2: // Buscar Producto sin Stock
                        productosEncontrados = productosEncontrados
                            .Where(p => p.CantidadProducto == 0).ToList();
                        break;
                    case 3: // Buscar por Cantidad ya para acabarse
                        productosEncontrados = productosEncontrados
                            .Where(p => p.CantidadProducto < 5).ToList();
                        break;
                    default:
                        // Si no se especifica un tipo, buscar por nombre por defecto
                        productosEncontrados = await _productoservice
                            .FindListByFunction(p => p.NombreProducto == searchTerm
                            && p.Estado == 1);
                        break;
                }

                return PartialView("_TablaProductos", productosEncontrados);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar productos: {SearchTerm}, Categoria: {CategoriaTerm}", searchTerm, categoriaTerm);
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> BuscarSinStock(string searchTerm, int categoriaTerm)
        {
            if (string.IsNullOrEmpty(searchTerm)) 
                searchTerm = "";
                    categoriaTerm = 0;


            var productosSinStock = (await _productoservice
                .FindListByFunction(p => p.Cod_Producto == searchTerm || p.IdCatepro == categoriaTerm && p.Estado == 1))
                .Where(p => p.CantidadProducto == 0)            
                .ToList();


            return PartialView("_TablaProductos", productosSinStock);
        }
        [HttpGet]
        public async  Task<IActionResult> BuscarProximosSinStock(string searchTerm, int categoriaTerm)
        {
            try
            {

                if (string.IsNullOrEmpty(searchTerm))
                    searchTerm = "";
                categoriaTerm = 0;


                var productosEncontrados = 
                    await _productoservice .FindListByFunction( p => p.IdCatepro == categoriaTerm && p.Estado == 1 && p.CantidadProducto < 5 );

                return PartialView("_TablaProductos", productosEncontrados);

            }
            catch (Exception ex)
            {

                throw;
            }

        }
        public async Task<IActionResult> Editar(string id)
        {
            //Validar logica ya que edita todos los productos con el mismo ID
            //int categoriaTerm = 0;
            //var editarProducto = _productoservice.BuscarProductos(id, categoriaTerm);

            var editarProducto = await _productoservice.FindListByFunction(p => p.Cod_Producto == id && p.Estado == 1);

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
            try
            {
                int categoriaTerm = 0;
                string searchTerm = id;
                var traerProductos = _productoservice.FindListByFunction(x => x.Cod_Producto == searchTerm || x.IdCatepro == categoriaTerm);
                return View(traerProductos);
            }
            catch (Exception)
            {
                //Implementar un log error con modal
                _logger.LogError("Error al visualizar el producto con ID: {Id}", id);
                throw;
            }
        }

      
        #region Productos Existentes para Venta
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
        public IActionResult ComprasProductos()
        {
            var cedulaClaim = User.FindFirst("Cedula");
            if (cedulaClaim != null && int.TryParse(cedulaClaim.Value, out int cedula))
            {
                var proveedorProducto = _productoservice.TraerProveedorProductos(cedula);
                return View(proveedorProducto);
            }else
            {
                var mensaje = "El claim 'Cedula' no existe o la conversión falló.";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }

        }
#endregion

        [HttpPost]
        public IActionResult GuardarHistoricoCompra(List<ProductoViewModel> productos, int codfact, DateTime fechaRegistro, string tpventa)
        {
            try
            {
                int idPDv = 1;
                decimal StockDecimal = 0;
                decimal total = 0;
                foreach (var producto in productos)
                {

                    if (producto.Stock.Contains("/"))
                    {
                        var partes = producto.Stock.Split('/');
                        if (partes.Length == 2 && decimal.TryParse(partes[0], out decimal numerador) && decimal.TryParse(partes[1], out decimal denominador) && denominador != 0)
                        {
                            StockDecimal = numerador / denominador;
                        }
                        else
                        {
                            var mensaje = $"Fraccion invalida en Cantidad: {producto.Stock}";
                            TempData["ErrorMessage"] = mensaje;
                            return RedirectToAction("Error", "Errores");
                        }
                    }
                    else
                    {
                        if (!decimal.TryParse(producto.Stock, out StockDecimal))
                        {
                            var mensaje = $"Stock invalido: {producto.Stock}";
                            TempData["ErrorMessage"] = mensaje;
                            return RedirectToAction("Error", "Errores");
                        }
                    }
                    total = StockDecimal * producto.VNeto;
                    //Console.WriteLine($"Producto: {producto.Codigo} | Vneto: {producto.VNeto} | Cantidad: {StockDecimal} | Total: {total}");
                    // Insertar pedido por cada producto
                    _productoservice.HistoricoCompra(
                        codfact,
                        producto.Codigo,
                        StockDecimal,
                        producto.UnidadMedida,
                        producto.VNeto,
                        total,
                        fechaRegistro,
                        tpventa,
                        idPDv
                    );
                }
                return RedirectToAction("ComprasProductos");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar el historico de compra.");
                var mensaje = "Ocurrió un error al procesar la compra. Por favor, inténtelo de nuevo más tarde.";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }
        }

        #region Facturacion
        public IActionResult VisualizarCompras()
        {
            return View();
        }
        [HttpPost]
        public IActionResult BuscarFactXFecha(DateTime fechaEscoger)
        {
            var cedulaClaim = User.FindFirst("Cedula");
            if (cedulaClaim != null && int.TryParse(cedulaClaim.Value, out int cedula))
            {
                var facturas = _facturaService.ObtenerFacturasPorFechaYUsuario(fechaEscoger, cedula);

                // Transformar las facturas a un objeto más ligero si es necesario
                var result = facturas.Select(f => new
                {
                    codFactura = f.Cod_factura,
                    fechaVenta = f.FechaVenta.ToShortDateString() // Formatear la fecha
                });

                return Json(result);
            }
            else
            {
                var mensaje = "El claim 'Cedula' no existe o la conversión falló.";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }
                
        }
        [HttpGet]
        public IActionResult ObtenerDetallesFactura(int codFactura)
        {
            // Llama al servicio para obtener los detalles
            var detallesFactura = _facturaService.ObtenerDetallesFactura(codFactura); 

            if (detallesFactura == null)
            {
                var mensaje = "Resultado Null, revisar datos.";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }

            // Devuelve la vista parcial con los detalles de la factura
            return PartialView("_DetallesFactura", detallesFactura);
        }

        #endregion
    }
}
 
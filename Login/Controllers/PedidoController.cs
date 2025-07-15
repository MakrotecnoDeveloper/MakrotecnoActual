using Microsoft.AspNetCore.Mvc;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IPedidoService _pedidoServicio;
        private readonly IProductoService _productoservice;
        private readonly ILogger<HomeController> _logger;
        public PedidoController(IPedidoService pedidoServicio, IProductoService productoservice, ILogger<HomeController> logger)
        {
            _pedidoServicio = pedidoServicio;
            _productoservice = productoservice;
            _logger = logger;
        }
        public IActionResult Index()
        {
            _pedidoServicio.ActualizarEstadoFacturas();
            var facturas = _pedidoServicio.ObtenerFacturasFechaDescendente();
            return View(facturas);
        }
        public IActionResult AgregarFactura()
        {
            var cedulaClaim = User.FindFirst("Cedula");
            if (cedulaClaim != null && int.TryParse(cedulaClaim.Value, out int cedula))
            {
                int idPDV = _pedidoServicio.TraerUltimoIDPdv(cedula);
                var estado = _pedidoServicio.ValidarExistenteIdPDV(idPDV, cedula);
                if (estado != null)
                {
                    if (estado == 1)
                    {
                        var productos = _pedidoServicio.ObtenerFacturas();
                        return View(productos);
                    }
                    else if (estado == 0)
                    {
                        var mensaje = "Error B10: PDV Cerrado, porfavor hacer apertura";
                        TempData["ErrorMessage"] = mensaje;
                        return RedirectToAction("Error", "Errores");
                    }
                    else if (estado == 2)
                    {
                        var mensaje = "Error B10: PDV esta en mantenimiento";
                        TempData["ErrorMessage"] = mensaje;
                        return RedirectToAction("Error", "Errores");
                    }
                }
            }
            else
            {
                var mensaje = "El claim 'Cedula' no existe o la conversión falló.";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult CrearFactura(int cedula_cliente, int cedula_empleado, string estado, string tpfactura)
        {
            DateTime fechaVenta = DateTime.Now;
                if(cedula_cliente > 0)
                {
                    if(fechaVenta != DateTime.MinValue)
                    {
                        _pedidoServicio.CrearFactura(cedula_cliente, cedula_empleado, fechaVenta, estado, tpfactura);
                        if(tpfactura == "Compra")
                        {
                            return RedirectToAction("ComprasProductos", "Producto");
                        }else 
                        { 
                            return RedirectToAction("Index");
                        }
                            
                    }else
                    {
                        var mensaje = "Error A2: La fecha no es un dato valido, verificar nuevamente.";
                        TempData["ErrorMessage"] = mensaje;
                        return RedirectToAction("Error", "Errores");
                    }
                }else
                {
                    var mensaje = "Error A1: La cedula del cliente esta vacia, escribala.";
                    TempData["ErrorMessage"] = mensaje;
                    return RedirectToAction("Error", "Errores");
                }
                
        }
        [HttpGet]
        public IActionResult CrearPedido(int id)
        {
            var cedulaClaim = User.FindFirst("Cedula");
            if (cedulaClaim != null && int.TryParse(cedulaClaim.Value, out int cedula))
            {
                int idPDV = _pedidoServicio.TraerUltimoIDPdv(cedula);
                var estado = _pedidoServicio.ValidarExistenteIdPDV(idPDV, cedula);
                if (estado != null)
                {
                    if (estado == 1)
                    {
                        var factura = _pedidoServicio.BuscarFacturaPorId(id);
                        if(factura != null)
                        {
                            var productos = _productoservice.ObtenerProductos();
                            var cedulaCliente = factura.Cedula_cliente;
                            var nombreCliente = _pedidoServicio.ObtenerNombreCliente(cedulaCliente);
                            // Crear el objeto ViewModel y asignar los valores
                            var viewModel = new PedidoViewModel
                            {
                                Factura = factura,
                                Productos = productos,
                                EstadoPDV = idPDV,
                                NombreCliente = nombreCliente
                            };

                            // Pasar el ViewModel a la vista
                            return View(viewModel);

                        }
                        else
                        {
                            var mensaje = "Error: Factura no encontrada.";
                            TempData["ErrorMessage"] = mensaje;
                            return RedirectToAction("Error", "Errores");
                        }
                    }
                    else if (estado == 0)
                    {
                        var mensaje = "Error B10: PDV Cerrado, porfavor hacer apertura";
                        TempData["ErrorMessage"] = mensaje;
                        return RedirectToAction("Error", "Errores");
                    }
                    else if (estado == 2)
                    {
                        var mensaje = "Error B10: PDV esta en mantenimiento";
                        TempData["ErrorMessage"] = mensaje;
                        return RedirectToAction("Error", "Errores");
                    }
                }
            }
            else
            {
                var mensaje = "El claim 'Cedula' no existe o la conversión falló.";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }
            return RedirectToAction("Index");
        }
        public IActionResult AutocompletarCodigosProducto(string codigo)
        {
            // Obtener una lista de códigos similares desde el servicio
            var codigosProductos = _pedidoServicio.ObtenerCodigosProductosAutocompletado(codigo);
            return Json(codigosProductos); // Retorna una lista de strings
        }
        [HttpGet]
        public async Task<IActionResult> AutocompletarProducto(string codigo)
        {
            // Obtener información del producto de forma asíncrona
            var productoInfo = await _pedidoServicio.ObtenerInfoProductoAsync(codigo);

            if (productoInfo != null)
            {
                return Json(new { vneto = productoInfo.ValorNetoProducto, vventa = productoInfo.ValorVentaProducto }); // Retorna un objeto con VNeto y VVenta
            }

            return NotFound(); // Si no se encuentra el producto
        }
        [HttpPost]
        public IActionResult InsertarPedido(List<ProductoViewModel> productos, int codfact, int idpdv)
        {
            DateTime fechaIngreso = DateTime.Now;
            var cedulaClaim = User.FindFirst("Cedula");

            if (cedulaClaim != null && int.TryParse(cedulaClaim.Value, out int cedula))
            {
                int idPDV = _pedidoServicio.TraerUltimoIDPdv(cedula);
                var estado = _pedidoServicio.ValidarExistenteIdPDV(idPDV, cedula);

                if (estado == 1)
                {
                    foreach (var producto in productos)
                    {
                        if (string.IsNullOrEmpty(producto.Codigo))
                        {
                            TempData["ErrorMessage"] = "El código del producto no puede ser nulo o vacío.";
                            return RedirectToAction("Error", "Errores");
                        }
                        var tpventa = "Venta";
                        decimal stockDecimal = 0;
                        var validarExisProd = _pedidoServicio.GetProdutos(producto.Codigo);
                        if (validarExisProd != null)
                        {
                            foreach (var prod in validarExisProd)
                            {
                                if (prod.CantidadProducto <= 0)
                                {
                                    TempData["ErrorMessage"] = "El producto no tiene stock suficiente.";
                                    return RedirectToAction("Error", "Errores");
                                }
                                decimal StockDecimal = 0;
                                if (producto.Stock.Contains("/"))
                                {
                                    var partes = producto.Stock.Split('/');
                                    if (partes.Length == 2 && decimal.TryParse(partes[0], out decimal numerador) && decimal.TryParse(partes[1], out decimal denominador) && denominador != 0)
                                    {
                                        StockDecimal = numerador / denominador;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Fraccion invalida en Cantidad: {producto.Stock}");
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (!decimal.TryParse(producto.Stock, out StockDecimal))
                                    {
                                        Console.WriteLine($"Stock invalido: {producto.Stock}");
                                        continue;
                                    }
                                }
                                // Insertar pedido por cada producto
                                _pedidoServicio.InsertarPedido(
                                    codfact,
                                    producto.Codigo,
                                    StockDecimal,
                                    producto.VNeto,
                                    producto.VVenta,
                                    fechaIngreso,
                                    tpventa,
                                    idPDV
                                );
                            }
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Producto no existe.";
                            return RedirectToAction("Error", "Errores");
                        }
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "PDV no está disponible.";
                    return RedirectToAction("Error", "Errores");
                }
            }

            TempData["ErrorMessage"] = "Claim 'Cedula' no existe.";
            return RedirectToAction("Error", "Errores");
        }
        public async Task<IActionResult> Facturas(int page = 1, int pageSize = 10)
        {
            List<Factura> facturas = await _pedidoServicio.ObtenerFacturasAsync(page, pageSize);
            return Json(facturas);
        }

        [HttpGet]
        public async Task<int> CantidadTotalFacturas()
        {
            int totalFacturas = await _pedidoServicio.ObtenerCantidadTotalFacturasAsync();
            return totalFacturas;
        }

        public async Task<IActionResult> BuscarFactura(int numeroFactura)
        {
            // Lógica para buscar la factura por número de factura
            // Puedes llamar a tu servicio para buscar la factura por número de factura
            List<Factura> facturasEncontradas = await _pedidoServicio.BuscarFacturaPorNumeroAsync(numeroFactura);
            return PartialView("_TablaFacturas", facturasEncontradas);
        }
        public async Task<IActionResult> VisualizarPedido(string estado)
        {
            List<Factura> facturasEncontradas = null;
            // Aquí puedes usar el valor de "estado" para tomar decisiones en tu lógica de negocio
            if (estado == "Proceso")
            {
                facturasEncontradas = await _pedidoServicio.VisualizarPedido(estado);
            }
            else if (estado == "Cerrado")
            {
               facturasEncontradas = await _pedidoServicio.VisualizarPedido(estado);
            }
            else
            {
                // Hacer algo si el estado no es reconocido
            }

            return View("PedidoVisualizado", facturasEncontradas);
        }
        public IActionResult PedidoVisualizado()
        {
            return View();
        }

        public async Task<IActionResult> VerPedidoPorId(int id)
        {
            List<Pedidos> pedidos = await _pedidoServicio.VisualizarPedidoPorId(id);

            if (pedidos == null || pedidos.Count == 0)
            {
                return NotFound();
            }

            if (pedidos.Count == 1)
            {
                // Si solo hay un pedido, mostrar la vista VerPedido para ese pedido
                return View("VerPedido", new List<Pedidos> { pedidos.First() });
            }
            else
            {
                // Si hay múltiples pedidos, mostrar la vista VerPedidos para la lista de pedidos
                return View("VerPedido", pedidos);
            }
        }
        public IActionResult FormGanancia()
        {
            return View("Ganancia");
        }
        public IActionResult AgregarGanancia()
        {
            DateTime fecha = DateTime.Now;
            decimal totalVneto = _pedidoServicio.SumarNetoDelDia(fecha);
            decimal totalVventa = _pedidoServicio.SumarVVentaDelDia(fecha);
            decimal totalCompras = _pedidoServicio.SumarCompraTotal(fecha);
            var cedulaClaim = User.FindFirst("Cedula");
            if (cedulaClaim != null && int.TryParse(cedulaClaim.Value, out int cedula))
            {
                int? buscarSede = _pedidoServicio.BuscarIdSedePorCedula(cedula);
                int? buscarIdPDV = _pedidoServicio.BuscarIdPDVPorIdSede(buscarSede);
                var modeloGanancia = new GananciaViewModel
                {
                    TotalVneto = totalVneto,
                    TotalVventa = totalVventa,
                    IdPDV = buscarIdPDV,
                    TotalCompras = totalCompras
                };
                return View(modeloGanancia);
            }
            else
            {
                var mensaje = "El claim 'Cedula' no existe o la conversión falló.";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }
            
        }
        [HttpPost]
        public async Task<IActionResult> InsertarVentas(int ventaMakrotecno, int netoMakrotecno, int ventaRecarga, int ventaEfectivo, int ventaBancaria, float valorCompras)
        {
            int idCompany = 1;
            if(idCompany == 1)
            {
                int ventaTotal = ventaEfectivo + ventaBancaria;//22000
                int ventaTienda = ventaTotal - ventaMakrotecno - ventaRecarga;//9999
                int gananciaTeresa = (int)(ventaTienda * 0.15);
                // Fase 2
                await _pedidoServicio.VentaInsertada(ventaEfectivo, ventaMakrotecno, netoMakrotecno, ventaRecarga, ventaTienda, ventaBancaria);
                // Fase 3
                int gananciaMakrotecno = ventaMakrotecno - netoMakrotecno;
                int gananciaMaria = (int)(gananciaMakrotecno * 0.20);
                int gananciaVictor = gananciaMakrotecno - gananciaMaria;
                int gananciaRecargas = (int)(ventaRecarga * 0.056);
                int gananciaTotal = gananciaMakrotecno + gananciaMaria + gananciaVictor + gananciaTeresa + gananciaRecargas;
                await _pedidoServicio.GananciaInsertada(gananciaMakrotecno, gananciaMaria, gananciaVictor, gananciaTeresa, gananciaRecargas, gananciaTotal);
                return RedirectToAction("Index");
            } else if (idCompany == 2)
            {
                int ventaTotal = ventaEfectivo + ventaBancaria;
                float comprasTotales = valorCompras;
                int ventaTienda = 0;
                ventaMakrotecno = 0;
                netoMakrotecno = 0;
                ventaRecarga = 0;
                await _pedidoServicio.VentaInsertada(ventaEfectivo, ventaMakrotecno, netoMakrotecno, ventaRecarga, ventaTienda, ventaBancaria);
                int gananciaMakrotecno = 0;
                int gananciaMaria = 0;
                int gananciaVictor = 0;
                int gananciaRecargas = 0;
                int gananciaTeresa = 0;
                int gananciaTotal = (int)(ventaTotal - comprasTotales);
                await _pedidoServicio.GananciaInsertada(gananciaMakrotecno, gananciaMaria, gananciaVictor, gananciaTeresa, gananciaRecargas, gananciaTotal);
                return RedirectToAction("Index");
            }
            else
            {
                var mensaje = "Error en el ID Company";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }
            
        }
        public IActionResult VisualizarGanancia()
        {
            var traerGanancia = _pedidoServicio.TraerGanancias();
            return View(traerGanancia);
        }
        public IActionResult EliminarProdPorId(int id)
        {
            _pedidoServicio.EliminarPedido(id);
            return RedirectToAction("Index");
        }
        public IActionResult VisualizarFactura()
        {
            var traerVentas = _pedidoServicio.TraerVentas();
            return View(traerVentas);
        }
        public IActionResult ProcesoRecogida()
        {
            return View();
        }
    }
}

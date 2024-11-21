using Microsoft.AspNetCore.Mvc;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Plataforma.Servicios.Implementacion;
using Microsoft.IdentityModel.Tokens;

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
        public IActionResult CrearFactura(int cedula_cliente, int cedula_empleado, string estado)
        {
            DateTime fechaVenta = DateTime.Now;
                if(cedula_cliente > 0)
                {
                    if(fechaVenta != DateTime.MinValue)
                    {
                        _pedidoServicio.CrearFactura(cedula_cliente, cedula_empleado, fechaVenta, estado);
                        return RedirectToAction("Index");
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
                            // Crear el objeto ViewModel y asignar los valores
                            var viewModel = new PedidoViewModel
                            {
                                Factura = factura,
                                Productos = productos,
                                EstadoPDV = idPDV
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
            var codigosProductos = _pedidoServicio.ObtenerCodigosProductosAutocompletado(codigo);
            return Json(codigosProductos);
        }
        [HttpGet]
        public async Task<IActionResult> AutocompletarProducto(string codigo)
        {
            var productoInfo = await _pedidoServicio.ObtenerInfoProductoAsync(codigo);

            if (productoInfo != null)
            {
                return Json(productoInfo);
            }

            return NotFound();
        }
        [HttpPost]
        public IActionResult InsertarPedido(int codfact, string cod_producto, int stock, int vneto, int vventa, string tpventa, int idpdv)
        {
            DateTime fechaIngreso = DateTime.Now;
            var cedulaClaim = User.FindFirst("Cedula");
            if (cedulaClaim != null && int.TryParse(cedulaClaim.Value, out int cedula))
            {
                int idPDV = _pedidoServicio.TraerUltimoIDPdv(cedula);
                var estado = _pedidoServicio.ValidarExistenteIdPDV(idPDV, cedula);
                if (estado != null)
                {
                    if (estado == 1)
                    {
                        if (string.IsNullOrEmpty(cod_producto))
                        {
                            var mensaje = "Error: El código del producto no puede ser nulo o vacío.";
                            TempData["ErrorMessage"] = mensaje;
                            return RedirectToAction("Error", "Errores");
                        }else
                        {
                            if (stock > 0 && vneto > 0 && vventa > 0 && !string.IsNullOrEmpty(tpventa))
                            {
                                var validarExisProd = _pedidoServicio.GetProdutos(cod_producto);
                                if (validarExisProd != null)
                                {
                                    // Llamada al servicio para insertar el pedido en la base de datos
                                    foreach (var producto in validarExisProd)
                                    {
                                        if (producto.CantidadProducto <= 0)
                                        {
                                            var mensaje = "Error: El producto no tiene stock para continuar la venta.";
                                            TempData["ErrorMessage"] = mensaje;
                                            return RedirectToAction("Error", "Errores");
                                        }
                                        else
                                        {
                                            _pedidoServicio.InsertarPedido(codfact, cod_producto, stock, vneto, vventa, fechaIngreso, tpventa, idpdv);
                                        }
                                    }
                                    // Redireccionar a la vista Index
                                    return RedirectToAction("Index");
                                }
                                else
                                {
                                    var mensaje = "Error: Producto no existe.";
                                    TempData["ErrorMessage"] = mensaje;
                                    return RedirectToAction("Error", "Errores");
                                }
                            }
                            else
                            {
                                var mensaje = "No puede haber espacios vacios entre campos.";
                                TempData["ErrorMessage"] = mensaje;
                                return RedirectToAction("Error", "Errores");
                            }
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
            Console.WriteLine("Este es el ID del pedido: ", id);
            // Obtener el pedido o la lista de pedidos por su ID
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
        public async Task<IActionResult> FormGanancia()
        {
            return View("Ganancia");
        }
        public async Task<IActionResult> AgregarGanancia()
        {
            DateTime fecha = DateTime.Now;
            decimal traerGananciaXFecha = _pedidoServicio.SumarGananciasDelDia(fecha);
            decimal totalVneto = _pedidoServicio.SumarNetoDelDia(fecha);
            decimal totalVventa = _pedidoServicio.SumarVVentaDelDia(fecha);
            ViewBag.TotalVneto = totalVneto;
            ViewBag.TotalVventa = totalVventa;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> InsertarVentas(int ventaMakrotecno, int netoMakrotecno, int ventaRecarga, int ventaTotal, int ventapasivos)
        {
            // Fase 1
            if (ventapasivos > 0)
            {
                ventaTotal = ventaTotal - ventapasivos;
            }
            int ventaTienda = ventaTotal - ventaMakrotecno - ventaRecarga;
            // Fase 2
            int id_venta = await _pedidoServicio.VentaInsertada(ventaTotal, ventaMakrotecno, netoMakrotecno, ventaRecarga, ventaTienda, ventapasivos);
            // Fase 3
            int gananciaMakrotecno = ventaMakrotecno - netoMakrotecno;
            Console.WriteLine(ventaMakrotecno);
            int gananciaMaria = (int)(gananciaMakrotecno * 0.20);
            //Console.WriteLine(gananciaMaria);
            int gananciaVictor = gananciaMakrotecno - gananciaMaria;
            int gananciaTeresa = (int)(ventaTienda * 0.15);
            int gananciaRecargas = (int)(ventaRecarga * 0.056);
            int gananciaTotal = gananciaMakrotecno + gananciaMaria + gananciaVictor + gananciaTeresa + gananciaRecargas;
            await _pedidoServicio.GananciaInsertada(gananciaMakrotecno, gananciaMaria, gananciaVictor, gananciaTeresa, gananciaRecargas, gananciaTotal);
            return RedirectToAction("Index");
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

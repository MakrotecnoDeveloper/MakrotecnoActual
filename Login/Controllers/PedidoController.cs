using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using Plataforma.Servicios.Implementacion;

namespace Plataforma.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IPedidoService _pedidoServicio;
        private readonly IProductoService _productoservice;
        private readonly ITercerosService _tercerosService;
        private readonly ILogger<HomeController> _logger;
        public PedidoController(IPedidoService pedidoServicio, IProductoService productoservice, ILogger<HomeController> logger, ITercerosService tercerService)
        {
            _pedidoServicio = pedidoServicio;
            _productoservice = productoservice;
            _logger = logger;
            _tercerosService = tercerService;
        }
        public IActionResult Index()
        {
            _pedidoServicio.ActualizarEstadoFacturas();
            var facturas = _pedidoServicio.ObtenerFacturasFechaDescendente();
            return View(facturas);
        }
        [HttpGet]
        public async Task<IActionResult> CrearVenta() 
        {
            var model = new OrdenesServicioViewModel
            {
                Clientes = await _tercerosService.ObtenerClientes(),
                MetodoPagos = await _tercerosService.ObtenerMetodosPago()
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> CrearVenta(Ventas venta)
        {
            venta.FechaVenta = DateTime.Now;
            venta.EstadoVenta = "Pendiente"; // por defecto
            venta.Total = 0;

            var creada = await _pedidoServicio.CrearVentaAsync(venta);

            if (!creada)
            {
                TempData["ErrorMessage"] = "No se pudo crear la venta.";
                return RedirectToAction("CrearVenta");
            }

            // Redirigir al detalle para agregar productos
            return RedirectToAction("AgregarProductoAVenta", new { idVenta = venta.IdVenta });
        }
        public async Task<IActionResult> ListaVentas()
        {
            var ventas = await _pedidoServicio.ObtenerTodasLasVentasAsync(); // este método trae las ventas
            return View(ventas);
        }
        [HttpGet]
        public async Task<IActionResult> DetalleVenta(int idVenta)
        {
            var venta = await _pedidoServicio.ObtenerVentaConPedidos(idVenta);

            // Si no existe la venta en la base de datos, ahí sí mostramos error
            if (venta == null)
            {
                TempData["ErrorMessage"] = "La venta no existe.";
                return RedirectToAction("ListaVentas");
            }

            // Si la venta existe pero no tiene pedidos, redirige a agregar productos
            if (venta.Pedidos == null || !venta.Pedidos.Any())
            {
                TempData["InfoMessage"] = "Agrega productos a esta venta.";
                return RedirectToAction("AgregarProductoAVenta", new { idVenta = idVenta });
            }
            return View(venta);
        }
        [HttpGet]
        public async Task<IActionResult> BuscarProductoPorCodigo(string codigo)
        {
            var resultado = await _pedidoServicio.BuscarProductoPorCodigoAsync(codigo);

            if (resultado == null)
                return NotFound();

            return Json(new
            {
                valorVenta = resultado.Value.valorVenta,
                valorNeto = resultado.Value.valorNeto
            });
        }

        [HttpGet]
        public IActionResult AgregarProductoAVenta(int idVenta)
        {
            var viewModel = new PedidosViewModel
            {
                IdVenta = idVenta,
                Productos = new List<Pedidos>
        {
            new Pedidos() // Inicializa con una fila vacía
        }
            };

            return View(viewModel); // ✅ Ahora el modelo coincide con la vista
        }

        [HttpPost]
        public async Task<IActionResult> AgregarMultiplesProductosAVenta(PedidosViewModel model)
        {
            //Validar cantidad antes de hacer la venta.. (ya se hizo y valida si es menor o igual a 0)
            try
            {
                foreach (var producto in model.Productos)
                {
                    var codigo = producto.Codigo.ToString();
                    var cantidadActual = await _pedidoServicio.ObtenerCantidadProductoActual(codigo);
                    //Validar cantidad es mayor al stock.. (pendiente mañana)
                    if (cantidadActual < producto.Stock)
                    {
                        TempData["ErrorMessage"] = $"Error: El producto '{producto.Codigo}' no tiene el stock para la venta.";
                        return RedirectToAction("Error", "Errores");
                    } 
                }
                await _pedidoServicio.GuardarPedidosAsync(model.Productos, model.IdVenta, User);
                return RedirectToAction("DetalleVenta", new { idVenta = model.IdVenta });

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al guardar los productos: {ex.Message}";
                return View(model);
            }
        }
        public IActionResult CrearFactura()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CrearFactura(int idVenta, int Iva)
        {
            var venta = await _pedidoServicio.ObtenerVentaConPedidos(idVenta);
            if (venta == null || venta.Pedidos.Count == 0)
            {
                TempData["ErrorMessage"] = "No se encontró la venta o no tiene productos.";
                return RedirectToAction("AgregarFactura");
            }
            //decimal iva = subtotal * 0.19M;
            decimal total = (decimal)venta.Pedidos.Sum(p => p.SubTotal);
            decimal totalIva = ((total * Iva) / 100);
            if (totalIva > 0)
            {
                total = totalIva + total;
            }
            await _pedidoServicio.GuardarVentaActualizada(venta, total);

            Factura factura = new Factura
            {
                NumeroFactura = await _pedidoServicio.GenerarConsecutivoFactura(),
                FechaEmision = DateTime.Now,
                IdVenta = idVenta,
                SubTotal = total,
                IVA = totalIva,
                Total = total,
                EstadoFactura = "Emitida"
            };

            await _pedidoServicio.GuardarFacturaAsync(factura);

            await _pedidoServicio.ActualizarEstadoVentaAsync(idVenta, "Finalizado");

            return RedirectToAction("ListaFacturas");
        }
        public async Task<IActionResult> ListaFacturas()
        {
            var facturas = await _pedidoServicio.ObtenerFacturasConVentaCliente();
            return View(facturas);
        }
        public async Task<IActionResult> DetalleFactura(int idFactura)
        {
            var factura = await _pedidoServicio.ObtenerFacturaConDetalle(idFactura);
            if (factura == null)
                return NotFound();

            return View(factura);
        }
        [HttpPost]
        public async Task<IActionResult> AnularFactura(int idFactura)
        {
            bool result = await _pedidoServicio.AnularFacturaAsync(idFactura);
            if (!result)
                TempData["ErrorMessage"] = "No se pudo anular la factura.";

            return RedirectToAction("ListaFacturas");
        }
        [HttpPost]
        public async Task<IActionResult> EliminarFactura(int idFactura)
        {
            bool eliminado = await _pedidoServicio.EliminarFacturaAsync(idFactura);
            if (!eliminado)
                TempData["ErrorMessage"] = "No se pudo eliminar la factura.";

            return RedirectToAction("ListaFacturas");
        }
        [HttpPost]
        public async Task<IActionResult> CambiarEstadoVenta(int idVenta, string nuevoEstado)
        {
            var venta = await _pedidoServicio.ObtenerVentaPorIdAsync(idVenta);
            if (venta == null)
            {
                TempData["ErrorMessage"] = "La venta no existe.";
                return RedirectToAction("ListaVentas");
            }

            venta.EstadoVenta = nuevoEstado;
            await _pedidoServicio.ActualizarVentaAsync(venta);

            TempData["SuccessMessage"] = $"La venta ha sido {nuevoEstado.ToLower()} exitosamente.";
            return RedirectToAction("DetalleVenta", new { idVenta = idVenta });
        }
        [HttpGet]
        public async Task<IActionResult> BuscarFacturaPorId(int IdFactura)
        {
            var factura = await _pedidoServicio.ObtenerFacturaConAdicionesAsync(IdFactura);
            if (factura == null)
                return PartialView("_FacturaNoEncontrada");

            var rol = User.IsInRole("Administrador") ? "Admin" : "User";
            ViewBag.Rol = rol;
            return PartialView("_FacturaParcial", factura);
        }

        [HttpGet]
        public IActionResult FormularioAdicion(int IdFactura)
        {
            return PartialView("_FormularioAdicion", IdFactura);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarAdicion(int IdFactura, decimal Valor, string Descripcion, string EstadoAdicion)
        {
            var cedulaClaim = User.FindFirst("Cedula")?.Value;
            if (int.TryParse(cedulaClaim, out int cedulaEmpleado))
            {
                var resultado = await _pedidoServicio.AgregarAdicionFacturaAsync(IdFactura, Valor, Descripcion, cedulaEmpleado, EstadoAdicion);
                return Json(new { success = resultado });
            }
            return Json(new { success = false });
        }
        public IActionResult VerConceptos()
        {
            //var facturas = _pedidoServicio.ObtenerFacturasFechaDescendente();
            var traerConceptos = _pedidoServicio.ObtenerConceptosCompletos();
            return View(traerConceptos);
        }
        [HttpGet]
        public async Task<IActionResult> BuscarProductoPorCodigoVenta(string codigo)
        {
            //Console.WriteLine("Me oprimiste aca" + codigo);
            var productos = await _pedidoServicio.BuscarProductosPorCodigo(codigo);
            var resultados = productos.Select(p => new {
                label = $"{p.ProductoId} - {p.Producto.NombreProducto}",
                value = p.ProductoId,
                valorNeto = p.Producto.ValorVentaProducto,
                valorVenta = p.Producto.ValorVentaProducto
            });

            return Json(resultados);
        }
    }
}

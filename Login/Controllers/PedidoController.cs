using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using Plataforma.Servicios.Implementacion;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Plataforma.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IPedidoService _pedidoServicio;
        private readonly IProductoService _productoservice;
        public PedidoController(IPedidoService pedidoServicio, IProductoService productoservice)
        {
            _pedidoServicio = pedidoServicio;
            _productoservice = productoservice;
        }
        public IActionResult Index()
        {
            _pedidoServicio.ActualizarEstadoFacturas();
            var facturas = _pedidoServicio.ObtenerFacturas();
            return View(facturas);
        }
        public IActionResult AgregarFactura()
        {
            var productos = _pedidoServicio.ObtenerFacturas();
            return View(productos);
        }
        [HttpPost]
        public IActionResult CrearFactura(int cedula_cliente, int cedula_empleado, DateTime fechaVenta, string estado)
        {
            try
            {
                _pedidoServicio.CrearFactura(cedula_cliente, cedula_empleado, fechaVenta, estado);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Manejar la excepción, por ejemplo, podrías devolver una vista de error con un mensaje personalizado.
                return View("Error", ex.Message);
            }
        }
        [HttpGet]
        public IActionResult CrearPedido(int id)
        {
            var factura = _pedidoServicio.BuscarFacturaPorId(id);
            if (factura != null)
            {
                var productos = _productoservice.ObtenerProductos();

                // Crear el objeto ViewModel y asignar los valores
                var viewModel = new PedidoViewModel
                {
                    Factura = factura,
                    Productos = productos
                };

                // Pasar el ViewModel a la vista
                return View(viewModel);
            }
            else
            {
                // Factura no encontrada, mostrar un mensaje de error
                ViewBag.Mensaje = "Factura no encontrada";
                return View("_Mensaje");
            }
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
        public IActionResult InsertarPedido(int codfact, string cod_producto, int stock, int vneto, int vventa, string estado)
        {
            // Llamada al servicio para insertar el pedido en la base de datos
            _pedidoServicio.InsertarPedido(codfact, cod_producto, stock, vneto, vventa, estado);

            // Redireccionar a la vista Index
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
            else if (estado == "Completado")
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
        public async Task<IActionResult> FormGanancia(string estado)
        {
            List<Factura> facturasEncontradas = null;
            if (estado == "Proceso")
            {
                facturasEncontradas = await _pedidoServicio.VisualizarPedido(estado);
            }
            return View("Ganancia", facturasEncontradas);
        }
        public async Task<IActionResult> VerGananciaPorId(int id)
        {
            //Fase 1: ID de la factura
            ViewBag.Id = id;
            //Fase 2: Venta Neto-Venta
            var productosValores = await _pedidoServicio.traerValorProductos(id);
            decimal totalVneto = productosValores.Sum(p => p.valorNeto);
            decimal totalVventa = productosValores.Sum(p => p.valorVenta);
            ViewBag.TotalVneto = totalVneto;
            ViewBag.TotalVventa = totalVventa;
            //Fase 3: Retorno a la vista
            return View("AgregarGanancia");
        }
        [HttpPost]
        public async Task<IActionResult> InsertarVentas(int cod_factura, int ventaMakrotecno, int netoMakrotecno, int ventaRecarga, int ventaTienda, int ventapasivos)
        {
            int ventaTotal = ventaMakrotecno + ventaRecarga + ventaTienda;
            // Fase 1
            if (ventapasivos > 0)
            {
                ventaTotal = ventaTotal - ventapasivos;
            }
            // Fase 2
            int id_venta = await _pedidoServicio.VentaInsertada(cod_factura, ventaTotal, ventaMakrotecno, netoMakrotecno, ventaRecarga, ventaTienda, ventapasivos);
            // Fase 3
            int gananciaMakrotecno = ventaMakrotecno - netoMakrotecno;
            Console.WriteLine(ventaMakrotecno);
            int gananciaMaria = (int)(gananciaMakrotecno * 0.20);
            //Console.WriteLine(gananciaMaria);
            int gananciaVictor = gananciaMakrotecno - gananciaMaria;
            int gananciaTeresa = (int)(ventaTienda * 0.15);
            int gananciaRecargas = (int)(ventaRecarga * 0.056);
            int gananciaTotal = gananciaMakrotecno + gananciaMaria + gananciaVictor + gananciaTeresa + gananciaRecargas;
            await _pedidoServicio.GananciaInsertada(id_venta, gananciaMakrotecno, gananciaMaria, gananciaVictor, gananciaTeresa, gananciaRecargas, gananciaTotal);
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
    }
}

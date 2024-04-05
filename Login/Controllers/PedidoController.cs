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
        public IActionResult InsertarPedido()
        {
            return View();
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
    }
}

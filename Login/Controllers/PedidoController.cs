using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma.Servicios.Contrato;
using Plataforma.Servicios.Implementacion;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Plataforma.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IPedidoService _pedidoServicio;
        public PedidoController(IPedidoService pedidoServicio)
        {
            _pedidoServicio = pedidoServicio;
        }
        public IActionResult Index()
        {
            _pedidoServicio.ActualizarEstadoFacturas();
            var productos = _pedidoServicio.ObtenerFacturas();
            return View(productos);
        }
        public IActionResult AgregarFactura()
        {
            var productos = _pedidoServicio.ObtenerFacturas();
            return View(productos);
        }
        [HttpPost]
        public IActionResult CrearFactura(int cod_factura, int cedula_cliente, int cedula_empleado, DateTime fechaVenta, string estado)
        {
            try
            {
                _pedidoServicio.CrearFactura(cod_factura, cedula_cliente, cedula_empleado, fechaVenta, estado);
                return View("CrearPedido");
            }
            catch (Exception ex)
            {
                // Manejar la excepción, por ejemplo, podrías devolver una vista de error con un mensaje personalizado.
                return View("Error", ex.Message);
            }
        }
        public IActionResult CrearPedido()
        {
            return View();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IPedidoService
    {
        List<Factura> ObtenerFacturas();
        void ActualizarEstadoFacturas();
        IEnumerable<Factura> CrearFactura(int cedula_cliente, int cedula_empleado, DateTime fechaVenta, string estado);
        List<string> ObtenerCodigosProductosAutocompletado(string codigo);
        Task<Producto> ObtenerInfoProductoAsync(string codigoProducto);
        Task<List<Factura>> ObtenerFacturasAsync(int pagina, int pageSize);
        Task<List<Factura>> BuscarFacturaPorNumeroAsync(int numeroFactura);
        Task<int> ObtenerCantidadTotalFacturasAsync();
        Factura BuscarFacturaPorId(int id);
        void InsertarPedido(int codfact, string cod_producto, int stock, int vneto, int vventa, string estado);
        Task<List<Factura>> VisualizarPedido(string estado);
        Task<List<Pedidos>> VisualizarPedidoPorId(int id);
    }
}
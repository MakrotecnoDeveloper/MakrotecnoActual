using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IPedidoService
    {
        Task<bool> CrearVentaAsync(Ventas venta);
        Task<List<Ventas>> ObtenerTodasLasVentasAsync();
        Task<bool> AgregarPedidoAVentaAsync(Pedidos pedido);
        Task GuardarPedidoAsync(Pedidos pedido);
        void ActualizarEstadoFacturas();
        List<Factura> ObtenerFacturasFechaDescendente();
        Task<Ventas> ObtenerVentaConPedidos(int idVenta);
        Task<int> GenerarConsecutivoFactura();
        Task GuardarFacturaAsync(Factura factura);
        Task<List<Factura>> ObtenerFacturasConVentaCliente();
        Task<Factura> ObtenerFacturaConDetalle(int idFactura);
        Task<bool> AnularFacturaAsync(int idFactura);
        Task<bool> EliminarFacturaAsync(int idFactura);
        Task<Ventas> ObtenerVentaPorIdAsync(int id);
        Task ActualizarVentaAsync(Ventas venta);
    }
}
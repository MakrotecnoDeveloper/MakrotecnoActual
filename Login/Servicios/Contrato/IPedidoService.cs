using Plataforma.Models;
using System.Security.Claims;
namespace Plataforma.Servicios.Contrato
{
    public interface IPedidoService
    {
        Task<List<MetodoPagos>> TraerMetodosPagoDisponibles(int metodoPago);
        Task<bool> CrearVentaAsync(Ventas venta);
        Task<Ventas?> ObtenerVentaConDetalleAsync(int idVenta);
        Task<List<Ventas>> ObtenerTodasLasVentasAsync();
        Task<(decimal? valorVenta, decimal? valorNeto)?> BuscarProductoPorCodigoAsync(string codigo);
        Task<decimal> ObtenerCantidadProductoActual(string codigo);
        Task GuardarPedidosAsync(List<Pedidos> pedidos, int idVenta, ClaimsPrincipal usuario);
        //Task GuardarPedidosAsync(List<Pedidos> pedidos);
        Task<bool> AgregarPedidoAVentaAsync(Pedidos pedido);
        void ActualizarEstadoFacturas();
        List<Factura> ObtenerFacturasFechaDescendente();
        Task<DetalleVentaViewModel> ObtenerVentaConPedidos(int idVenta);
        Task GuardarVentaActualizada(Ventas venta, decimal total);
        Task GuardarFacturaAsync(Factura factura);
        Task<List<Factura>> ObtenerFacturasConVentaCliente();
        Task<DetalleFacturaViewModel> ObtenerFacturaConDetalle(int idFactura, string empresaId);
        Task<bool> AnularFacturaAsync(int idFactura);
        Task<bool> EliminarFacturaAsync(int idFactura);
        Task<Ventas> ObtenerVentaPorIdAsync(int id);
        Task ActualizarVentaAsync(Ventas venta);
        Task ActualizarEstadoVentaAsync(int idVenta, string nuevoEstado);
        Task<Factura> ObtenerFacturaConAdicionesAsync(int IdFactura);
        Task<bool> AgregarAdicionFacturaAsync(int IdFactura, decimal valor, string descripcion, int cedulaEmpleado, string EstadoAdicion);
        List<AdicionFactura> ObtenerConceptosCompletos();
        List<InventarioSede> ValidarProductoPorCodigo(string Codigo);
        Task<List<InventarioSede>> BuscarProductosPorCodigo(string codigo);
        Task<bool> FacturarConPagosAsync(
            int idVenta,
            int idCliente,
            string metodoPagoFinal,
            decimal montoEfectivo,
            decimal montoTransferencia,
            decimal montoCredito,
            DateTime? fechaVencimientoCredito
        );
        Task<bool> EmitirFacturaAsync(int idVenta, string metodoPago);
        Task CambiarEstadoVentaAsync(int idVenta, string nuevoEstado);
        Task<List<ProductoVentaDto>> BuscarProductosPorNombreVentaAsync(string texto, ClaimsPrincipal usuario);
    }
}
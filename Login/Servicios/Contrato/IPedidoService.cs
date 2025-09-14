using Plataforma.Models;
using System.Security.Claims;
namespace Plataforma.Servicios.Contrato
{
    public interface IPedidoService
    {
        Task<bool> CrearVentaAsync(Ventas venta);
        Task<List<Ventas>> ObtenerTodasLasVentasAsync();
        Task<(decimal? valorVenta, decimal? valorNeto)?> BuscarProductoPorCodigoAsync(string codigo);
        Task<decimal> ObtenerCantidadProductoActual(string codigo);
        Task GuardarPedidosAsync(List<Pedidos> pedidos, int idVenta, ClaimsPrincipal usuario);
        //Task GuardarPedidosAsync(List<Pedidos> pedidos);
        Task<bool> AgregarPedidoAVentaAsync(Pedidos pedido);
        void ActualizarEstadoFacturas();
        List<Factura> ObtenerFacturasFechaDescendente();
        Task<Ventas> ObtenerVentaConPedidos(int idVenta);
        Task<int> GenerarConsecutivoFactura();
        Task GuardarVentaActualizada(Ventas venta, decimal total);
        Task GuardarFacturaAsync(Factura factura);
        Task<List<Factura>> ObtenerFacturasConVentaCliente();
        Task<Factura> ObtenerFacturaConDetalle(int idFactura);
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
    }
}
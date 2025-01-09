using Mysqlx.Cursor;
using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IPedidoService
    {
        List<Producto> GetProdutos(string cod_producto);
        List<Factura> ObtenerFacturasFechaDescendente();
        List<Factura> ObtenerFacturas();
        void ActualizarEstadoFacturas();
        int? BuscarIdSedePorCedula(int cedula);
        int? BuscarIdPDVPorIdSede(int? buscarIdSede);
        IEnumerable<Factura> CrearFactura(int cedula_cliente, int cedula_empleado, DateTime fechaVenta, string estado, string tpfactura);
        List<string> ObtenerCodigosProductosAutocompletado(string codigo);
        Task<Producto> ObtenerInfoProductoAsync(string codigoProducto);
        Task<List<Factura>> ObtenerFacturasAsync(int pagina, int pageSize);
        Task<List<Factura>> BuscarFacturaPorNumeroAsync(int numeroFactura);
        Task<int> ObtenerCantidadTotalFacturasAsync();
        Factura BuscarFacturaPorId(int id);
        void InsertarPedido(int codfact, string cod_producto, decimal stock, int vneto, int vventa, DateTime fechaIngreso, string tpventa, int idpdv);
        Task<List<Factura>> VisualizarPedido(string estado);
        Task<List<Pedidos>> VisualizarPedidoPorId(int id);
        Task<List<Pedidos>> traerValorProductos(int id);
        Task<int> VentaInsertada(int ventaTotal, int ventaMakrotecno, int netoMakrotecno, int ventaRecarga, int ventaTienda, int ventapasivos);
        Task GananciaInsertada(int gananciaMakrotecno, int gananciaMaria, int gananciaVictor, int gananciaTeresa, int gananciaRecargas, int gananciaTotal);
        List<Ganancias> TraerGanancias();
        void EliminarPedido(int id);
        List<Ventas> TraerVentas();
        int TraerUltimoIDPdv(int cedulaEmpleado);
        int? ValidarExistenteIdPDV(int idPDV, int cedula);
        decimal SumarNetoDelDia(DateTime fecha);
        decimal SumarVVentaDelDia(DateTime fecha);
        decimal SumarCompraTotal(DateTime fecha);
        string? ObtenerNombreCliente(int cedulaCliente);
    }
}
using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IComprasService
    {
        Task<List<Proveedores>> ObtenerProveedoresAsync();
        Task<Proveedores> BuscarProveedorPorIdAsync(int idProveedor);
        Task<List<Producto>> BuscarProductosPorCodigoAsync(string term);
        Task InsertarCompraAsync(Compras compra, List<DetalleCompra> detalles);
        Task<List<Compras>> ObtenerComprasAsync();
    }
}

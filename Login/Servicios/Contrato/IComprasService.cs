using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IComprasService
    {
        Task<List<Proveedores>> ObtenerProveedoresAsync();
        Task<Proveedores> BuscarProveedorPorIdAsync(int idProveedor);
        Task<List<Producto>> BuscarProductosPorCodigoAsync(string codigo);
        Task<bool> InsertarCompraAsync(CompraViewModel model);
        Task<List<Compras>> ObtenerComprasAsync();
    }
}

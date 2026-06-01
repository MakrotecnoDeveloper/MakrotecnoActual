using MakroTecno.Models;
using MakroTecno.ViewModels.Compras;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        /*Inicio Agregar imagen de factura y otros datos*/
        Task<List<FacturaCompra>> ObtenerFacturasCompraAsync();

        Task<FacturaCompra?> ObtenerFacturaCompraPorIdAsync(int id);

        Task CrearFacturaCompraAsync(FacturaCompraViewModel model);

        Task ActualizarFacturaCompraAsync(FacturaCompraViewModel model);

        Task InactivarFacturaCompraAsync(int id, string? motivoInactivacion = null, string? usuario = null);
        Task<List<SelectListItem>> ObtenerProveedoresSelectAsync();
        /*Fin*/
    }
}

using Plataforma.Models;
using System.Linq.Expressions;
namespace Plataforma.Servicios.Contrato
{
    public interface IProductoService
    {
       // Task<bool> AgregarProductoAsync(string id_empresa, string codigo, string descripcion, float valor_neto, float valor_unitario, decimal stock, int categorias);
        Task<bool> AgregarProductoAsync(Producto model);
        List<Producto> ObtenerProductos();
        List<CategoriaProductos> ObtenerCategoriaProductos(int IdServicio);
        List<Producto> BuscarProductos(string searchTerm, int categoriaTerm);
        List<Producto> SinStock(string searchTerm, int categoriaTerm);
        List<Producto> BuscarProSinStock(string searchTerm, int categoriaTerm);
        Task<bool> AgregarStockAsync(string idProducto, int cantidad);
        IEnumerable<Producto> EditarStock(string id, int cantidad, int opcion);
        void EditarProducto(string codigo, string nombreProducto, float valorNeto, float valorVenta, int valorUnidad, int cantidad, int categoria, string idEmpresa, int estado);
        void EliminarProducto(string id);
        List<Producto> TraerProductosXCategoria(int categoria);
        ProveedorProductosViewModel TraerProveedorProductos(int cedula);
        void HistoricoCompra(int codfact, string cod_producto, decimal stock, string? UnidadMedida, int vneto, decimal vtotal, DateTime fechaIngreso, string tpventa, int idpdv);

        //Nuevos metodos
        Task<List<Producto>> FindListByFunction(Expression<Func<Producto, bool>> lambda);
    }
}

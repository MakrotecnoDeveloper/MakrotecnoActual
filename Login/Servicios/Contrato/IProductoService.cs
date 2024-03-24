//establece un contrato que define las operaciones necesarias para interactuar con usuarios en una aplicación
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IProductoService
    {
        Task<bool> AgregarProductoAsync(string id_empresa, string codigo, string descripcion, float valor_neto, float valor_unitario, int stock, string categorias);
        List<Producto> ObtenerProductos();
        List<Producto> BuscarProductos(string searchTerm, string categoriaTerm);
        Task<bool> AgregarStockAsync(string idProducto, int cantidad);
        IEnumerable<Producto> EditarStock(string id, int cantidad, int opcion);
    }
}

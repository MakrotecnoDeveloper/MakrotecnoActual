using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Servicios.Implementacion
{
    public class ProductoService : IProductoService
    {
        //variable de solo lectura para referenciar la base de datos
        private readonly BaseAdmContext _dbContext;
        public ProductoService(BaseAdmContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task<bool> AgregarProductoAsync(string id_empresa, string codigo, string descripcion, int valor_neto, int valor_unitario, int stock, string categorias)
        {
            try
            {
                // Crear un nuevo objeto Producto con los parámetros proporcionados
                var nuevoProducto = new Producto
                {
                    cod_producto = codigo,
                    nombreProducto = descripcion,
                    cantidadProducto = stock,
                    valorNetoProducto = valor_neto,
                    valorVentaProducto = valor_unitario,
                    id_empresa = id_empresa,
                    categoria = categorias
                };

                // Agregar el nuevo producto al DbContext y guardar los cambios en la base de datos
                _dbContext.Productos.Add(nuevoProducto);
                _dbContext.SaveChanges();

                // Devolver true si la operación fue exitosa
                return Task.FromResult(true);
            }
            catch (Exception)
            {
                // Manejar cualquier error y devolver false si la operación falla
                return Task.FromResult(false);
            }
        }
        public List<Producto> ObtenerProductos()
        {
            return _dbContext.Productos.ToList();
        }
    }
}

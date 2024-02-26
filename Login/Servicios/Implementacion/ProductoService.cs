using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        public List<Producto> ObtenerProductos()
        {
            return _dbContext.Productos.ToList();
        }
        public Task<bool> AgregarProductoAsync(string id_empresa, string codigo, string descripcion, int valor_neto, int valor_unitario, int stock, string categorias)
        {
            try
            {
                // Crear un nuevo objeto Producto con los parámetros proporcionados
                var nuevoProducto = new Producto
                {
                    Cod_Producto = codigo,
                    NombreProducto = descripcion,
                    CantidadProducto = stock,
                    ValorNetoProducto = valor_neto,
                    ValorVentaProducto = valor_unitario,
                    ID_Empresa = id_empresa,
                    Categoria = categorias
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
        public async Task<IEnumerable<Producto>> BuscarProductosAsync(string id_empresa, string codigo, string descripcion, float valorNeto, float valorVenta, int stock, string categoria)
        {
            var query = _dbContext.Productos.AsQueryable();

            // Aplica los criterios de búsqueda según sea necesario
            if (!string.IsNullOrEmpty(id_empresa))
            {
                query = query.Where(p => p.ID_Empresa == id_empresa);
            }

            // Agrega lógica para otros criterios de búsqueda...

            var resultados = await query.ToListAsync();
            return resultados;
        }
    }
}

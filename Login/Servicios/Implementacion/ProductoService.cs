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
        public Task<bool> AgregarProductoAsync(string id_empresa, string codigo, string descripcion, float valor_neto, float valor_unitario, int stock, string categorias)
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
        public List<Producto> BuscarProductos(string searchTerm, string categoriaTerm)
        {
            
            // Lógica para buscar productos por el nombre o la categoría
            if (!string.IsNullOrEmpty(searchTerm))
            {
                Console.WriteLine("searchTerm");
                var consulta = _dbContext.Productos.Where(p => p.Cod_Producto == searchTerm).ToList();
                return consulta;
            }
            else if (!string.IsNullOrEmpty(categoriaTerm))
            {
                Console.WriteLine("Categoria");
                var consulta = _dbContext.Productos.Where(p => p.Categoria == categoriaTerm).ToList();
                return consulta;
            }
            else
            {
                // Ambos términos están vacíos, puedes manejarlo según tus necesidades
                return new List<Producto>();
            }
        }
        public async Task<bool> AgregarStockAsync(string idProducto, int cantidad)
        {
            try
            {
                // Buscar el producto en la base de datos
                var producto = await _dbContext.Productos.FindAsync(idProducto);

                if (producto != null)
                {
                    // Actualizar el stock del producto
                    producto.CantidadProducto += cantidad;

                    // Guardar los cambios en la base de datos
                    await _dbContext.SaveChangesAsync();

                    return true; // Devolver true si la actualización fue exitosa
                }
                else
                {
                    return false; // Devolver false si no se encontró el producto
                }
            }
            catch (Exception)
            {
                // Manejar cualquier error que ocurra durante la actualización del stock
                return false; // Devolver false en caso de error
            }
        }
        public IEnumerable<Producto> EditarStock(string id, int cantidad, int opcion)
        {
            // Lógica para editar el stock del producto
            var producto = _dbContext.Productos.FirstOrDefault(p => p.Cod_Producto == id);
            if (producto != null)
            {
                if (opcion == 1)
                {
                    if(cantidad == 0)
                    {
                        Console.WriteLine("La cantidad no puede ser 0");
                    }else
                    {
                        // Agregar stock al producto
                        producto.CantidadProducto += cantidad;
                    }
                    
                }
                else if (opcion == 2)
                {
                    // Verificar si hay suficiente stock antes de eliminar
                    if (producto.CantidadProducto >= cantidad)
                    {
                        if (cantidad == 0)
                        {
                            Console.WriteLine("La cantidad no puede ser 0");
                        }
                        else
                        {
                            // Eliminar la cantidad especificada de stock del producto
                            producto.CantidadProducto -= cantidad;
                        }
                    }
                    else
                    {
                        // No hay suficiente stock para eliminar
                        // Lanza una excepción indicando que la cantidad a eliminar es mayor que el stock actual
                        Console.WriteLine("La cantidad a eliminar es mayor que el stock actual del producto.");
                    }
                }
                // Guardar los cambios en la base de datos
                _dbContext.SaveChanges();
            }
            return _dbContext.Productos.ToList();
        }
        //a
        public void EditarProducto(string codigo, string nombreProducto, float valorNeto, float valorVenta, int cantidad, string categoria, string idEmpresa)
        {

            if (codigo == null || nombreProducto == null || categoria == null || idEmpresa == null || valorNeto < 0 || valorVenta < 0 || cantidad < 0)
            {
                Console.WriteLine("Error: Todos los campos deben tener un valor. No se permiten valores nulos.");
                return;
            }

            var producto = _dbContext.Productos.FirstOrDefault(p => p.Cod_Producto == codigo);
            //Console.WriteLine("El ID de la empresa es: " + idEmpresa);
            if (producto != null)
            {
                producto.Cod_Producto = codigo;
                producto.NombreProducto = nombreProducto;
                producto.CantidadProducto = cantidad;
                producto.ValorNetoProducto = valorNeto;
                producto.ValorVentaProducto = valorVenta;
                producto.ID_Empresa = idEmpresa;
                producto.Categoria = categoria;
                try
                {
                    _dbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al guardar los cambios en la base de datos: " + ex.Message);
                    // Puedes agregar un código adicional aquí para manejar el error, como registrar el error en un archivo de registro, notificar al usuario, etc.
                }
            }
            else
            {
                Console.WriteLine("El producto no existe");
                // Puedes agregar un código adicional aquí si necesitas manejar el caso en que el producto no exista
            }
        }
        public void EliminarProducto(string id)
        {
            var producto = _dbContext.Productos.FirstOrDefault(p => p.Cod_Producto == id);
            if (producto != null)
            {
                try
                {
                    // 3. Eliminar el producto.
                    _dbContext.Productos.Remove(producto);

                    // 4. Guardar los cambios en la base de datos.
                    _dbContext.SaveChanges();

                    Console.WriteLine("Producto eliminado exitosamente.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al eliminar el producto: " + ex.Message);
                    // Puedes agregar un código adicional aquí para manejar el error, como registrar el error en un archivo de registro, notificar al usuario, etc.
                }
            }
            else
            {
                Console.WriteLine("El producto no existe.");
                // Puedes agregar un código adicional aquí si necesitas manejar el caso en que el producto no exista
            }
        }

        //a

    }
}

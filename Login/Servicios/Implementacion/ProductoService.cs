using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Servicios.Implementacion
{
    public class ProductoService : IProductoService
    {
        //variable de solo lectura para referenciar la base de datos
        private readonly BaseAdmContext _dbContext;
        private readonly ILogger<ProductoService> _logger;
        public ProductoService(BaseAdmContext dbContext, IConfiguration configuration, ILogger<ProductoService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public List<Producto> ObtenerProductos()
        {
            var productosTraidosDB = _dbContext.Productos
                .Where(p => p.Estado == 1)
                .ToList();
            return productosTraidosDB;
        }
        public List<CategoriaProductos> ObtenerCategoriaProductos(int IdServicio)
        {
            var categorias = _dbContext.CategoriaProductos
                    .Where(c => c.IdServicio == IdServicio) // Filtrar por IdServicio
                    .ToList();
                return categorias;
        }
       // public Task<bool> AgregarProductoAsync(string id_empresa, string codigo, string descripcion, float valor_neto, float valor_unitario, decimal stock, int categorias)
        public Task<bool> AgregarProductoAsync(Producto model)
        {
            try
            {
                int valorUnidad = 0;
                int estado = 1;
                string Ubicacion = "Web";
                /*
                // Crear un nuevo objeto Producto con los parámetros proporcionados
                var nuevoProducto = new Producto
                {
                    Cod_Producto = codigo,
                    NombreProducto = descripcion,
                    CantidadProducto = stock,
                    ValorNetoProducto = valor_neto,
                    ValorVentaProducto = valor_unitario,
                    ValorUnidad = valorUnidad,
                    ID_Empresa = id_empresa,
                    IdCatepro = categorias,
                    Estado = estado,
                    Ubicacion = Ubicacion
                };
                */

                // Agregar el nuevo producto al DbContext y guardar los cambios en la base de datos
                //_dbContext.Productos.Add(nuevoProducto);
                model.ValorUnidad = valorUnidad;
                model.Estado = estado;
                model.Ubicacion = Ubicacion;

                _dbContext.Productos.Add(model);
                _dbContext.SaveChanges();

                // Devolver true si la operación fue exitosa
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar productos asincronicamente.");
                // Manejar cualquier error y devolver false si la operación falla
                return Task.FromResult(false);
            }
        }
        public List<Producto> BuscarProductos(string searchTerm, int categoriaTerm)
        {
            
            // Lógica para buscar productos por el nombre o la categoría
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var consulta = _dbContext.Productos.Where(p => p.Cod_Producto == searchTerm)
                    .Where(p => p.Estado == 1)
                    .ToList();
                return consulta;
            }
            else if (categoriaTerm > 0)
            {
                var consulta = _dbContext.Productos.Where(p => p.IdCatepro == categoriaTerm).ToList()
                    .Where(p => p.Estado == 1)
                    .ToList();
                return consulta;
            }
            else
            {
                // Ambos términos están vacíos, puedes manejarlo según tus necesidades
                return new List<Producto>();
            }
        }
        public List<Producto> SinStock(string searchTerm, int categoriaTerm)
        {

            // Lógica para buscar productos por el nombre o la categoría
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var productosSinStock = _dbContext.Productos
                    .Where(p => p.Cod_Producto == searchTerm && p.CantidadProducto == 0 && p.Estado == 1)
                    .ToList();
                return productosSinStock;
            }
            else if (categoriaTerm > 0)
            {
                var productosSinStock = _dbContext.Productos
                    .Where(p => p.IdCatepro == categoriaTerm && p.CantidadProducto == 0 && p.Estado == 1)
                    .ToList();
                return productosSinStock;
            }
            else
            {
                // Ambos términos están vacíos, puedes manejarlo según tus necesidades
                return new List<Producto>();
            }
        }
        public List<Producto> BuscarProSinStock(string searchTerm, int categoriaTerm)
        {

            // Lógica para buscar productos por el nombre o la categoría
            if (!string.IsNullOrEmpty(searchTerm))
            {
                Console.WriteLine("searchTerm");
                var productosProximosSinStock = _dbContext.Productos.Where(p => p.CantidadProducto == 1 && p.Cod_Producto == searchTerm && p.Estado == 1).ToList(); // Suponiendo que "próximos sin stock" se refiere a productos con cantidad menor a 5
                return productosProximosSinStock;
            }
            else if (categoriaTerm > 0)
            {
                Console.WriteLine("Categoria");
                var productosProximosSinStock = _dbContext.Productos.Where(p => p.CantidadProducto == 1 && p.IdCatepro == categoriaTerm && p.Estado == 1).ToList(); // Suponiendo que "próximos sin stock" se refiere a productos con cantidad menor a 5
                return productosProximosSinStock;
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
        public void EditarProducto(string codigo, string nombreProducto, float valorNeto, float valorVenta, int valorUnidad, int cantidad, int categoria, string idEmpresa, int estado)
        {

            if (codigo == null || nombreProducto == null || categoria <= 0 || idEmpresa == null || valorNeto < 0 || valorVenta < 0 || cantidad < 0)
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
                producto.ValorUnidad = valorUnidad;
                producto.ID_Empresa = idEmpresa;
                producto.IdCatepro = categoria;
                producto.Estado = estado;
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

        
        public List<Producto> TraerProductosXCategoria(int categoria)
        {
            var productos = _dbContext.Productos
               .Where(p => p.IdCatepro == categoria && p.Estado == 1)
               .ToList();
            return productos;
        }
        public ProveedorProductosViewModel TraerProveedorProductos(int cedula)
        {
            var consultarProveedor = _dbContext.Proveedores.ToList();
            var consultarProductos = _dbContext.Productos.ToList();
            int facturaReciente = _dbContext.Factura
            .Where(f => f.Cedula == cedula && f.TipoFactura == "Compra")
            .OrderByDescending(f => f.Cod_factura)
            .Select(f => f.Cod_factura) // Seleccionar solo el campo idFactura
            .FirstOrDefault(); // Devuelve 0 si no hay resultados
            var provProdViewModel = new ProveedorProductosViewModel
            {
                Proveedores = consultarProveedor,
                Productos = consultarProductos,
                Cod_Factura = facturaReciente
            };
            return provProdViewModel;
        }
        public void HistoricoCompra(int codfact, string cod_producto, decimal stock, string? UnidadMedida, int vneto, decimal vtotal, DateTime fechaIngreso, string tpventa, int idpdv)
        {
            var producto = _dbContext.Productos.FirstOrDefault(p => p.Cod_Producto == cod_producto);
            if (producto != null)
            {
                _dbContext.SaveChanges();
                var pedido = new HistoricoCompras
                {
                    cod_factura = codfact,
                    Cod_Producto = cod_producto,
                    Stock = stock,
                    UnidadMedida = UnidadMedida,
                    ValorU = vneto,
                    ValorTotal = vtotal,
                    Nit = tpventa,
                    Estado = idpdv,
                    FechaRegistro = fechaIngreso
                };

                _dbContext.HistoricoCompras.Add(pedido);
                _dbContext.SaveChanges();
            }
        }
        
        
    }
}

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
        public Task<bool> AgregarProductoAsync(string id_empresa, string codigo, string descripcion, float valor_neto, float valor_unitario, decimal stock, int categorias)
        {
            try
            {
                int valorUnidad = 0;
                int estado = 1;
                string Ubicacion = "Web";
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

                // Agregar el nuevo producto al DbContext y guardar los cambios en la base de datos
                _dbContext.Productos.Add(nuevoProducto);
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

        //a
        public void InserPlataformaService(int idPlataforma, string descripcion, int valorventa, int valorneto, DateTime fechaInipago, DateTime fechaFinpago, int cantidad, string correo, string contrasena, int cedula, int estado)
        {
            var nuevaPlataforma = new Plataformasuscripcion
            {
                IdPlataforma = idPlataforma,
                Descripcion = descripcion,
                ValorVenta = valorventa,
                ValorNeto = valorneto,
                FechaIniPago = fechaInipago,
                FechaFinPago = fechaFinpago,
                Cantidad = cantidad,
                Correo = correo,
                Contrasena = contrasena,
                CedulaEmpleado = cedula,
                Estado = estado
            };

            // Agregar el nuevo producto al DbContext y guardar los cambios en la base de datos
            _dbContext.Plataformasuscripcion.Add(nuevaPlataforma);
            _dbContext.SaveChanges();
        }
        public List<Plataformas> TraerPlataformasExistentes()
        {
            return _dbContext.Plataformas.ToList();
        }
        public List<Plataformasuscripcion> SuscripcionesActivas()
        {
            return _dbContext.Plataformasuscripcion.ToList();
        }
        public async Task<List<Plataformasuscripcion>> ObtenerSuscripcionesActivas(int plataformaId)
        {
            // Consultar las suscripciones activas para una plataforma específica
            var suscripciones = await _dbContext.Plataformasuscripcion
                .Where(s => s.Estado == 1 && s.IdPlataforma == plataformaId)
                .ToListAsync();

            return suscripciones;
        }
        public async Task<List<ClientePlataformaDTO>> ObtenerDatosSuscripcion(int suscripcionId)
        {
            var datos = await _dbContext.ClientesPlataforma
                .Where(cp => cp.IdPltfSuscripcion == suscripcionId && cp.Estado == 1)
                .Join(_dbContext.Plataformasuscripcion,
                    cp => cp.IdPltfSuscripcion,
                    ps => ps.IdPltfSuscripcion,
                    (cp, ps) => new ClientePlataformaDTO
                    {
                        IdCliente = cp.IdCliPltf,
                        IdClientePlataforma = ps.IdPlataforma,
                        NombreCliente = cp.NombreCliente,
                        CorreoPlataforma = ps.Correo,
                        ClavePlataforma = ps.Contrasena,
                        ClavePerfil = cp.ClavePerfil,
                        Celular = cp.CelularCliente,
                        FechaIni = cp.FechaIniPago,
                        FechaFin = cp.FechaFinPago,
                        Plataforma = ps.IdPlataforma,
                        NombrePlataforma = ps.Descripcion,
                        Estado = cp.Estado,
                        IdPltfSuscripcion = cp.IdPltfSuscripcion
                    })
                .ToListAsync();

            return datos;
        }
        public async Task<List<ClientePlataformaDTO>> ObtenerDatosPlataforma(int suscripcionId)
        {
            var datos = await _dbContext.Plataformasuscripcion
                .Where(ps => ps.IdPltfSuscripcion == suscripcionId && ps.Estado == 1)
                .Select(ps => new ClientePlataformaDTO
                {
                    IdCliente = ps.IdPltfSuscripcion,
                    IdClientePlataforma = ps.IdPlataforma,
                    NombreCliente = ps.Descripcion,
                    CorreoPlataforma = ps.Correo,
                    ClavePlataforma = ps.Contrasena,
                    FechaIni = ps.FechaIniPago,
                    FechaFin = ps.FechaFinPago,
                    Plataforma = ps.Cantidad,
                    Estado = ps.Estado
                })
            .ToListAsync();

            return datos;
        }
        public async Task<bool> EliminarClienteAsync(int idClientePlataforma)
        {
            try
            {
                var cliente = await _dbContext.ClientesPlataforma.FindAsync(idClientePlataforma);
                if (cliente == null)
                {
                    return false; // Cliente no encontrado
                }

                _dbContext.ClientesPlataforma.Remove(cliente);
                await _dbContext.SaveChangesAsync();
                return true; // Eliminado con éxito
            }
            catch (Exception)
            {
                return false; // Manejo de errores
            }
        }
        public void ServicioInsertarVentClientPlataforma(string nombrecliente, string celularcliente, string correo, string contrasena, int idPltfSuscripcion, int cantidad, string ppm, DateTime feciniplat, DateTime fecfinplat, int valorventa, int valorneto, int cedula, int estado, string clave)
        {
            var plataforma = _dbContext.Plataformasuscripcion.SingleOrDefault(p => p.IdPltfSuscripcion == idPltfSuscripcion);
            if (plataforma != null)
            {
                // Si la plataforma existe, actualizar el campo cantidad
                int cantidadActual = plataforma.Cantidad;
                int cantidadNueva = cantidadActual - cantidad;
                if (cantidadNueva < 0)
                {
                    Console.WriteLine("La plataforma no tiene esa cantidad de espacios disponibles");
                }else
                {
                    //agregar cuenta
                    var nuevoVentClientPltf = new ClientesPlataforma
                    {
                        NombreCliente = nombrecliente,
                        CelularCliente = celularcliente,
                        Correo = correo,
                        Clave = contrasena,
                        IdPltfSuscripcion = idPltfSuscripcion,
                        Cantidad = cantidad,
                        Ppm = ppm,
                        FechaIniPago = feciniplat,
                        FechaFinPago = fecfinplat,
                        ValorVenta = valorventa,
                        ValorNeto = valorneto,
                        CedulaEmpleado = cedula,
                        Estado = estado,
                        ClavePerfil = clave,
                    };

                    // Agregar el nuevo producto al DbContext y guardar los cambios en la base de datos
                    _dbContext.ClientesPlataforma.Add(nuevoVentClientPltf);
                    _dbContext.SaveChanges();

                    //cantidad
                    plataforma.Cantidad = cantidadNueva;
                    // Guardar los cambios en la base de datos
                    _dbContext.SaveChanges();
                }
            }
            else
            {
                Console.WriteLine("Plataforma no encontrada.");
            }
        }
        public async Task ActualizarCliente(int id, int estado, int idCliente)
        {
            // Buscar la plataforma en la base de datos
            var plataforma = await _dbContext.Plataformasuscripcion.SingleOrDefaultAsync(p => p.IdPltfSuscripcion == id);
            if (plataforma != null)
            {
                // Si la plataforma existe, actualizar el campo cantidad
                int cantidadActual = plataforma.Cantidad;
                int cantidadReducida = 1; // Este es el valor que quieres reducir
                int cantidadNueva = cantidadActual + cantidadReducida;
                // Asignar la nueva cantidad a la plataforma
                plataforma.Cantidad = cantidadNueva;

                // Guardar los cambios en la base de datos
                await _dbContext.SaveChangesAsync();
            }
            var clientePlataforma = await _dbContext.ClientesPlataforma.SingleOrDefaultAsync(c => c.IdCliPltf == idCliente);
            if (clientePlataforma != null)
            {
                clientePlataforma.Estado = estado;
                await _dbContext.SaveChangesAsync();
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
        public List<Factura> ObtenerFacturasPorFechaYUsuario(DateTime fecha, int cedula)
        {
            return _dbContext.Factura
                .Where(f => f.FechaVenta.Date == fecha.Date && f.Cedula == cedula && f.TipoFactura == "Compra")
                .ToList();
        }
        // Método para obtener los detalles de la factura seleccionada
        public DetallesFacturaViewModel ObtenerDetallesFactura(int codFactura)
        {
            var factura = _dbContext.Factura
                .Where(f => f.Cod_factura == codFactura)
                .FirstOrDefault();

            var compras = _dbContext.HistoricoCompras
                .Where(h => h.cod_factura == codFactura)
                .ToList();

            if (factura == null || compras.Count == 0)
            {
                return null;
            }

            var model = new DetallesFacturaViewModel
            {
                Factura = factura,
                Compras = compras
            };

            return model;
        }
    }
}

using DocumentFormat.OpenXml.InkML;
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
            return _dbContext.Productos
                .Where(p => p.Estado == 1)
                .ToList();
        }

        public List<CategoriaProductos> ObtenerCategorias()
        {
            return _dbContext.CategoriaProductos.ToList();
        }

        public List<Producto> ObtenerProductosPorCategoria(int idCategoria)
        {
            return _dbContext.Productos
                .Where(p => p.IdCatepro == idCategoria && p.Estado == 1)
                .ToList();
        }
        public List<CategoriaProductos> ObtenerCategoriaProductos(int idServicio)
        {
            return _dbContext.CategoriaProductos
                           .Where(c => c.IdServicio == idServicio)
                           .ToList();
        }
        public async Task<List<CategoriaProductos>> ObtenerCategoriasPorServicio(int idServicio)
        {
            return await _dbContext.CategoriaProductos
                .Where(c => c.IdServicio == idServicio)
                .ToListAsync();
        }
        public Task<bool> AgregarProductoAsync(string id_empresa, string codigo, string descripcion, float valor_neto, float valor_unitario, decimal stock, int categorias, int id_proveedor)
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
                    Ubicacion = Ubicacion,
                    idProveedor = id_proveedor
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
                var productosProximosSinStock = _dbContext.Productos.Where(p => p.CantidadProducto <= 3 && p.Cod_Producto == searchTerm && p.Estado == 1).ToList(); // Suponiendo que "próximos sin stock" se refiere a productos con cantidad menor a 5
                return productosProximosSinStock;
            }
            else if (categoriaTerm > 0)
            {
                Console.WriteLine("Categoria");
                var productosProximosSinStock = _dbContext.Productos.Where(p => p.CantidadProducto <= 3 && p.IdCatepro == categoriaTerm && p.Estado == 1).ToList(); // Suponiendo que "próximos sin stock" se refiere a productos con cantidad menor a 5
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
        public void EditarProducto(string codigo, float valorNeto, float valorVenta, int valorUnidad, int cantidad)
        {

            if (codigo == null || valorNeto < 0 || valorVenta < 0 || cantidad < 0)
            {
                Console.WriteLine("Error: Todos los campos deben tener un valor. No se permiten valores nulos.");
                return;
            }

            var producto = _dbContext.Productos.FirstOrDefault(p => p.Cod_Producto == codigo);
            //Console.WriteLine("El ID de la empresa es: " + idEmpresa);
            if (producto != null)
            {
                producto.Cod_Producto = codigo;
                producto.CantidadProducto = cantidad;
                producto.ValorNetoProducto = valorNeto;
                producto.ValorVentaProducto = valorVenta;
                producto.ValorUnidad = valorUnidad;
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

        //plataformas de streaming
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
        //fin plataformas de streaming
        public async Task<bool> CrearCategoriaAsync(CategoriaProductos categoria)
        {
            _dbContext.CategoriaProductos.Add(categoria);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<List<Servicio>> ObtenerServiciosAsync()
        {
            return await _dbContext.Servicio.ToListAsync();
        }
        public async Task<List<Servicio>> ObtenerServicios()
        {
            return await _dbContext.Servicio.ToListAsync();
        }
        public async Task<List<Proveedores>> ObtenerProveedores()
        {
            return await _dbContext.Proveedores.ToListAsync();
        }
        public async Task<int?> SeleccionarServicio(Producto p)
        {
            return await _dbContext.CategoriaProductos
                .Where(c => c.IdCateProducto == p.IdCatepro)
                .Select(c => (int?)c.IdServicio)
                .FirstOrDefaultAsync();
        }

        public async Task<Servicio> CrearServicio(Servicio servicio)
        {
            _dbContext.Servicio.Add(servicio);
            await _dbContext.SaveChangesAsync();
            return servicio;
        }
        public async Task<PagedResult<ProductoStockVm>> ObtenerStockAsync(
        int? sedeId, string? q, int page, int pageSize,
        string? sortBy = null, bool desc = false)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 20 : pageSize;
            q = (q ?? string.Empty).Trim();

            var hasSede = sedeId.HasValue && sedeId.Value > 0;

            IQueryable<ProductoStockVm> baseQuery;

            if (!hasSede)
            {
                // TODAS las sedes (agregado por producto)
                baseQuery =
                    from i in _dbContext.InventarioSedes
                    join p in _dbContext.Productos on i.ProductoId equals p.Cod_Producto
                    where p.Estado == 1
                    group i by new { p.Cod_Producto, p.NombreProducto } into g
                    select new ProductoStockVm
                    {
                        ProductoId = g.Key.Cod_Producto,
                        Nombre = g.Key.NombreProducto!,
                        Cantidad = g.Sum(x => x.Cantidad)
                    };
            }
            else
            {
                // SOLO la sede seleccionada
                var sedeNombre = await _dbContext.Sede
                    .Where(s => s.Id_sede == sedeId.Value)
                    .Select(s => s.NombreSede)
                    .FirstOrDefaultAsync();

                baseQuery =
                    from i in _dbContext.InventarioSedes
                    join p in _dbContext.Productos on i.ProductoId equals p.Cod_Producto
                    where p.Estado == 1 && i.SedeId == sedeId.Value
                    select new ProductoStockVm
                    {
                        ProductoId = p.Cod_Producto!,
                        Nombre = p.NombreProducto!,
                        Cantidad = i.Cantidad,
                        SedeId = sedeId.Value,
                        SedeNombre = sedeNombre
                    };
            }

            // Filtro de búsqueda (SKU o Nombre)
            if (!string.IsNullOrEmpty(q))
            {
                var qLower = q.ToLower();
                baseQuery = baseQuery.Where(x =>
                    x.ProductoId.ToLower().Contains(qLower) ||
                    x.Nombre.ToLower().Contains(qLower)
                );
            }

            // Orden
            baseQuery = (sortBy?.ToLower()) switch
            {
                "cantidad" => desc ? baseQuery.OrderByDescending(x => x.Cantidad)
                                   : baseQuery.OrderBy(x => x.Cantidad),
                _ => desc ? baseQuery.OrderByDescending(x => x.Nombre)
                                   : baseQuery.OrderBy(x => x.Nombre),
            };

            var totalRows = await baseQuery.CountAsync();
            var rows = await baseQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ProductoStockVm>
            {
                Page = page,
                PageSize = pageSize,
                TotalRows = totalRows,
                Rows = rows
            };
        }


        public async Task<(decimal totalUnidades, int skusConStock)> ResumenAsync(int? sedeId, string? q)
        {
            // Construimos la misma base de datos que arriba pero solo para sumar
            IQueryable<ProductoStockVm> baseQuery;

            if (sedeId == null)
            {
                baseQuery =
                    from i in _dbContext.InventarioSedes
                    join p in _dbContext.Productos on i.ProductoId equals p.Cod_Producto
                    where p.Estado == 1
                    group i by new { p.Cod_Producto, p.NombreProducto } into g
                    select new ProductoStockVm
                    {
                        ProductoId = g.Key.Cod_Producto,
                        Nombre = g.Key.NombreProducto,
                        Cantidad = g.Sum(x => x.Cantidad) // suma aunque sea 0
                    };
            }
            else
            {
                baseQuery =
                    from p in _dbContext.Productos
                    where p.Estado == 1
                    join i in _dbContext.InventarioSedes.Where(x => x.SedeId == sedeId.Value)
                        on p.Cod_Producto equals i.ProductoId into gi
                    from i in gi.DefaultIfEmpty()
                    select new ProductoStockVm
                    {
                        ProductoId = p.Cod_Producto,
                        Nombre = p.NombreProducto,
                        Cantidad = i != null ? i.Cantidad : 0
                    };
            }

            if (!string.IsNullOrEmpty(q))
            {
                var qLower = q.ToLower();
                baseQuery = baseQuery.Where(x =>
                    x.Nombre.ToLower().Contains(qLower));
            }

            var totalUnidades = await baseQuery.SumAsync(x => (decimal?)x.Cantidad) ?? 0;
            var skusConStock = await baseQuery.CountAsync(x => x.Cantidad > 0);
            return (totalUnidades, skusConStock);
        }

        public async Task AplicarMovimientoAsync(int productoId, int sedeId, decimal delta, string? motivo = null)
        {
            var conn = _dbContext.Database.GetDbConnection();
            await _dbContext.Database.OpenConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "dbo.sp_Inventario_AplicarMovimiento";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            var p1 = cmd.CreateParameter(); p1.ParameterName = "@ProductoId"; p1.Value = productoId; cmd.Parameters.Add(p1);
            var p2 = cmd.CreateParameter(); p2.ParameterName = "@SedeId"; p2.Value = sedeId; cmd.Parameters.Add(p2);
            var p3 = cmd.CreateParameter(); p3.ParameterName = "@Delta"; p3.Value = delta; cmd.Parameters.Add(p3);
            var p4 = cmd.CreateParameter(); p4.ParameterName = "@Motivo"; p4.Value = (object?)motivo ?? DBNull.Value; cmd.Parameters.Add(p4);

            await cmd.ExecuteNonQueryAsync();
        }
        public async Task<decimal> ObtenerTotalStockAsync(int? sedeId)
        {
            if (sedeId == null)
            {
                return await _dbContext.InventarioSedes.SumAsync(x => (decimal?)x.Cantidad) ?? 0;
            }
            else
            {
                return await _dbContext.InventarioSedes
                    .Where(x => x.SedeId == sedeId.Value)
                    .SumAsync(x => (decimal?)x.Cantidad) ?? 0;
            }
        }

        public async Task<int> ObtenerSkusConStockAsync(int? sedeId)
        {
            if (sedeId == null)
            {
                return await _dbContext.InventarioSedes
                    .GroupBy(x => x.ProductoId)
                    .CountAsync(g => g.Sum(x => x.Cantidad) > 0);
            }
            else
            {
                return await _dbContext.InventarioSedes
                    .Where(x => x.SedeId == sedeId.Value)
                    .CountAsync(x => x.Cantidad > 0);
            }
        }
        public async Task<(int insertados, int omitidos)> InsertarLoteAsync(IEnumerable<ProductoInsertDto> lote)
        {
            int ok = 0, omit = 0;

            // Trae catálogos para validar (minimiza roundtrips)
            var catsByServicio = (await _dbContext.CategoriaProductos
                .Select(c => new { c.IdCateProducto, c.Descripcion, c.IdServicio })
                .ToListAsync())
                .GroupBy(x => x.IdServicio)
                .ToDictionary(g => g.Key, g => g.Select(x => x.IdServicio).ToHashSet());

            var proveedores = await _dbContext.Proveedores.Select(p => p.IdProveedor).ToListAsync();
            var proveedoresSet = proveedores.ToHashSet();

            // Evitar duplicados por código existente
            var cods = lote.Where(x => !string.IsNullOrWhiteSpace(x.Cod_Producto))
                           .Select(x => x.Cod_Producto!.Trim())
                           .Distinct()
                           .ToList();

            var existentes = await _dbContext.Productos
                .Where(p => cods.Contains(p.Cod_Producto!))
                .Select(p => p.Cod_Producto!)
                .ToListAsync();
            var existentesSet = existentes.ToHashSet();

            var nuevos = new List<Producto>();

            foreach (var x in lote)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(x.Cod_Producto) ||
                        string.IsNullOrWhiteSpace(x.NombreProducto) ||
                        x.IdCatepro <= 0 || x.idProveedor <= 0)
                    { omit++; continue; }

                    // valida relación categoría ↔ servicio
                    if (!catsByServicio.TryGetValue(x.ServicioId, out var bucket) || !bucket.Contains(x.IdCatepro))
                    { omit++; continue; }

                    var codigo = x.Cod_Producto!.Trim();
                    if (existentesSet.Contains(codigo))
                    { omit++; continue; }

                    nuevos.Add(new Producto
                    {
                        Cod_Producto = codigo,
                        NombreProducto = x.NombreProducto!.Trim(),
                        CantidadProducto = x.CantidadProducto,
                        ValorNetoProducto = x.ValorNetoProducto,
                        ValorVentaProducto = x.ValorVentaProducto,
                        ValorUnidad = x.ValorUnidad,
                        ID_Empresa = x.ID_Empresa,
                        Estado = x.Estado,
                        Ubicacion = x.Ubicacion,
                        IdCatepro = x.IdCatepro,
                        idProveedor = x.idProveedor
                    });
                    ok++;
                }
                catch { omit++; }
            }

            if (nuevos.Count > 0)
            {
                _dbContext.Productos.AddRange(nuevos);
                await _dbContext.SaveChangesAsync();
            }

            return (ok, omit);
        }

        public async Task<List<Servicio>> GetServiciosAsync()
            => await _dbContext.Servicio
                .OrderBy(s => s.NombreServicio)
                .Select(s => new Servicio { IdServicio = s.IdServicio, DescripcionServicio = s.DescripcionServicio })
                .ToListAsync();

        public async Task<List<CategoriaProductos>> GetCategoriasPorServicioAsync(int servicioId)
            => await _dbContext.CategoriaProductos
                .Where(c => c.IdServicio == servicioId)
                .OrderBy(c => c.Descripcion)
                .Select(c => new CategoriaProductos { IdCateProducto = c.IdCateProducto, Descripcion = c.Descripcion })
                .ToListAsync();

        public async Task<List<Proveedores>> GetProveedoresAsync()
            => await _dbContext.Proveedores
                .OrderBy(p => p.RazonSocial)
                .Select(p => new Proveedores { IdProveedor = p.IdProveedor, RazonSocial = p.RazonSocial })
                .ToListAsync();
        public bool ValidarSedeAsignacionProducto(int idSede)
        {
            return _dbContext.Sede.Any(idsede => idsede.Id_sede == idSede);
        }
        public bool ValidarProductoAsignacion(string producto)
        {
            return _dbContext.Productos.Any(codProducto => codProducto.Cod_Producto == producto);
        }
        public bool ValidarCantidadProducto(string producto, decimal cantidad)
        {
            // Busca la cantidad disponible del producto
            var stock = _dbContext.Productos
                .Where(cp => cp.Cod_Producto == producto)
                .Select(c => c.CantidadProducto)
                .FirstOrDefault();

            // Si no existe el producto o la cantidad es insuficiente, retorna false
            return stock >= cantidad;
        }
        public InventarioSede AsignarProductoSede(string producto, int sede, int cantidad, int valorUnitario, string cedulaClaim)
        {
                var cedula = int.Parse(cedulaClaim);
                var inventario = _dbContext.InventarioSedes
                .FirstOrDefault(cod => cod.ProductoId == producto && cod.SedeId == sede);
                if (inventario != null) 
                { 
                    inventario.Cantidad += cantidad;
                    inventario.ActualizadoEn = DateTime.Now;
                    inventario.Cedula = cedula;
                }else
                {
                    inventario = new InventarioSede
                    {
                        ProductoId = producto,
                        SedeId = sede,
                        Cantidad = cantidad,
                        PrecioUnitario = valorUnitario,
                        ActualizadoEn = DateTime.Now,
                        Cedula = cedula
                    };
                    _dbContext.InventarioSedes.Add(inventario);
                    _dbContext.SaveChanges();
                }
                var prod = _dbContext.Productos.FirstOrDefault(p => p.Cod_Producto == producto);
                prod.CantidadProducto -= cantidad;
                _dbContext.SaveChanges();

                return inventario;

        }
        public List<Producto> TraerProductosInactivos()
        {
            return _dbContext.Productos.Where(est => est.Estado == 0).ToList();
        }
    }

}

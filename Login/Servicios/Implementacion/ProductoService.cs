using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using OpenAI_API;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Servicios.Implementacion
{
    public class ProductoService : IProductoService
    {
        //variable de solo lectura para referenciar la base de datos
        private readonly BaseAdmContext _dbContext;
        private readonly OpenAIAPI _openAIAPI;
        private readonly string _connectionString;
        public ProductoService(BaseAdmContext dbContext, IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("cadenaSQL");
            _dbContext = dbContext;
            var apiKey = configuration["OpenAI:ApiKey"];
            _openAIAPI = new OpenAIAPI(apiKey);
        }
        public List<Producto> ObtenerProductos()
        {
            return _dbContext.Productos.ToList();
        }
        public Task<bool> AgregarProductoAsync(string id_empresa, string codigo, string descripcion, float valor_neto, float valor_unitario, int stock, string categorias)
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
                    valorUnidad = valorUnidad,
                    ID_Empresa = id_empresa,
                    Categoria = categorias,
                    estado = estado,
                    Ubicacion = Ubicacion
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
        public List<Producto> SinStock(string searchTerm, string categoriaTerm)
        {

            // Lógica para buscar productos por el nombre o la categoría
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var productosSinStock = _dbContext.Productos
                    .Where(p => p.Cod_Producto == searchTerm && p.CantidadProducto == 0)
                    .ToList();
                return productosSinStock;
            }
            else if (!string.IsNullOrEmpty(categoriaTerm))
            {
                var productosSinStock = _dbContext.Productos
                    .Where(p => p.Categoria == categoriaTerm && p.CantidadProducto == 0)
                    .ToList();
                return productosSinStock;
            }
            else
            {
                // Ambos términos están vacíos, puedes manejarlo según tus necesidades
                return new List<Producto>();
            }
        }
        public List<Producto> BuscarProSinStock(string searchTerm, string categoriaTerm)
        {

            // Lógica para buscar productos por el nombre o la categoría
            if (!string.IsNullOrEmpty(searchTerm))
            {
                Console.WriteLine("searchTerm");
                var productosProximosSinStock = _dbContext.Productos.Where(p => p.CantidadProducto == 1 && p.Cod_Producto == searchTerm).ToList(); // Suponiendo que "próximos sin stock" se refiere a productos con cantidad menor a 5
                return productosProximosSinStock;
            }
            else if (!string.IsNullOrEmpty(categoriaTerm))
            {
                Console.WriteLine("Categoria");
                var productosProximosSinStock = _dbContext.Productos.Where(p => p.CantidadProducto == 1 && p.Categoria == categoriaTerm).ToList(); // Suponiendo que "próximos sin stock" se refiere a productos con cantidad menor a 5
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
        public void EditarProducto(string codigo, string nombreProducto, float valorNeto, float valorVenta, int valorUnidad, int cantidad, string categoria, string idEmpresa, int estado)
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
                producto.valorUnidad = valorUnidad;
                producto.ID_Empresa = idEmpresa;
                producto.Categoria = categoria;
                producto.estado = estado;
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
        public void inserPlataformaService(string plataforma, string descripcion, int valorventa, int valorneto, DateTime fechaInipago, DateTime fechaFinpago, int cantidad, string correo, string contrasena, int cedula, int estado)
        {
            var nuevaPlataforma = new Plataformas
            {
                plataforma = plataforma,
                descripcion = descripcion,
                valorVenta = valorventa,
                valorNeto = valorneto,
                fechaIniPago = fechaInipago,
                fechaFinPago = fechaFinpago,
                cantidad = cantidad,
                correo = correo,
                contrasena = contrasena,
                cedulaEmpleado = cedula,
                estado = estado
            };

            // Agregar el nuevo producto al DbContext y guardar los cambios en la base de datos
            _dbContext.Plataformas.Add(nuevaPlataforma);
            _dbContext.SaveChanges();
        }
        public List<Plataformas> traerPlataformasExistentes()
        {
            return _dbContext.Plataformas.ToList();
        }
        public List<ClientePlataformaDTO> TraerCtaClientPlatfExistentes()
        {
            var resultado = (from cp in _dbContext.ClientesPlataforma
                             join p in _dbContext.Plataformas on cp.idPlataforma equals p.idPlataforma
                             select new ClientePlataformaDTO
                             {
                                 idCliente = cp.idCliPltf,
                                 IdClientePlataforma = cp.idPlataforma,
                                 CorreoPlataforma = p.correo,
                                 ClavePlataforma = p.contrasena,
                                 NombreCliente = cp.nombreCliente,
                                 ClavePerfil = cp.clavePerfil,
                                 Celular = cp.celularCliente,
                                 FechaIni = cp.fechaIniPago,
                                 FechaFin = cp.fechaFinPago,
                                 Plataforma = p.plataforma,
                                 Estado = cp.estado
                             }).ToList();
            return resultado;
        }
        public void servicioInsertarVentClientPlataforma(string nombrecliente, string celularcliente, string correo, string contrasena, int idplataforma, int cantidad, string ppm, DateTime feciniplat, DateTime fecfinplat, int valorventa, int valorneto, int cedula, int estado, string clave)
        {
            var plataforma = _dbContext.Plataformas.SingleOrDefault(p => p.idPlataforma == idplataforma);
            if (plataforma != null)
            {
                // Si la plataforma existe, actualizar el campo cantidad
                int cantidadActual = plataforma.cantidad;
                int cantidadNueva = cantidadActual - cantidad;
                if (cantidadNueva < 0)
                {
                    Console.WriteLine("La plataforma no tiene esa cantidad de espacios disponibles");
                }else
                {
                    //agregar cuenta
                    var nuevoVentClientPltf = new ClientesPlataforma
                    {
                        nombreCliente = nombrecliente,
                        celularCliente = celularcliente,
                        correo = correo,
                        clave = contrasena,
                        idPlataforma = idplataforma,
                        cantidad = cantidad,
                        ppm = ppm,
                        fechaIniPago = feciniplat,
                        fechaFinPago = fecfinplat,
                        valorVenta = valorventa,
                        valorNeto = valorneto,
                        cedulaEmpleado = cedula,
                        estado = estado,
                        clavePerfil = clave,
                    };

                    // Agregar el nuevo producto al DbContext y guardar los cambios en la base de datos
                    _dbContext.ClientesPlataforma.Add(nuevoVentClientPltf);
                    _dbContext.SaveChanges();

                    //cantidad
                    plataforma.cantidad = cantidadNueva;
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
            var plataforma = await _dbContext.Plataformas.SingleOrDefaultAsync(p => p.idPlataforma == id);
            if (plataforma != null)
            {
                // Si la plataforma existe, actualizar el campo cantidad
                int cantidadActual = plataforma.cantidad;
                int cantidadReducida = 1; // Este es el valor que quieres reducir
                int cantidadNueva = cantidadActual + cantidadReducida;
                // Asignar la nueva cantidad a la plataforma
                plataforma.cantidad = cantidadNueva;

                // Guardar los cambios en la base de datos
                await _dbContext.SaveChangesAsync();
            }
            var clientePlataforma = await _dbContext.ClientesPlataforma.SingleOrDefaultAsync(c => c.idCliPltf == idCliente);
            if (clientePlataforma != null)
            {
                clientePlataforma.estado = estado;
                await _dbContext.SaveChangesAsync();
            }
        }
        public List<Producto> traerProductosXCategoria(string categoria)
        {
            var productos = _dbContext.Productos
               .Where(p => p.Categoria == categoria && p.estado == 1)
               .ToList();
            return productos;
        }


        /*Metodos con OpenAI*/
        // Método para buscar productos en la base de datos
        public async Task<List<string>> BuscarProductosAsync(string consulta)
        {
            var productos = new List<string>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Buscar productos que coincidan con la consulta del usuario
                string query = "SELECT nombreProducto FROM Productos WHERE nombreProducto LIKE @consulta";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@consulta", "%" + consulta + "%");
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            productos.Add(reader.GetString(0));
                        }
                    }
                }
            }

            return productos;
        }

        // Método para generar una respuesta con OpenAI
        public async Task<string> GenerarRespuestaAsync(string consulta, List<string> productos)
        {
            string prompt;

            if (productos.Count > 0)
            {
                // Si hay productos encontrados, genera una respuesta basada en ellos
                prompt = $"El usuario está buscando '{consulta}'. Estos son los productos que coinciden: {string.Join(", ", productos)}.";
            }
            else
            {
                // Si no se encontraron productos, pregunta a OpenAI cómo responder
                prompt = $"El usuario está buscando '{consulta}', pero no se encontraron productos coincidentes. Proporcione una respuesta general sobre productos relacionados.";
            }

            var completion = await _openAIAPI.Completions.CreateCompletionAsync(new OpenAI_API.Completions.CompletionRequest
            {
                Prompt = prompt,
                MaxTokens = 150
            });

            return completion.Completions[0].Text.Trim();
        }
    }
}

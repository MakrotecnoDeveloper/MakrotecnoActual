using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using System.Runtime.InteropServices;

namespace Plataforma.Servicios.Implementacion
{
    public class UsuarioService : IUsuarioService
    {
        private readonly BaseAdmContext _dbContext;
        private readonly ILogger<UsuarioService> _logger;
        public UsuarioService(BaseAdmContext dbContext, ILogger<UsuarioService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public async Task<List<Empleados>> ObtenerUsuarios()
        {
            var empleados = await _dbContext.Empleado.ToListAsync();
            return empleados;
        }
        public async Task<List<Empleados>> ObtenerTodos()
        {
            return await _dbContext.Empleado.ToListAsync();
        }

        public async Task<Empleados> ObtenerPorCedula(int cedula)
        {
            return await _dbContext.Empleado.FindAsync(cedula);
        }

        public async Task InsertarEmpleado(Empleados empleado)
        {
            _dbContext.Empleado.Add(empleado);
            await _dbContext.SaveChangesAsync();
        }

        public async Task ActualizarEmpleado(Empleados empleado)
        {
            _dbContext.Empleado.Update(empleado);
            await _dbContext.SaveChangesAsync();
        }
        public int ObtenerRolPermisos(int cedula)
        {
            return _dbContext.Sedeempleado
                     .Where(se => se.Cedula == cedula)
                     .Select(se => se.Id_cargo)
                     .FirstOrDefault();
        }
        public RolPermisoDTO? ObtenerNombreRolPermisos(int rolEmpleado)
        {
            return (from tc in _dbContext.TipoCargo
                    join e in _dbContext.Empresas
                        on tc.Id_empresa equals e.Id_empresa
                    where tc.Id_tipo == rolEmpleado
                    select new RolPermisoDTO
                    {
                        NombreCargo = tc.NombreCargo,
                        IdEmpresa = tc.Id_empresa,
                        TipoCargo = tc.Id_tipo,
                        NombreEmpresa = e.Nombre  // ⬅️ agregado
                    })
            .FirstOrDefault();
        }
        public int TraerUltimoIDPdv(int cedulaEmpleado)
        {
            return _dbContext.LogsLogin
            .Where(ce => ce.Cedula == cedulaEmpleado)
            .OrderByDescending(ce => ce.Id_log)
            .Select(ce => ce.InfopdvId)
            .FirstOrDefault();
        }
        public bool ServValidarDisponSede(int cedula)
        {
            var empleadoExistente = _dbContext.Sedeempleado.FirstOrDefault(ced => ced.Cedula == cedula);
            return empleadoExistente != null;
        }
        public LogsLogin? InsertarLogLogin(int cedulaEmpleado, string correoEmpleado, int estado, int idPDV)
        {
            var utcNow = DateTime.UtcNow;
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            var localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, timeZone);

            var login = new LogsLogin
            {
                Cedula = cedulaEmpleado,
                Correo = correoEmpleado,
                Fecha = localDateTime,
                Estado = estado,
                InfopdvId = idPDV
            };

                _dbContext.LogsLogin.Add(login);
                var result = _dbContext.SaveChanges();

                if (result > 0)
                {
                    return login;
                }
                else
                {
                    return null;
                }
        }

        public Empleados? GetUsuarios(int cedula, string password)
        {
            Empleados? usuario_encontrando = _dbContext.Empleado.Where(u => u.Cedula == cedula && u.Contrasena == password).FirstOrDefault();
            if (usuario_encontrando == null)
            {
                return null;
            }

            return usuario_encontrando;
        }
        public List<Infopdv> FunValidarPDV(int cedula)
        {

			var idSede = _dbContext.Sedeempleado
	        .Where(se => se.Cedula == cedula)
	        .Select(se => se.Id_sede)
	        .FirstOrDefault();

			if (idSede <= 0)
			{
				return new List<Infopdv>();
			}

			return _dbContext.Infopdv
	        .Where(p => p.Id_Sede == idSede)
	        .ToList();
            

        }
        public async Task<Empleados> SaveUsuario(Empleados modelo)
        {
            _dbContext.Empleado.Add(modelo);
            await _dbContext.SaveChangesAsync();
            return modelo;
        }
        public bool ValidarEmpleado(int cedula)
        {
            var empleadoExistente = _dbContext.Empleado.FirstOrDefault(p => p.Cedula == cedula);
            return empleadoExistente != null;
        }
        public Empleados ObtenerEmpleadoPorId(string cedula)
        {
            return _dbContext.Empleado.Find(cedula);
        }

        public void AgregarEmpleado(Empleados empleado)
        {
            _dbContext.Empleado.Add(empleado);
            _dbContext.SaveChanges();
        }
        public List<Empleados> BuscarUsuario(int id)
        {
            if (id > 0)
            {
                var consulta = _dbContext.Empleado.Where(p => p.Cedula == id).ToList();
                return consulta;
            }
            else
            {
                return new List<Empleados>();
            }
        }
        public void EditarEmpleado(Empleados empleado, int cedula, string nombre, string apellido, string genero, string correo, string rh, string celular, string contrasena)
        {
                empleado.Cedula = cedula;
                empleado.Nombre = nombre;
                empleado.Apellido = apellido;
                empleado.Genero = genero;
                empleado.Correo = correo;
                empleado.Rh = rh;
                empleado.Celular = celular;
                empleado.Contrasena = contrasena;
                _dbContext.SaveChanges();
        }
        public List<TipoCargo> ObtenerCargos()
        {
            return _dbContext.TipoCargo.ToList();
        }
        public List<Empresas> ObtenerEmpresas()
        {
            return _dbContext.Empresas.ToList();
        }
        public TipoCargo? ValidarCargo(string nombreCargo)
        {
            return _dbContext.TipoCargo.FirstOrDefault(p => p.NombreCargo == nombreCargo);
        }
        public IEnumerable<TipoCargo> InsertarCargos(string nombreCargo, string descripcionCargo, string id_empresa)
        {
            var nuevoCargo = new TipoCargo
            {
                NombreCargo = nombreCargo,
                DescripcionCargo = descripcionCargo,
                Id_empresa = id_empresa
            };
            _dbContext.TipoCargo.Add(nuevoCargo);
            _dbContext.SaveChanges();
            return _dbContext.TipoCargo.ToList();
        }
        public Empresas? ValidarExistenciaEmpresa(string nit)
        {
            return _dbContext.Empresas.FirstOrDefault(p => p.Id_empresa == nit);
        }
        public IEnumerable<Empresas> InsertarEmpresa(string nit, string nombreEmpresa, string pais, string calle, string carrera, string ciudad, string departamento, string indicativo, string numero)
        {
            var direccion = calle + " # " + carrera + ", " + ciudad + " - " + departamento;
            var celular = indicativo + " " + numero;
            var nuevaEmpresa = new Empresas
            {
                Id_empresa = nit,
                Nombre = nombreEmpresa,
                Pais = pais,
                Direccion = direccion,
                Telefono = celular
            };
            _dbContext.Empresas.Add(nuevaEmpresa);
            _dbContext.SaveChanges();
            return _dbContext.Empresas.ToList();
        }
        public List<Sede> ObtenerSedes()
        {
            return _dbContext.Sede.ToList();
        }
        public Sede? ValidarExistenciaSede(string nombreSede)
        {
            return _dbContext.Sede.FirstOrDefault(p => p.NombreSede == nombreSede);
        }
        public IEnumerable<Sede> InsertarSede(string id_empresa, string nombreSede, string ciudad, string direccion, string telefono)
        {
            var nuevaSede = new Sede
            {
                Id_empresa = id_empresa,
                NombreSede = nombreSede,
                Ciudad = ciudad,
                Direccion = direccion,
                Telefono = telefono
            };
            _dbContext.Sede.Add(nuevaSede);
            _dbContext.SaveChanges();
            return _dbContext.Sede.ToList();
        }
        public EmpleadoEmpresa? ValidarExisEmpleadoEmpresa(string id_empresa, int cedula)
        {
            return _dbContext.EmpleadoEmpresa.FirstOrDefault(ee => ee.Id_empresa == id_empresa && ee.Cedula == cedula);
        }
        public IEnumerable<EmpleadoEmpresa> InsertarEmpleadoEmpresa(string id_empresa, int cedula)
        {
            var nuevoEmpleadoEmpresa = new EmpleadoEmpresa
            {
                Id_empresa = id_empresa,
                Cedula = cedula
            };
            _dbContext.EmpleadoEmpresa.Add(nuevoEmpleadoEmpresa);
            _dbContext.SaveChanges();
            return _dbContext.EmpleadoEmpresa.ToList();
        }
        public EmpleadoSedeViewModel? EmpleadoSede()
        {
            var empleados = _dbContext.Empleado.ToList();
            var empresas = _dbContext.Empresas.ToList();
            var sedes = _dbContext.Sede.ToList();
            var cargos = _dbContext.TipoCargo.ToList();
            var sedeEmpleados = _dbContext.Sedeempleado.ToList();
            var empleadoEmpresas = _dbContext.EmpleadoEmpresa.ToList();

            if (empleados != null && empresas != null && sedes != null && cargos != null && sedeEmpleados != null && empleadoEmpresas != null)
            {
                var empleadoConSedeYCargo = sedeEmpleados
                    .Select(se =>
                    {
                        var empleado = empleados.FirstOrDefault(e => e.Cedula == se.Cedula);
                        var sede = sedes.FirstOrDefault(s => s.Id_sede == se.Id_sede);
                        var cargo = cargos.FirstOrDefault(c => c.Id_tipo == se.Id_cargo);
                        var empresa = empleadoEmpresas
                            .Where(ee => ee.Cedula == se.Cedula)
                            .Join(empresas, ee => ee.Id_empresa, emp => emp.Id_empresa, (ee, emp) => emp)
                            .FirstOrDefault();

                        return new EmpleadoConSedeYEmpresa
                        {
                            Empleado = empleado,
                            Sede = sede,
                            Empresa = empresa,
                            TipoCargo = cargo
                        };
                    }).Where(e => e.Empleado != null && e.Sede != null && e.Empresa != null && e.TipoCargo != null).ToList();

                return new EmpleadoSedeViewModel
                {
                    Empleados = empleados,
                    Empresas = empresas,
                    Sedes = sedes,
                    EmpleadoConSedeYEmpresas = empleadoConSedeYCargo
                };
            }

            throw new Exception("Error 14 (EMPL/EMPR/SEDE/CARG/SEDEMPL/EMPLEMPRE)");
        }
        public List<Sede> GetSedesByEmpresaId(string empresaId)
        {
            return _dbContext.Sede
             .Where(s => s.Id_empresa == empresaId)
             .Select(s => new Sede
             {
                 Id_sede = s.Id_sede,
                 NombreSede = s.NombreSede
             })
             .ToList();
        }
        public Empleados? ValidarCedula(int cedula)
        {
            return _dbContext.Empleado.FirstOrDefault(e => e.Cedula == cedula);
        }
        public string? ObtenerIdEmpresa(int cedula)
        {
            return _dbContext.EmpleadoEmpresa
                .Where(ee => ee.Cedula == cedula)
                .Select(ee => ee.Id_empresa)
                .FirstOrDefault();
        }
        public List<Sede> ObtenerSedes(string idEmpresa)
        {
            return _dbContext.Sede.Where(s => s.Id_empresa == idEmpresa).ToList();
        }

        public bool ObtenerSedePorEmpleado(int cedula)
        {
            // Buscar el registro en Sedeempleado basado en la cédula
            var empleadoSede = _dbContext.Sedeempleado.FirstOrDefault(es => es.Cedula == cedula);
            if(empleadoSede == null)
            {
                return true;
            }else
            {
                return false;
            }
        }
        public List<TipoCargo> ObtenerCargos(string idEmpresa)
        {
            return _dbContext.TipoCargo.Where(c => c.Id_empresa == idEmpresa).ToList();
        }
        public bool InsertarSedeEmpleado(int cedula, int idSede, int idCargo)
        {
            var sedeEmpleado = new Sedeempleado
            {
                Cedula = cedula,
                Id_sede = idSede,
                Id_cargo = idCargo
            };

            _dbContext.Sedeempleado.Add(sedeEmpleado);
            _dbContext.SaveChanges();
            return true;
        }
        public async Task<List<FacProUserViewModel>> TraerFactXDia(int cedula, int idPDVActual)
        {
            // 1) Fechas de "hoy" en zona horaria local (Colombia)
            var tzId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "SA Pacific Standard Time"    // Windows
                : "America/Bogota";             // Linux/macOS
            var tz = TimeZoneInfo.FindSystemTimeZoneById(tzId);

            var nowUtc = DateTime.UtcNow;
            var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, tz);

            var hoyLocalInicio = nowLocal.Date;           // 00:00 local
            var mananaLocalInicio = hoyLocalInicio.AddDays(1);

            // Si tu columna FechaEmision está guardada en UTC en la BD, convierte el rango a UTC:
            var hoyUtcInicio = TimeZoneInfo.ConvertTimeToUtc(hoyLocalInicio, tz);
            var mananaUtcInicio = TimeZoneInfo.ConvertTimeToUtc(mananaLocalInicio, tz);

            // ----- IMPORTANTE -----
            // Usa UNO de los dos bloques siguientes según cómo guardes FechaEmision.

            // 1. Obtener el id_sede del usuario logueado
            var idSedeUsuario = await _dbContext.Sedeempleado
                .Where(se => se.Cedula == cedula)
                .Select(se => se.Id_sede)
                .FirstOrDefaultAsync();

            // 2. Obtener todas las cédulas de empleados en esa sede
            var cedulasEnSede = await _dbContext.Sedeempleado
                .Where(se => se.Id_sede == idSedeUsuario)
                .Select(se => se.Cedula)
                .ToListAsync();

            // 3. Obtener las facturas relacionadas a esas cédulas (por ventas)
            var facturasQuery = _dbContext.Factura
                .Where(f => f.FechaEmision >= hoyLocalInicio && f.FechaEmision < mananaLocalInicio)
                .Where(f => _dbContext.Ventas
                    .Where(v => cedulasEnSede.Contains(v.Cedula))
                    .Select(v => v.IdVenta)
                    .Contains(f.IdVenta));

            // 2A) Si guardas FechaEmision en HORA LOCAL (común en apps on-prem):
            int totalFacturas = await facturasQuery.CountAsync();

            decimal totalVendido = await facturasQuery
            .SumAsync(f => (decimal?)f.Total) ?? 0m;

            // 3) Empleados totales (hasta ahora)
            var totalEmpleados = cedulasEnSede.Count;

            //InfoPDV
            string rolEmpleado = string.Empty; 
            var rolEmpleadoResult = (from se in _dbContext.Sedeempleado 
                                     join tc in _dbContext.TipoCargo 
                                     on se.Id_cargo equals tc.Id_tipo 
                                     where se.Cedula == cedula 
                                     select tc.NombreCargo).FirstOrDefault(); 
            if (rolEmpleadoResult != null) 
            { 
                rolEmpleado = rolEmpleadoResult; 
            } 
            else 
            { 
                rolEmpleado = "No asignado"; 
            }
            string? traerNombrePDV = _dbContext.Infopdv
                .Where(ce => ce.InfopdvId == idPDVActual)
                .Select(ce => ce.NombreInfoPDV)
                .FirstOrDefault();
            //FinInfoPDV

            // 4) Productos activos (Estado = 1)
            int totalProductosActivos = await _dbContext.InventarioSedes
                .CountAsync();

            // 5) Armas tu modelo/vista (agrega una propiedad si quieres mostrar el total vendido hoy)
            var viewModel = new FacProUserViewModel
            {
                TotalSumaCodFactura = totalFacturas,
                TotalEmpleados = totalEmpleados,
                TotalProductos = totalProductosActivos,
                TotalVentaDia = totalVendido,
                IdPDV = idPDVActual
            };

            return new List<FacProUserViewModel> { viewModel }; // o solo 'return viewModel;'
        }
        public async Task<IEnumerable<ClientesPlataforma>> ObtenerCuentasProximas(int idPlataforma)
        {
            var fechaActual = DateTime.Now;
            var cuentasProximas = await _dbContext.ClientesPlataforma
                .Where(cp => cp.IdPltfSuscripcion == idPlataforma && cp.FechaFinPago <= fechaActual.AddDays(5))
                .ToListAsync();

            return cuentasProximas;
        }
        #region tiendaMama
        //Tienda..
        public List<Producto> ProductosAbarrotes()
        {
            try
            {
                return _dbContext.Productos
                     .Where(c => c.IdCatepro == 42 && c.Estado == 1)
                     .OrderBy(c => c.NombreProducto)
                     .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos de abarrotes");
                _logger.LogInformation("Base de datos: {Database}", _dbContext.Database.GetDbConnection().Database);
                return new List<Producto>();
            }
            
        }
        public async Task<bool> ActualizarProductoAsync(string id, string campo, string newVal)
        {
            var producto = await _dbContext.Productos.FirstOrDefaultAsync(p => p.Cod_Producto == id);

            if (producto == null)
                return false;

            try
            {
                if (campo == "Cod_Producto")
                {
                    // Verificar que no exista otro producto con el nuevo código
                    var existe = await _dbContext.Productos.AnyAsync(p => p.Cod_Producto == newVal);
                    if (existe)
                        throw new InvalidOperationException($"Ya existe un producto con el código '{newVal}'.");

                    // Crear una copia del producto con el nuevo código
                    var nuevoProducto = new Producto
                    {
                        Cod_Producto = newVal,
                        NombreProducto = producto.NombreProducto,
                        CantidadProducto = producto.CantidadProducto,
                        ValorNetoProducto = producto.ValorNetoProducto,
                        ValorVentaProducto = producto.ValorVentaProducto,
                        ValorUnidad = producto.ValorUnidad,
                        ID_Empresa = producto.ID_Empresa,
                        Estado = producto.Estado,
                        Ubicacion = producto.Ubicacion,
                        IdCatepro = producto.IdCatepro,
                        idProveedor = producto.idProveedor,
                        ImagenPath = producto.ImagenPath,
                        AutenticidadProducto = producto.AutenticidadProducto,
                        CondicionProducto = producto.CondicionProducto
                    };

                    _dbContext.Productos.Add(nuevoProducto);
                    _dbContext.Productos.Remove(producto);
                    await _dbContext.SaveChangesAsync();

                    return true;
                }

                // Si el campo NO es la clave primaria, actualizar normalmente
                var property = producto.GetType().GetProperty(campo);

                if (property == null)
                    throw new ArgumentException($"El campo '{campo}' no existe en la clase Producto.");

                object? convertedValue;

                if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?))
                {
                    if (string.IsNullOrWhiteSpace(newVal))
                        convertedValue = null;
                    else if (decimal.TryParse(newVal, System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture, out var parsedDecimal))
                        convertedValue = parsedDecimal;
                    else
                        throw new FormatException($"El valor '{newVal}' no es un número decimal válido.");
                }
                else if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
                {
                    if (string.IsNullOrWhiteSpace(newVal))
                        convertedValue = null;
                    else if (int.TryParse(newVal, out var parsedInt))
                        convertedValue = parsedInt;
                    else
                        throw new FormatException($"El valor '{newVal}' no es un número entero válido.");
                }
                else
                {
                    convertedValue = Convert.ChangeType(newVal, property.PropertyType);
                }

                property.SetValue(producto, convertedValue);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> InsertProInventario(string codigoProducto, string nombreProducto, int cantidadProducto, decimal? valorNetoProductoFloat, decimal? valorVentaProductoFloat, int valorUnidadInt, string id_empresa, int categoria, int estado, string ubicacion, int IdProveedor)
        {
            var productosObtenidos = _dbContext.Productos.Where(c => c.IdCatepro == 42).OrderBy(c => c.NombreProducto).ToList();
            var codigosNumericos = productosObtenidos
            .Select(p => {
                bool isNumeric = int.TryParse(p.Cod_Producto, out int numero);
                return new { Producto = p, Numero = isNumeric ? (int?)numero : null };
            })
            .Where(p => p.Numero.HasValue)
            .Select(p => p.Numero.GetValueOrDefault())
            .ToList();
            var codigosNumericosOrdenados = codigosNumericos.OrderBy(n => n).ToList();

            foreach (var codigo in codigosNumericosOrdenados)
            {
                //_logger.LogInformation("Código numérico encontrado: " + codigo);

            }
            var nuevoProductoInventario = new Producto
            {
                Cod_Producto = codigoProducto,
                NombreProducto = nombreProducto,
                CantidadProducto = cantidadProducto,
                ValorNetoProducto = valorNetoProductoFloat,
                ValorVentaProducto = valorVentaProductoFloat,
                ValorUnidad = valorUnidadInt,
                ID_Empresa = id_empresa,
                IdCatepro = categoria,
                Estado = estado,
                Ubicacion = ubicacion,
                idProveedor = IdProveedor,
                ImagenPath = $"/img/Productos/nodisponible.png",
                AutenticidadProducto = "Original",
                CondicionProducto = "Nuevo"
            };
            _dbContext.Productos.Add(nuevoProductoInventario);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarProductoXIdAsync(string id)
        {
            var producto = await _dbContext.Productos.FirstOrDefaultAsync(c => c.Cod_Producto == id);

            if (producto != null)
            {
                producto.Estado = 0;

                await _dbContext.SaveChangesAsync();

                return true;
            }

            return false;
        }
        //Fin Tienda..
#endregion
        public Infopdv? SeleccionarNombrePDV(int selectedPDV)
        {
            return _dbContext.Infopdv.FirstOrDefault(p => p.InfopdvId == selectedPDV);
        }
        public int? ValidarExistenteIdPDV(int idPDV, int cedula)
        {
            return _dbContext.Syncpdv
                .Where(s => s.InfopdvId == idPDV && s.Cedula == cedula)
                .OrderByDescending(s => s.Idsync)
                .Select(s => s.Estado)
                .FirstOrDefault();
        }
        public Syncpdv AgregarEstadoPDV(int estadopdv, int idPDV, int cedula)
        {
            var nuevoEstadoPDV = new Syncpdv
            {
                InfopdvId = idPDV,
                Estado = estadopdv,
                FechaEstado = DateTime.Now,
                Cedula = cedula
            };
            _dbContext.Syncpdv.Add(nuevoEstadoPDV);
            _dbContext.SaveChanges();
            return nuevoEstadoPDV;
        }
        public bool InsertAddClient(int cedulaCliente, string nombreCliente, string empresaCliente, string ciudadCliente, string telefonoCliente, string correoCliente, string direccionCliente)
        {

            var cliente = new Clientes
            {
                CedulaCliente = cedulaCliente,
                NombreCliente = nombreCliente,
                EmpresaCliente = empresaCliente,
                CiudadCliente = ciudadCliente,
                TelefonoCliente = telefonoCliente,
                CorreoCliente = correoCliente,
                DireccionCliente = direccionCliente
            };

            _dbContext.Clientes.Add(cliente);
            var result = _dbContext.SaveChanges();

            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public List<Clientes> ServVisuaCliente()
        {
            var clientes = _dbContext.Clientes.ToList();
            return clientes;
        }
        public bool InsertAddProveedor(string nit, string razonSocial, string direccion, string celular, string correo)
        {
            var proveedor = new Proveedores
            {
                Nit = nit,
                RazonSocial = razonSocial,
                Direccion = direccion,
                Celular = celular,
                Correo = correo
            };

            _dbContext.Proveedores.Add(proveedor);
            var result = _dbContext.SaveChanges();

            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public List<Proveedores> ServVisuaProveedor()
        {
            var proveedores = _dbContext.Proveedores.ToList();
            return proveedores;
        }
        public List<Servicio> ServTraerServicios()
        {
            var servicios = _dbContext.Servicio.ToList();
            return servicios;
        }
        public async Task<string> ObtenerIdEmpresaAsync(int cargoId)
        {
            // Suponiendo que tienes una tabla o entidad que relaciona los cargos con las empresas
            var empresa = await _dbContext.TipoCargo
                .Where(e => e.Id_tipo == cargoId)
                .Select(e => e.Id_empresa) // Obtienes solo el IdEmpresa
                .FirstOrDefaultAsync(); // Traes el primer o único resultado

            return empresa; // Devuelves el IdEmpresa como string
        }
        public async Task<bool> CrearPuntoVentaAsync(Infopdv infopdv)
        {
            try
            {
                _dbContext.Infopdv.Add(infopdv);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Puedes loguear el error si tienes un sistema de logs
                return false;
            }
        }
        public async Task<Clientes?> ObtenerPorIdAsync(int id)
        {
            return await _dbContext.Clientes
                .FirstOrDefaultAsync(o => o.IdCliente == id);
        }

        public async Task<Proveedores?> ObtenerPorIdAsyncProveedor(int id)
        {
            return await _dbContext.Proveedores
                .FirstOrDefaultAsync(o => o.IdProveedor == id);
        }

        public async Task ActualizarProveedorAsync(Proveedores proveedores)
        {
            var proveedorExistente = await _dbContext.Proveedores
                .FirstOrDefaultAsync(o => o.IdProveedor == proveedores.IdProveedor);

            if (proveedorExistente == null)
            {

            }
            else
            {
                proveedorExistente.Nit = proveedores.Nit;
                proveedorExistente.RazonSocial = proveedores.RazonSocial;
                proveedorExistente.Direccion = proveedores.Direccion;
                proveedorExistente.Celular = proveedores.Celular;
                proveedorExistente.Correo = proveedores.Correo;

                _dbContext.Proveedores.Update(proveedorExistente);
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task ActualizarClienteAsync(Clientes clientes)
        {
            var clienteExistente = await _dbContext.Clientes
                .FirstOrDefaultAsync(o => o.IdCliente == clientes.IdCliente);

            if(clienteExistente == null)
            {

            }else
            {
                clienteExistente.NombreCliente = clientes.NombreCliente;
                clienteExistente.CiudadCliente = clientes.CiudadCliente;
                clienteExistente.TelefonoCliente = clientes.TelefonoCliente;
                clienteExistente.EmpresaCliente = clientes.EmpresaCliente;
                clienteExistente.CorreoCliente = clientes.CorreoCliente;
                clienteExistente.DireccionCliente = clientes.DireccionCliente;
                clienteExistente.CedulaCliente = clientes.CedulaCliente;

                _dbContext.Clientes.Update(clienteExistente);
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task<EmpleadoPdvViewModel> ObtenerDatosAsignacion(string cedulaUsuario)
        {
            var cedulaEmpleado = int.Parse(cedulaUsuario);
            // Paso 1: buscar la sede del usuario logueado
            var sedeEmpleadoUsuario = await _dbContext.Sedeempleado
                .FirstOrDefaultAsync(se => se.Cedula == cedulaEmpleado);

            if (sedeEmpleadoUsuario == null)
            {
                return new EmpleadoPdvViewModel
                {
                    Empleados = new List<Empleados>(),
                    PuntosDeVenta = new List<Infopdv>()
                };
            }

            var idSede = sedeEmpleadoUsuario.Id_sede;

            // Paso 2: empleados que tienen un registro en SedeEmpleado con el mismo id_sede
            var empleados = await (
                from se in _dbContext.Sedeempleado
                join e in _dbContext.Empleado on se.Cedula equals e.Cedula
                where se.Id_sede == idSede
                select e
            ).ToListAsync();

            // Paso 3: PDVs de esa sede
            var pdvs = await _dbContext.Infopdv
                .Where(p => p.Id_Sede == idSede)
                .ToListAsync();

            return new EmpleadoPdvViewModel
            {
                Empleados = empleados,
                PuntosDeVenta = pdvs
            };
        }
        //Funcionalidades de belleza
        public List<Agendamientos> AgendamientosServicios()
        {
            var modAgendamientoCliente = _dbContext.Agendamientos.ToList();
            return modAgendamientoCliente;
        }
        public bool GuardarEdicionServicio(Agendamientos model)
        {
            try
            {
                var servicio = _dbContext.Agendamientos.FirstOrDefault(x => x.Id == model.Id);
                if (servicio != null)
                {
                    servicio.NombreCliente = model.NombreCliente;
                    servicio.CelularCliente = model.CelularCliente;
                    servicio.Fecha = model.Fecha;
                    _dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        public Agendamientos ObtenerAgendamientoPorId(int id)
        {
            return _dbContext.Agendamientos.FirstOrDefault(x => x.Id == id);
        }
        public bool AprobarAgendamiento(int id)
        {
            try
            {
                var servicio = _dbContext.Agendamientos.FirstOrDefault(x => x.Id == id);
                if (servicio != null)
                {
                    servicio.Estado = "Realizado";
                    _dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        public bool EliminarAgendamiento(int id)
        {
            try
            {
                var servicio = _dbContext.Agendamientos.FirstOrDefault(x => x.Id == id);
                if (servicio != null)
                {
                    _dbContext.Agendamientos.Remove(servicio);
                    _dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        public async Task<List<Producto>> ConsultarCatProductos(int id)
        {
            return await _dbContext.Productos
                .Where(p => p.IdCatepro == id)
                .ToListAsync();
        }
        public async Task<bool> ExisteFechaAsync(string codProducto, DateTime fecha)
        {
            return await _dbContext.Agendamientos
                .AnyAsync(a => a.Cod_Producto == codProducto && a.Fecha == fecha);
        }

        // Registrar un nuevo agendamiento si no existe
        public async Task<bool> RegistrarAgendamientoAsync(string codProducto, DateTime fecha, string NombreCliente, string CelularCliente)
        {
            if (await ExisteFechaAsync(codProducto, fecha))
                return false;

            var nuevoAgendamiento = new Agendamientos
            {
                Cod_Producto = codProducto,
                Fecha = fecha,
                NombreCliente = NombreCliente,
                CelularCliente = CelularCliente,
                Estado = "Pendiente"
            };

            _dbContext.Agendamientos.Add(nuevoAgendamiento);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}

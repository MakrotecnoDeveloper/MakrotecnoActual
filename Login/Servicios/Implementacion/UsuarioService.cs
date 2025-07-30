using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

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
        public async Task<List<Empleado>> ObtenerUsuarios()
        {
            var empleados = await _dbContext.Empleado.ToListAsync();
            return empleados;
        }
        public async Task<List<Empleado>> ObtenerTodos()
        {
            return await _dbContext.Empleado.ToListAsync();
        }

        public async Task<Empleado> ObtenerPorCedula(int cedula)
        {
            return await _dbContext.Empleado.FindAsync(cedula);
        }

        public async Task InsertarEmpleado(Empleado empleado)
        {
            _dbContext.Empleado.Add(empleado);
            await _dbContext.SaveChangesAsync();
        }

        public async Task ActualizarEmpleado(Empleado empleado)
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
        public string? ObtenerNombreRolPermisos(int rolEmpleado)
        {
            return _dbContext.TipoCargo
                         .Where(tc => tc.Id_tipo == rolEmpleado)
                         .Select(tc => tc.NombreCargo)
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

        public Empleado? GetUsuarios(int cedula, string password)
        {
            Empleado? usuario_encontrando = _dbContext.Empleado.Where(u => u.Cedula == cedula && u.Contrasena == password).FirstOrDefault();
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
        public async Task<Empleado> SaveUsuario(Empleado modelo)
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
        public Empleado ObtenerEmpleadoPorId(string cedula)
        {
            return _dbContext.Empleado.Find(cedula);
        }

        public void AgregarEmpleado(Empleado empleado)
        {
            _dbContext.Empleado.Add(empleado);
            _dbContext.SaveChanges();
        }
        public List<Empleado> BuscarUsuario(int id)
        {
            if (id > 0)
            {
                var consulta = _dbContext.Empleado.Where(p => p.Cedula == id).ToList();
                return consulta;
            }
            else
            {
                return new List<Empleado>();
            }
        }
        public void EditarEmpleado(Empleado empleado, int cedula, string nombre, string apellido, string genero, string correo, string rh, string celular, string contrasena)
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
        public Empleado? ValidarCedula(int cedula)
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
        public List<FacProUserViewModel> TraerFactXDia(int cedula, int idPDVActual)
        {
            DateTime fecha = DateTime.UtcNow.Date;
            DateTime fechaInicio = new(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            DateTime fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

            int totalFacturas = _dbContext.Factura
                .Where(f => f.FechaEmision == fecha)
                .Count();


            int totalEmpleados = _dbContext.Empleado.Count();

            int totalProductos = _dbContext.Productos.Count();

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

            FacProUserViewModel viewModel = new()
            {
                TotalSumaCodFactura = totalFacturas,
                TotalEmpleados = totalEmpleados,
                TotalProductos = totalProductos,
                RolEmpleado = rolEmpleado,
                IdPDV = idPDVActual,
                NombrePDV = traerNombrePDV
            };

            return new List<FacProUserViewModel> { viewModel };
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
                     .Where(c => c.IdCatepro == 17 && c.Estado == 1)
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
                {
                    return false;
                }
            try
            {
                //Console.WriteLine(campo);
                // Obtener información de la propiedad usando reflexión
                var property = producto.GetType().GetProperty(campo);

                if (property == null)
                {
                    throw new ArgumentException($"El campo '{campo}' no existe en la clase Producto.");
                }

                // Intentar convertir y asignar el nuevo valor
                var convertedValue = Convert.ChangeType(newVal, property.PropertyType);
                property.SetValue(producto, convertedValue);

                // Guardar cambios en la base de datos
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (FormatException ex)
            {
                // Error en la conversión de tipo
                Console.WriteLine($"Error de formato: {ex.Message}");
                return false;
            }
            catch (InvalidCastException ex)
            {
                // Error de conversión de tipo
                Console.WriteLine($"Error de conversión: {ex.Message}");
                return false;
            }
            catch (ArgumentException ex)
            {
                // Error por un argumento inválido
                Console.WriteLine($"Argumento inválido: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                // Otros errores no específicos
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> InsertProInventario(string nombreProducto, int cantidadProducto, float valorNetoProductoFloat, float valorVentaProductoFloat, int valorUnidadInt, string id_empresa, int categoria, int estado, string ubicacion)
        {
            var productosObtenidos = _dbContext.Productos.Where(c => c.IdCatepro == 17).OrderBy(c => c.NombreProducto).ToList();
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
            int nuevoCodigo = codigosNumericos.Count > 0 ? codigosNumericos.Max() + 1 : 1;
            string nuevoCodigoStr = nuevoCodigo.ToString();
            /*_logger.LogInformation("Nuevo Codigo: " + nuevoCodigoStr +
                                     "Nuevo Producto: " + nombreProducto +
                                     "Cantidad Producto " + cantidadProducto +
                                     "Valor Neto " + valorNetoProductoFloat +
                                     "Valor Venta " + valorVentaProductoFloat +
                                     "Valor Unidad " + valorUnidadInt +
                                     "Id_Empresa " + id_empresa +
                                     "Categoria " + categoria +
                                     "Estado " + estado +
                                     "Ubicacion " + ubicacion);*/
            var nuevoProductoInventario = new Producto
            {
                Cod_Producto = nuevoCodigoStr,
                NombreProducto = nombreProducto,
                CantidadProducto = cantidadProducto,
                ValorNetoProducto = valorNetoProductoFloat,
                ValorVentaProducto = valorVentaProductoFloat,
                ValorUnidad = valorUnidadInt,
                ID_Empresa = id_empresa,
                IdCatepro = categoria,
                Estado = estado,
                Ubicacion = ubicacion
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
            Console.WriteLine("La cedula del trabajador es: " + cedula);
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
        public bool InsertAddClient(int cedulaCliente, string nombreCliente, string empresaCliente, string ciudadCliente, string telefonoCliente)
        {

            var cliente = new Cliente
            {
                CedulaCliente = cedulaCliente,
                NombreCliente = nombreCliente,
                EmpresaCliente = empresaCliente,
                CiudadCliente = ciudadCliente,
                TelefonoCliente = telefonoCliente
            };

            _dbContext.Cliente.Add(cliente);
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
        public List<Cliente> ServVisuaCliente()
        {
            var clientes = _dbContext.Cliente.ToList();
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
        public async Task<List<MenuOption>> GetMenuOptionsAsync(int cargoId, string idEmpresa)
        {
            return await _dbContext.MenuOption
                .Where(m => m.Id_Tipo == cargoId && m.Id_Empresa == idEmpresa)
                .ToListAsync();
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
    }
}

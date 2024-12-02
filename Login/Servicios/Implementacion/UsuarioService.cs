using Login.Models;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Servicios.Implementacion
{
    public class UsuarioService : IUsuarioService
    {
        private readonly BaseAdmContext _dbContext;
        public UsuarioService(BaseAdmContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<Empleado> ObtenerUsuarios()
        {
            return _dbContext.Empleado.ToList();
        }
        public int ObtenerRolPermisos(int cedula)
        {
            return _dbContext.Sedeempleado
                     .Where(se => se.cedula == cedula)
                     .Select(se => se.id_cargo)
                     .FirstOrDefault();
        }
        public string? ObtenerNombreRolPermisos(int rolEmpleado)
        {
            return _dbContext.TipoCargo
                         .Where(tc => tc.id_tipo == rolEmpleado)
                         .Select(tc => tc.nombreCargo)
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
	        .Where(se => se.cedula == cedula)
	        .Select(se => se.id_sede)
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
        public IEnumerable<Empleado> RegistrarEmpleado(int cedula, string nombre, string apellido, string genero, string correo, string rh, string celular, string contrasena)
        {
            var nuevoEmpleado = new Empleado
            {
                Cedula = cedula,
                Nombre = nombre,
                Apellido = apellido,
                Genero = genero,
                Correo = correo,
                Rh = rh,
                Celular = celular,
                Contrasena = contrasena
            };
            _dbContext.Empleado.Add(nuevoEmpleado);
            _dbContext.SaveChanges();
            return _dbContext.Empleado.ToList();
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
            return _dbContext.TipoCargo.FirstOrDefault(p => p.nombreCargo == nombreCargo);
        }
        public IEnumerable<TipoCargo> InsertarCargos(string nombreCargo, string descripcionCargo, string id_empresa)
        {
            var nuevoCargo = new TipoCargo
            {
                nombreCargo = nombreCargo,
                descripcionCargo = descripcionCargo,
                id_empresa = id_empresa
            };
            _dbContext.TipoCargo.Add(nuevoCargo);
            _dbContext.SaveChanges();
            return _dbContext.TipoCargo.ToList();
        }
        public Empresas? ValidarExistenciaEmpresa(string nit)
        {
            return _dbContext.Empresas.FirstOrDefault(p => p.id_empresa == nit);
        }
        public IEnumerable<Empresas> InsertarEmpresa(string nit, string nombreEmpresa, string pais, string calle, string carrera, string ciudad, string departamento, string indicativo, string numero)
        {
            var direccion = calle + " # " + carrera + ", " + ciudad + " - " + departamento;
            var celular = indicativo + " " + numero;
            var nuevaEmpresa = new Empresas
            {
                id_empresa = nit,
                nombre = nombreEmpresa,
                pais = pais,
                direccion = direccion,
                telefono = celular
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
            return _dbContext.Sede.FirstOrDefault(p => p.nombreSede == nombreSede);
        }
        public IEnumerable<Sede> InsertarSede(string id_empresa, string nombreSede, string ciudad, string direccion, string telefono)
        {
            var nuevaSede = new Sede
            {
                id_empresa = id_empresa,
                nombreSede = nombreSede,
                ciudad = ciudad,
                direccion = direccion,
                telefono = telefono
            };
            _dbContext.Sede.Add(nuevaSede);
            _dbContext.SaveChanges();
            return _dbContext.Sede.ToList();
        }
        public EmpleadoEmpresa? ValidarExisEmpleadoEmpresa(string id_empresa, int cedula)
        {
            return _dbContext.EmpleadoEmpresa.FirstOrDefault(ee => ee.id_empresa == id_empresa && ee.cedula == cedula);
        }
        public IEnumerable<EmpleadoEmpresa> InsertarEmpleadoEmpresa(string id_empresa, int cedula)
        {
            var nuevoEmpleadoEmpresa = new EmpleadoEmpresa
            {
                id_empresa = id_empresa,
                cedula = cedula
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
                        var empleado = empleados.FirstOrDefault(e => e.Cedula == se.cedula);
                        var sede = sedes.FirstOrDefault(s => s.id_sede == se.id_sede);
                        var cargo = cargos.FirstOrDefault(c => c.id_tipo == se.id_cargo);
                        var empresa = empleadoEmpresas
                            .Where(ee => ee.cedula == se.cedula)
                            .Join(empresas, ee => ee.id_empresa, emp => emp.id_empresa, (ee, emp) => emp)
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
             .Where(s => s.id_empresa == empresaId)
             .Select(s => new Sede
             {
                 id_sede = s.id_sede,
                 nombreSede = s.nombreSede
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
                .Where(ee => ee.cedula == cedula)
                .Select(ee => ee.id_empresa)
                .FirstOrDefault();
        }
        public List<Sede> ObtenerSedes(string idEmpresa)
        {
            return _dbContext.Sede.Where(s => s.id_empresa == idEmpresa).ToList();
        }

        public bool ObtenerSedePorEmpleado(int cedula)
        {
            // Buscar el registro en Sedeempleado basado en la cédula
            var empleadoSede = _dbContext.Sedeempleado.FirstOrDefault(es => es.cedula == cedula);
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
            return _dbContext.TipoCargo.Where(c => c.id_empresa == idEmpresa).ToList();
        }
        public bool InsertarSedeEmpleado(int cedula, int idSede, int idCargo)
        {
            var sedeEmpleado = new Sedeempleado
            {
                cedula = cedula,
                id_sede = idSede,
                id_cargo = idCargo
            };

            _dbContext.Sedeempleado.Add(sedeEmpleado);
            _dbContext.SaveChanges();
            return true;
        }
        public List<FacProuserViewModel> TraerFactXDia(int cedula, int idPDVActual)
        {
            DateTime fecha = DateTime.UtcNow.Date;
            DateTime fechaInicio = new(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            DateTime fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

            int totalFacturas = _dbContext.Factura
                .Where(f => f.fechaVenta == fecha)
                .Count();

            decimal sumaValorVenta = _dbContext.Pedidos
            .Join(
                _dbContext.Factura,
                pedido => pedido.cod_factura,
                factura => factura.cod_factura,
                (pedido, factura) => new { pedido, factura })
            .Where(x => x.factura.fechaVenta == fecha)
            .Sum(x => (decimal?)x.pedido.valorVenta) ?? 0;


            int totalEmpleados = _dbContext.Empleado.Count();

            int totalProductos = _dbContext.Productos.Count();

            string rolEmpleado = string.Empty;
                var rolEmpleadoResult = (from se in _dbContext.Sedeempleado
                                         join tc in _dbContext.TipoCargo
                                         on se.id_cargo equals tc.id_tipo
                                         where se.cedula == cedula
                                         select tc.nombreCargo).FirstOrDefault();

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
          .Select(ce => ce.Name)
          .FirstOrDefault();

            FacProuserViewModel viewModel = new()
            {
                TotalSumaCodFactura = totalFacturas,
                TotalVentaDia = sumaValorVenta,
                TotalEmpleados = totalEmpleados,
                TotalProductos = totalProductos,
                RolEmpleado = rolEmpleado,
                IdPDV = idPDVActual,
                NombrePDV = traerNombrePDV
            };

            return new List<FacProuserViewModel> { viewModel };
        }
        public async Task<IEnumerable<ClientesPlataforma>> ObtenerCuentasProximas(int idPlataforma)
        {
            var fechaActual = DateTime.Now;
            var cuentasProximas = await _dbContext.ClientesPlataforma
                .Where(cp => cp.idPltfSuscripcion == idPlataforma && cp.fechaFinPago <= fechaActual.AddDays(5))
                .ToListAsync();

            return cuentasProximas;
        }
        public List<Producto> ProductosAbarrotes()
        {
            return _dbContext.Productos
                     .Where(c => c.Categoria == "Abarrotes" && c.estado == 1)
                     .OrderBy(c => c.NombreProducto)
                     .ToList();
        }
        public async Task<bool> ActualizarProductoAsync(string id, string campo, string newVal)
        {
                var producto = await _dbContext.Productos.FirstOrDefaultAsync(p => p.Cod_Producto == id);

                if (producto == null)
                {
                    return false;
                }

                var property = producto.GetType().GetProperty(campo);
                property?.SetValue(producto, Convert.ChangeType(newVal, property.PropertyType));

                await _dbContext.SaveChangesAsync();
                return true;
        }
        public async Task<bool> InsertProInventario(string nombreProducto, int cantidadProducto, float valorNetoProductoFloat, float valorVentaProductoFloat, int valorUnidadInt, string id_empresa, string categoria, int estado, string ubicacion)
        {
            var productosObtenidos = _dbContext.Productos.Where(c => c.Categoria == "Abarrotes").OrderBy(c => c.NombreProducto).ToList();
            var codigosNumericos = productosObtenidos
            .Select(p => {
                bool isNumeric = int.TryParse(p.Cod_Producto, out int numero);
                return new { Producto = p, Numero = isNumeric ? (int?)numero : null };
            })
            .Where(p => p.Numero.HasValue)
            .Select(p => p.Numero.GetValueOrDefault())
            .ToList();
            int nuevoCodigo = codigosNumericos.Count > 0 ? codigosNumericos.Max() + 1 : 1;
            string nuevoCodigoStr = nuevoCodigo.ToString();
            var nuevoProductoInventario = new Producto
            {
                Cod_Producto = nuevoCodigoStr,
                NombreProducto = nombreProducto,
                CantidadProducto = cantidadProducto,
                ValorNetoProducto = valorNetoProductoFloat,
                ValorVentaProducto = valorVentaProductoFloat,
                valorUnidad = valorUnidadInt,
                ID_Empresa = id_empresa,
                Categoria = categoria,
                estado = estado,
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
                producto.estado = 0;

                await _dbContext.SaveChangesAsync();

                return true;
            }

            return false;
        }
        public Infopdv? SeleccionarNombrePDV(int selectedPDV)
        {
            return _dbContext.Infopdv.FirstOrDefault(p => p.InfopdvId == selectedPDV);
        }
        public int? ValidarExistenteIdPDV(int idPDV, int cedula)
        {
            return _dbContext.Syncpdv
                .Where(s => s.InfopdvId == idPDV && s.Cedula == cedula)
                .OrderByDescending(s => s.Idsync)
                .Select(s => s.estado)
                .FirstOrDefault();
        }
        public Syncpdv AgregarEstadoPDV(int estadopdv, int idPDV, int cedula)
        {
            Console.WriteLine("La cedula del trabajador es: " + cedula);
            var nuevoEstadoPDV = new Syncpdv
            {
                InfopdvId = idPDV,
                estado = estadopdv,
                fechaEstado = DateTime.Now,
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
                cedulaCliente = cedulaCliente,
                nombreCliente = nombreCliente,
                empresaCliente = empresaCliente,
                ciudadCliente = ciudadCliente,
                telefonoCliente = telefonoCliente
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
                nit = nit,
                razonSocial = razonSocial,
                direccion = direccion,
                celular = celular,
                correo = correo
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
    }
}

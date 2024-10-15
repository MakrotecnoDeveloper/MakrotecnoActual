using Login.Models;
using Microsoft.EntityFrameworkCore;
using OpenAI_API.Moderation;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using System.Security.Claims;

namespace Plataforma.Servicios.Implementacion
{
    public class UsuarioService : IUsuarioService
    {
        //variable de solo lectura para referenciar la base de datos
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
            var idCargo = _dbContext.Sedeempleado
                     .Where(se => se.cedula == cedula)
                     .Select(se => se.id_cargo)
                     .FirstOrDefault();
            if (idCargo < 0)
            {
                throw new Exception("El Usuario no tiene una sede y un rol sincronizado.");
            }
            else
            {
                return idCargo;
            }
        }
        public string ObtenerNombreRolPermisos(int rolEmpleado)
        {
            var nombreCargo = _dbContext.TipoCargo
                         .Where(tc => tc.id_tipo == rolEmpleado)
                         .Select(tc => tc.nombreCargo)
                         .FirstOrDefault();
            if(nombreCargo == null)
            {
                throw new Exception("No tiene creado un cargo para dicha empresa.");
            }
            else
            {
                return nombreCargo;
            }
        }
        public Sedeempleado ObtenerSedeEmpleadoPorCedula(int cedulaEmpleado)
        {
            var valorSedeEmpleadoCedula = _dbContext.Sedeempleado.FirstOrDefault(se => se.cedula == cedulaEmpleado);
            if(valorSedeEmpleadoCedula != null)
            {
                return valorSedeEmpleadoCedula;
            }else
            {
                throw new Exception("El empleado no tiene conexion con una sede y cargo, porfavor crearla.");
            }
        }
        public async Task<logsLogin> InsertarLogLogin(int cedulaEmpleado, string correoEmpleado, int estado)
        {
            var utcNow = DateTime.UtcNow;
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            var localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, timeZone);
            var login = new logsLogin
            {
                Cedula = cedulaEmpleado,
                Correo = correoEmpleado,
                Fecha = localDateTime,
                Estado = estado // Sesión activa
            };
            // Guardar información de sesión en la base de datos
            _dbContext.LogsLogin.Add(login);
            await _dbContext.SaveChangesAsync();

            return login;
        }
        public Empleado GetUsuarios(int cedula, string password)
        {
            Empleado? usuario_encontrando = _dbContext.Empleado.Where(u => u.Cedula == cedula && u.Contrasena == password).FirstOrDefault();
            if (usuario_encontrando == null)
            {
                return null;
            }

            return usuario_encontrando;
        }
        public List<Infopdv> funValidarPDV(int cedula)
        {
			// Primero, buscamos al empleado en la tabla sedeempleado
			var idSede = _dbContext.Sedeempleado
	        .Where(se => se.cedula == cedula)
	        .Select(se => se.id_sede)
	        .FirstOrDefault();
			// Verificamos si el empleado fue encontrado
			if (idSede <= 0)
			{
				return new List<Infopdv>();
			}
			// Ahora, usamos el id_sede para buscar las PDVs en la tabla infopdv
			var pdvs = _dbContext.Infopdv
	        .Where(p => p.Id_Sede == idSede)
	        .ToList();
			// Retornamos la lista de PDVs asociadas a esa sede
			return pdvs;

		}
        public async Task<Empleado> SaveUsuario(Empleado modelo)
        {
            _dbContext.Empleado.Add(modelo);
            await _dbContext.SaveChangesAsync();
            return modelo;
        }
        public bool validarEmpleado(int cedula)
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
        public void EditarEmpleado(int cedula, string nombre, string apellido, string genero, string correo, string rh, string celular, string contrasena)
        {
            var empleado = _dbContext.Empleado.FirstOrDefault(p => p.Cedula == cedula);
            if (empleado != null)
            {
                empleado.Cedula = cedula;
                empleado.Nombre = nombre;
                empleado.Apellido = apellido;
                empleado.Genero = genero;
                empleado.Correo = correo;
                empleado.Rh = rh;
                empleado.Celular = celular;
                empleado.Contrasena = contrasena;
                try
                {
                    _dbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al guardar los cambios en la base de datos: " + ex.Message);
                }
            }
            else
            {
                throw new Exception("El empleado no existe");
            }
        }
        public List<TipoCargo> ObtenerCargos()
        {
            return _dbContext.TipoCargo.ToList();
        }
        public List<Empresas> ObtenerEmpresas()
        {
            return _dbContext.Empresas.ToList();
        }
        public IEnumerable<TipoCargo> InsertarCargos(string nombreCargo, string descripcionCargo, string id_empresa)
        {
            var cargoExistente = _dbContext.TipoCargo.FirstOrDefault(p => p.nombreCargo == nombreCargo);
            if (cargoExistente != null)
            {
                throw new Exception("Ya existe este cargo");
            }
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
        public IEnumerable<Empresas> InsertarEmpresa(string nit, string nombreEmpresa, string pais, string calle, string carrera, string ciudad, string departamento, string indicativo, string numero)
        {
            var empresaExiste = _dbContext.Empresas.FirstOrDefault(p => p.id_empresa == nit);
            if(empresaExiste != null)
            {
                throw new Exception("Ya existe la empresa");
            }
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
        public IEnumerable<Sede> InsertarSede(string id_empresa, string nombreSede, string ciudad, string direccion, string telefono)
        {
            var sedeExiste = _dbContext.Sede.FirstOrDefault(p => p.nombreSede == nombreSede);
            if (sedeExiste != null)
            {
                throw new Exception("Ya existe la sede");
            }
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
        public IEnumerable<EmpleadoEmpresa> InsertarEmpleadoEmpresa(string id_empresa, int cedula)
        {
            var empleadoExisteEmpresaExiste = _dbContext.EmpleadoEmpresa.FirstOrDefault(ee => ee.id_empresa == id_empresa && ee.cedula == cedula);
            if (empleadoExisteEmpresaExiste != null)
            {
                throw new Exception("Ya existe la relación entre este empleado y esta empresa.");
            }
            Console.WriteLine("La empresa con IDEmpresa: " + id_empresa);
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
            var sedes = _dbContext.Sede
             .Where(s => s.id_empresa == empresaId)
             .Select(s => new Sede
             {
                 id_sede = s.id_sede,
                 nombreSede = s.nombreSede
             })
             .ToList();

            return sedes;
        }
        public Empleado ValidarCedula(int cedula)
        {
            var empleado = _dbContext.Empleado.FirstOrDefault(e => e.Cedula == cedula);
            return empleado ?? throw new Exception("Empleado no encontrado (VC)");
        }
        public string? ObtenerIdEmpresa(int cedula)
        {
            var empleadoEmpresa = _dbContext.EmpleadoEmpresa.FirstOrDefault(ee => ee.cedula == cedula);
            return empleadoEmpresa?.id_empresa;
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
        public List<FacProuserViewModel> TraerFactXDia(int cedula)
        {
            DateTime fecha = DateTime.UtcNow.Date;
            //DateTime fechaInicio = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            DateTime fechaInicio = new(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            DateTime fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

            int totalFacturas = _dbContext.Factura
                .Where(f => f.fechaVenta == fecha)
                .Count();
            decimal sumaValorVenta = (from pedido in _dbContext.Pedidos
                                  join factura in _dbContext.Factura
                                  on pedido.cod_factura equals factura.cod_factura
                                  where factura.fechaVenta == fecha
                                      select pedido.valorVenta).Sum();
            int totalEmpleados = _dbContext.Empleado.Count();
            int totalProductos = _dbContext.Productos.Count();
            var productosMasVendidos = (from pedido in _dbContext.Pedidos
                                        join factura in _dbContext.Factura
                                        on pedido.cod_factura equals factura.cod_factura
                                        join producto in _dbContext.Productos
                                        on pedido.cod_producto equals producto.Cod_Producto
                                        where factura.fechaVenta >= fechaInicio && factura.fechaVenta <= fechaFin
                                        group pedido by new { pedido.cod_producto, producto.NombreProducto } into grouped
                                        orderby grouped.Sum(p => p.cantidad) descending
                                        select new ProductoMasVendidoViewModel
                                        {
                                            NombreProducto = grouped.Key.NombreProducto,
                                            CantidadVendida = grouped.Sum(p => p.cantidad)
                                        }).Take(5).ToList();

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
                    throw new Exception("Empleado no encontrado  (TFXD)");

                }

            //Traer Plataformas
            var plataformas = _dbContext.Plataformas.ToList();

            // Crear el ViewModel con la suma total
            FacProuserViewModel viewModel = new()
            {
                TotalSumaCodFactura = totalFacturas,
                TotalVentaDia = sumaValorVenta,
                TotalEmpleados = totalEmpleados,
                TotalProductos = totalProductos,
                ProductosMasVendidos = productosMasVendidos,
                RolEmpleado = rolEmpleado,
                Plataformas = plataformas
            };

            // Devolver una lista con el ViewModel
            return new List<FacProuserViewModel> { viewModel };
        }
        public async Task<IEnumerable<ClientesPlataforma>> ObtenerCuentasProximas(int idPlataforma)
        {
            var fechaActual = DateTime.Now;
            var cuentasProximas = await _dbContext.ClientesPlataforma
                .Where(cp => cp.idPlataforma == idPlataforma && cp.fechaFinPago <= fechaActual.AddDays(5))
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
            try
            {
                // Obtener el producto a actualizar
                var producto = await _dbContext.Productos.FirstOrDefaultAsync(p => p.Cod_Producto == id);

                if (producto == null)
                {
                    return false;
                }

                // Usar reflexión para asignar el nuevo valor al campo específico
                var property = producto.GetType().GetProperty(campo);
                if (property != null)
                {
                    property.SetValue(producto, Convert.ChangeType(newVal, property.PropertyType));
                }

                // Guardar cambios en la base de datos
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Loguear o manejar la excepción según sea necesario
                return false;
            }
        }
        public async Task<bool> insertProInventario(string nombreProducto, int cantidadProducto, float valorNetoProductoFloat, float valorVentaProductoFloat, int valorUnidadInt, string id_empresa, string categoria, int estado, string ubicacion)
        {
            var productosObtenidos = _dbContext.Productos.Where(c => c.Categoria == "Abarrotes").OrderBy(c => c.NombreProducto).ToList();
            var codigosNumericos = productosObtenidos
            .Select(p => {
                int numero;
                bool isNumeric = int.TryParse(p.Cod_Producto, out numero);
                return new { Producto = p, Numero = isNumeric ? (int?)numero : null };
            })
            .Where(p => p.Numero.HasValue)
            .Select(p => p.Numero.Value)
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

        public async Task<bool> eliminarProductoXIdAsync(string id)
        {
            // Buscamos el producto con el ID dado
            var producto = await _dbContext.Productos.FirstOrDefaultAsync(c => c.Cod_Producto == id);

            if (producto != null)
            {
                // Modificamos el campo Estado a 1
                producto.estado = 0;

                // Guardamos los cambios en la base de datos
                await _dbContext.SaveChangesAsync();

                return true;
            }

            // Retorna falso si no se encuentra el producto
            return false;
        }
        public Infopdv seleccionarNombrePDV(int selectedPDV)
        {
            return _dbContext.Infopdv.FirstOrDefault(p => p.Id == selectedPDV);
        }
        public Syncpdv ValidarExistenteIdPDV(int idPDV)
        {
            return _dbContext.Syncpdv.FirstOrDefault(s => s.Id == idPDV);
        }
        public Syncpdv AgregarEstadoPDV(int estadopdv, int idPDV)
        {
            var nuevoEstadoPDV = new Syncpdv
            {
                Id = idPDV,
                estado = estadopdv,
                fechaEstado = DateTime.Now
            };
            _dbContext.Syncpdv.Add(nuevoEstadoPDV);
            _dbContext.SaveChanges();
            return nuevoEstadoPDV;
        }
    }
}

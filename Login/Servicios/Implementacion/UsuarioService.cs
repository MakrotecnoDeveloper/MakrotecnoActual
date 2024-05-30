using Login.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public async Task<Empleado> GetUsuarios(string correo, string password)
        {
            Empleado usuario_encontrando = await _dbContext.Empleado.Where(u => u.Correo == correo && u.Contrasena == password).FirstOrDefaultAsync();

            return usuario_encontrando;
        }

        public async Task<Empleado> SaveUsuario(Empleado modelo)
        {
            _dbContext.Empleado.Add(modelo);
            await _dbContext.SaveChangesAsync();
            return modelo;
        }
        public IEnumerable<Empleado> RegistrarEmpleado(int cedula, string nombre, string apellido, string genero, string correo, string rh, string celular, string contrasena)
        {
            var empleadoExistente = _dbContext.Empleado.FirstOrDefault(p => p.Cedula == cedula);
            if (empleadoExistente != null)
            {
                // Si ya existe un empleado con la misma cédula, puedes manejarlo de acuerdo a tus requerimientos, por ejemplo, lanzar una excepción, devolver un mensaje de error, etc.
                // Aquí estoy lanzando una excepción como ejemplo.
                Console.WriteLine("Ya existe un empleado con la misma cédula");
            }

            // Crear una nueva instancia de Empleado
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

            // Agregar el nuevo empleado al contexto de la base de datos
            _dbContext.Empleado.Add(nuevoEmpleado);

            // Guardar los cambios en la base de datos
            _dbContext.SaveChanges();

            // Retornar todos los empleados después de agregar el nuevo empleado
            return _dbContext.Empleado.ToList();
        }
        public List<Empleado> BuscarUsuario(int id)
        {
            // Lógica para buscar productos por el nombre o la categoría
            if (id > 0)
            {
                var consulta = _dbContext.Empleado.Where(p => p.Cedula == id).ToList();
                return consulta;
            }
            else
            {
                // Ambos términos están vacíos, puedes manejarlo según tus necesidades
                return new List<Empleado>();
            }
        }
        public void EditarEmpleado(int cedula, string nombre, string apellido, string genero, string correo, string rh, string celular, string contrasena)
        {

            if (cedula < 0 || nombre == null || apellido == null || genero == null || correo == null || rh == null || celular == null || contrasena == null)
            {
                Console.WriteLine("Error: Todos los campos deben tener un valor. No se permiten valores nulos.");
                return;
            }

            var empleado = _dbContext.Empleado.FirstOrDefault(p => p.Cedula == cedula);
            //Console.WriteLine("El ID de la empresa es: " + idEmpresa);
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
                    Console.WriteLine("Error al guardar los cambios en la base de datos: " + ex.Message);
                    // Puedes agregar un código adicional aquí para manejar el error, como registrar el error en un archivo de registro, notificar al usuario, etc.
                }
            }
            else
            {
                Console.WriteLine("El miembro no existe");
                // Puedes agregar un código adicional aquí si necesitas manejar el caso en que el producto no exista
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
                Console.WriteLine("Ya existe este cargo");
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
                Console.WriteLine("Ya existe la empresa");
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
                Console.WriteLine("Ya existe la sede");
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
        public IEnumerable<EmpleadoEmpresa> InsertarEmpleadoEmpresa(string idEmpresa, int cedula)
        {
            var empleadoExisteEmpresaExiste = _dbContext.EmpleadoEmpresa.FirstOrDefault(ee => ee.id_empresa == idEmpresa && ee.cedula == cedula);
            if (empleadoExisteEmpresaExiste != null)
            {
                Console.WriteLine("Ya existe la relación entre este empleado y esta empresa.");
            }
            var nuevoEmpleadoEmpresa = new EmpleadoEmpresa
            {
                id_empresa = idEmpresa,
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
                    })
                    .Where(e => e.Empleado != null && e.Sede != null && e.Empresa != null && e.TipoCargo != null)
                    .ToList();

                return new EmpleadoSedeViewModel
                {
                    Empleados = empleados,
                    Empresas = empresas,
                    Sedes = sedes,
                    EmpleadoConSedeYEmpresas = empleadoConSedeYCargo
                };
            }

            return null;
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
            return empleado;
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

        public Sede ObtenerSedePorEmpleado(int cedula)
        {
            var empleadoSede = _dbContext.Sedeempleado.FirstOrDefault(es => es.cedula == cedula);
            if (empleadoSede == null)
            {
                return null;
            }

            // Obtener la sede correspondiente usando el id_sede
            var sede = _dbContext.Sede.FirstOrDefault(s => s.id_sede == empleadoSede.id_sede);
            return sede;
        }
        public List<TipoCargo> ObtenerCargos(string idEmpresa)
        {
            return _dbContext.TipoCargo.Where(c => c.id_empresa == idEmpresa).ToList();
        }
        public List<Sedeempleado> InsertarSedeEmpleado(int cedula, int idSede, int idCargo)
        {
            var sedeEmpleado = new Sedeempleado
            {
                cedula = cedula,
                id_sede = idSede,
                id_cargo = idCargo
            };

            _dbContext.Sedeempleado.Add(sedeEmpleado);
            _dbContext.SaveChanges();
            return _dbContext.Sedeempleado.ToList();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

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
    }
}

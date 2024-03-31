//establece un contrato que define las operaciones necesarias para interactuar con usuarios en una aplicación
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IUsuarioService
    {
        List<Empleado> ObtenerUsuarios();
        Task<Empleado> GetUsuarios(string correo, string password);
        //el segundo metodo guarda usuarios
        Task<Empleado> SaveUsuario(Empleado modelo);
        IEnumerable<Empleado> RegistrarEmpleado(int cedula, string nombre, string apellido, string genero, string correo, string rh, string celular, string contrasena);
    }
}

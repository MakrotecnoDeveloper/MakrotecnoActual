//establece un contrato que define las operaciones necesarias para interactuar con usuarios en una aplicación
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IUsuarioService
    {
        //primer metodo devuelve un usuario atravez del correo y la contraseña
        Task<Empleado> GetUsuarios(string correo, string password);
        //el segundo metodo guarda usuarios
        Task<Empleado> SaveUsuario(Empleado modelo);
    }
}

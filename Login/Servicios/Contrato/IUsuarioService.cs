//establece un contrato que define las operaciones necesarias para interactuar con usuarios en una aplicación
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IUsuarioService
    {
        //primer metodo devuelve un usuario atravez del correo y la contraseña
        Task<Usuario> GetUsuarios(string correo, string password);
        //el segundo metodo guarda usuarios
        Task<Usuario> SaveUsuario(Usuario modelo);
    }
}

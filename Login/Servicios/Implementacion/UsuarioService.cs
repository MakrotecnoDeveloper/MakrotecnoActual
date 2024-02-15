using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Servicios.Implementacion
{
    public class UsuarioService : IUsuarioService
    {
        //variable de solo lectura para referenciar la base de datos
        private readonly PruebaDbContext _dbContext;
        public UsuarioService(PruebaDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Usuario> GetUsuarios(string correo, string password)
        {
            Usuario usuario_encontrando = await _dbContext.Usuarios.Where(u => u.Correo == correo && u.Clave == password).FirstOrDefaultAsync();

            return usuario_encontrando;
        }

        public async Task<Usuario> SaveUsuario(Usuario modelo)
        {
            _dbContext.Usuarios.Add(modelo);
            await _dbContext.SaveChangesAsync();
            return modelo;
        }
    }
}

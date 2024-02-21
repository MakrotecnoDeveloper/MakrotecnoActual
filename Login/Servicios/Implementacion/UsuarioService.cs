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
    }
}

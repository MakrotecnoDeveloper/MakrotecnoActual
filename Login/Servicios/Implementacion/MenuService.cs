using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Servicios.Implementacion
{
    public class MenuService : IMenuService
    {
        private readonly BaseAdmContext _dbContext;
        private readonly ILogger<MenuService> _logger;
        public MenuService(BaseAdmContext dbContext, ILogger<MenuService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public async Task<List<int>> ObtenerIdsMenusActivosPorCargoAsync(int idCargo)
        {
            return await _dbContext.MkPermisosMenus
                .Where(p => p.Id_TipoCargo == idCargo && p.Estado == true)
                .Select(p => p.Id_Menu)
                .ToListAsync();
        }
        public async Task ActualizarMenusPorCargoAsync(int idCargo, List<int> idsSeleccionados)
        {
            var actuales = await _dbContext.MkPermisosMenus
                .Where(p => p.Id_TipoCargo == idCargo)
                .ToListAsync();

            foreach (var permiso in actuales)
            {
                permiso.Estado = idsSeleccionados.Contains(permiso.Id_Menu);
            }

            var nuevos = idsSeleccionados
                .Where(id => !actuales.Any(p => p.Id_Menu == id))
                .Select(id => new MkPermisosMenu
                {
                    Id_TipoCargo = idCargo,
                    Id_Menu = id,
                    Estado = true
                }).ToList();

            if (nuevos.Any())
                await _dbContext.MkPermisosMenus.AddRangeAsync(nuevos);

            await _dbContext.SaveChangesAsync();
        }
        public async Task<List<TipoCargo>> ObtenerTodosLosCargosAsync()
        {
            return await _dbContext.TipoCargo.ToListAsync();
        }
        public async Task<List<MkMenu>> ObtenerTodosMenusAsync()
        {
            return await _dbContext.MkMenus.ToListAsync();
        }

    }
}
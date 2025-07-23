using Microsoft.AspNetCore.Mvc;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Controllers
{
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;
        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }
        public async Task<IActionResult> ConfiguracionMenu()
        {
            var cargos = await _menuService.ObtenerTodosLosCargosAsync(); // tabla TipoCargo
            return View(cargos); // Pasas lista de roles al select
        }
        public async Task<IActionResult> ObtenerMenusPorCargo(int idCargo)
        {
            var todosMenus = await _menuService.ObtenerTodosMenusAsync(); // tabla MkMenu
            var idsActivos = await _menuService.ObtenerIdsMenusActivosPorCargoAsync(idCargo); // tabla MkPermisosMenu

            var modelo = todosMenus.Select(m => new MenuConEstadoDTO
            {
                IdMenu = m.Id,
                NombreMenu = m.Nombre,
                EstaActivo = idsActivos.Contains(m.Id)
            }).ToList();

            return PartialView("_CheckboxesMenus", modelo);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarMenusPorCargo([FromBody] MenuActualizacionDTO dto)
        {
            await _menuService.ActualizarMenusPorCargoAsync(dto.idCargo, dto.idsMenu);
            return Ok();
        }

    }
}
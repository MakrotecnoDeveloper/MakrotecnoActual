using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IMenuService
    {
       Task<List<int>> ObtenerIdsMenusActivosPorCargoAsync(int idCargo);
       Task ActualizarMenusPorCargoAsync(int idCargo, List<int> idsSeleccionados);
       Task<List<TipoCargo>> ObtenerTodosLosCargosAsync();
       Task<List<MkMenu>> ObtenerTodosMenusAsync();
    }
}
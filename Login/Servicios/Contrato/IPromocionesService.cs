using Plataforma.Models;

namespace Plataforma.Servicios.Contrato
{
    public interface IPromocionesService
    {
        Task<List<MkPromociones>> GetListAsync();
        Task<MkPromociones?> GetByIdAsync(int id);
        Task<MkPromociones> CrearAsync(MkPromociones nuevo);
        Task<MkPromociones?> EditarAsync(int id, MkPromociones cambios);
        Task<bool> EliminarAsync(int id);
    }
}

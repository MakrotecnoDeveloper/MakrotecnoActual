using DocumentFormat.OpenXml.Bibliography;
using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IGastosService
    {
        Task<List<GastosMensuales>> ListarPorMesAsync(int year, int month);
        Task<GastosMensuales> CrearAsync(GastosMensuales nuevo);
        Task<GastosMensuales?> EditarAsync(int id, GastosMensuales cambios);
        Task<bool> EliminarAsync(int id);
        Task<GastosMensuales?> ObtenerAsync(int id);
    }
}

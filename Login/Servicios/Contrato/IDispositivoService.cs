using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IDispositivoService
    {
        Task CrearDispositivoAsync(Dispositivo dispositivo);
        Task<List<Dispositivo>> ObtenerDispositivosConClientesAsync();
        Task<Dispositivo?> ObtenerPorIdAsync(int idDispositivo);
        Task ActualizarDispositivoAsync(Dispositivo dispositivo);
    }
}

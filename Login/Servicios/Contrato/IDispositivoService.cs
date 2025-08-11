using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IDispositivoService
    {
        Task<List<TipoDispositivos>> ObtenerTipoDispositivos();
        Task CrearDispositivoAsync(Dispositivos dispositivo, string cedulaClaim);
        Task<List<Dispositivos>> ObtenerDispositivosConClientesAsync();
        Task<Dispositivos?> ObtenerPorIdAsync(int idDispositivo);
        Task ActualizarDispositivoAsync(Dispositivos dispositivo);
    }
}

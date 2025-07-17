using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IDispositivoService
    {
        Task AgregarDispositivoAsync(Dispositivo dispositivo);
    }
}

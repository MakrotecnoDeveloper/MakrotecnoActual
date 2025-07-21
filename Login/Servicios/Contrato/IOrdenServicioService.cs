using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IOrdenServicioService
    {
        Task AgregarDispositivoAsync(Dispositivo dispositivo);
    }
}

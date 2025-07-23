using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IOrdenServicioService
    {
        Task AgregarDispositivoAsync(Dispositivo dispositivo);
        Task<List<OrdenServicio>> ObtenerTodasAsync();
        Task<OrdenServicio?> ObtenerPorIdAsync(int id);
        Task CrearAsync(OrdenServicio orden);
        Task ActualizarAsync(OrdenServicio orden, int cedulaEmpleado);
    }
}

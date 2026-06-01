using Plataforma.Models;
using Plataforma.Models.Dto.Streaming;
namespace Plataforma.Servicios.Contrato
{
    public interface IStreamingService
    {
        void InserPlataformaService(int idPlataforma, string descripcion, int valorventa, int valorneto, DateTime fechaInipago, DateTime fechaFinpago, int cantidad, string correo, string contrasena, int cedula, int estado);
        List<Plataformas> TraerPlataformasExistentes();
        List<Plataformasuscripcion> SuscripcionesActivas();
        Task<List<Plataformasuscripcion>> ObtenerSuscripcionesActivas(int plataformaId);
        Task<List<ClientePlataformaDTO>> ObtenerDatosSuscripcion(int suscripcionId);
        Task<List<ClientePlataformaDTO>> ObtenerDatosPlataforma(int suscripcionId);
        Task<bool> EliminarClienteAsync(int idClientePlataforma);
        void ServicioInsertarVentClientPlataforma(string nombrecliente, string celularcliente, string correo, string contrasena, int idPltfSuscripcion, int cantidad, string ppm, DateTime feciniplat, DateTime fecfinplat, int valorventa, int valorneto, int cedula, int estado, string clave);
        Task ActualizarCliente(int id, int estado, int idCliente);
        Task<List<ClienteStreamingBusquedaDTO>> BuscarClientesStreamingAsync(string termino);

        Task<List<CuentaClienteStreamingDTO>> ObtenerCuentasPorClientesAsync(string idsClienteStreaming);
        Task<(bool ok, string mensaje)> ActualizarCuentaClienteStreamingAsync(ActualizarCuentaClienteStreamingDTO model);

    }
}
using Plataforma.Models;

namespace Plataforma.Servicios.Contrato
{
    public interface IStreamingPlataformService
    {
        void InserPlataformaService(int idPlataforma, string descripcion, int valorventa, int valorneto, DateTime fechaInipago, DateTime fechaFinpago, int cantidad, string correo, string contrasena, int cedula, int estado);
        void ServicioInsertarVentClientPlataforma(string nombrecliente, string celularcliente, string correo, string contrasena, int idPltfSuscripcion, int cantidad, string ppm, DateTime feciniplat, DateTime fecfinplat, int valorventa, int valorneto, int cedula, int estado, string clave);
        List<Plataformas> TraerPlataformasExistentes();
        List<Plataformasuscripcion> SuscripcionesActivas();
        Task<List<Plataformasuscripcion>> ObtenerSuscripcionesActivas(int plataformaId);
        Task<List<ClientePlataformaDTO>> ObtenerDatosSuscripcion(int suscripcionId);
        Task<List<ClientePlataformaDTO>> ObtenerDatosPlataforma(int suscripcionId);
        Task<bool> EliminarClienteAsync(int idClientePlataforma);
        Task ActualizarCliente(int id, int estado, int idCliente);
        //Task<bool> ActualizarEstadoCliente(int idClientePlataforma, int estado);
    }
}
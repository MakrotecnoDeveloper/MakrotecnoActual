using Plataforma.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plataforma.Services.Contracts
{
    public interface IPlataformaWeb
    {
        void InserPlataformaService(int idPlataforma, string descripcion, int valorventa, int valorneto, DateTime fechaInipago, DateTime fechaFinpago, int cantidad, string correo, string contrasena, int cedula, int estado);
        List<Plataformas> TraerPlataformasExistentes();
        List<Plataformasuscripcion> SuscripcionesActivas();
        Task<List<Plataformasuscripcion>> ObtenerSuscripcionesActivas(int plataformaId);
        Task<List<ClientePlataformaDTO>> ObtenerDatosSuscripcion(int suscripcionId);
        Task<List<ClientePlataformaDTO>> ObtenerDatosPlataforma(int suscripcionId);
    }
}

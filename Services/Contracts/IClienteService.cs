using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plataforma.Services.Contracts
{
    public interface IClienteService
    {
        Task<bool> EliminarClienteAsync(int idClientePlataforma);
        void ServicioInsertarVentClientPlataforma(string nombrecliente, string celularcliente, string correo, string contrasena, int idPltfSuscripcion, int cantidad, string ppm, DateTime feciniplat, DateTime fecfinplat, int valorventa, int valorneto, int cedula, int estado, string clave);
        Task ActualizarCliente(int id, int estado, int idCliente);


    }
}

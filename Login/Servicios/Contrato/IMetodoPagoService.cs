using DocumentFormat.OpenXml.Bibliography;
using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IMetodoPagoService
    {
        Task<CxcVentas> CrearCuentaPorCobrarAsync(int idVenta, int idCliente, decimal total);
        Task<bool> RegistrarPagoCxcAsync(int idCxc, decimal monto, string observacion = null);
        Task<CxcVentas> ObtenerCxcPorVentaAsync(int idVenta);
    }
}

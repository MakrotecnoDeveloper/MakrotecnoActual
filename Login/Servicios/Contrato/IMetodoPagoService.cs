using DocumentFormat.OpenXml.Bibliography;
using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IMetodoPagoService
    {
        Task<CxcVenta> CrearCuentaPorCobrarAsync(int idVenta, int idCliente, decimal total);
        Task<bool> RegistrarPagoCxcAsync(int idCxc, decimal monto, string observacion = null);
        Task<CxcVenta> ObtenerCxcPorVentaAsync(int idVenta);
    }
}

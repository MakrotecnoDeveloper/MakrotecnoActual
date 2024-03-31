using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IPedidoService
    {
        List<Factura> ObtenerFacturas();
        void ActualizarEstadoFacturas();
        IEnumerable<Factura> CrearFactura(int cod_factura, int cedula_cliente, int cedula_empleado, DateTime fechaVenta, string estado);
    }
}
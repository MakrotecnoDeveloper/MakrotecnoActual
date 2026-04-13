using Plataforma.Models;
using Plataforma.Models.ViewModels.Cxc;
namespace Plataforma.Servicios.Contrato;

    public interface ICxcService
    {
        Task<(bool Ok, string Mensaje, int? IdCxc)> CrearDesdeVentaAsync(
            int idVenta,
            int idCliente,
            decimal totalCredito,
            DateTime? fechaVencimiento,
            string? observacion = null);

        Task<(bool Ok, string Mensaje)> RegistrarPagoAsync(RegistrarPagoCxcViewModel model);

        Task<(bool Ok, string Mensaje)> RecalcularSaldoAsync(int idCxc);

        Task<CxcVentas?> ObtenerPorVentaAsync(int idVenta);

        Task<CxcDetalleViewModel?> ObtenerDetalleAsync(int idCxc);

        Task<List<MetodoPagos>> ObtenerMetodosPagoAsync();
        Task<CxcIndexViewModel> ObtenerListadoAsync(string? texto, string? estado);
    }
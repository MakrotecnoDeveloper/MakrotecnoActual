using Plataforma.Models;

namespace Plataforma.Servicios.Contrato
{
    public interface IFlujoCajaService
    {
        Task<List<FlujoCaja>> ObtenerMovimientosPorFechaAsync(DateTime fecha);
        Task<(bool, string)> RegistrarCierreAsync(CierreCajaViewModel model, int cedula);
        Task<decimal> ObtenerTotalFacturadoHoyAsync();
    }
}

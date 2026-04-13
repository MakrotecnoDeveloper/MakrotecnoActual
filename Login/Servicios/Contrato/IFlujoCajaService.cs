using Plataforma.Models;
using Plataforma.Models.Dto.Pedido;

namespace Plataforma.Servicios.Contrato
{
    public interface IFlujoCajaService
    {
        Task<IndexFlujoCajaVM> ObtenerResumenDeHoyAsync();
        Task<(bool, string)> RegistrarCierreAsync(CierreCajaViewModel model, ContextoAccesoDto ctx);
        Task<decimal> ObtenerTotalFacturadoHoyAsync();
        Task<decimal> ObtenerTotalFacturadoDelDiaAsync(
    ContextoAccesoDto ctx,
    DateTime? fecha = null);
        Task<List<ConceptoServicioVM>> ObtenerConceptosDelDiaAsync(
    ContextoAccesoDto ctx,
    DateTime fecha);
        Task<decimal> ObtenerTotalFacturadoAsync(ContextoAccesoDto ctx, DateTime fecha);
        Task<CierreCaja?> ObtenerCierreExistenteDelDiaAsync(int cedula, DateTime fecha);
    }
}

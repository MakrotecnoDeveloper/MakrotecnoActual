using Plataforma.Models;

namespace Plataforma.Servicios.Contrato
{
    public interface IFlujoCajaService
    {
        Task<IndexFlujoCajaVM> ObtenerResumenDeHoyAsync();
        Task<(bool, string)> RegistrarCierreAsync(CierreCajaViewModel model, int cedula);
        Task<decimal> ObtenerTotalFacturadoHoyAsync();
        Task<List<ConceptoServicioVM>> ObtenerConceptosDelDiaAsync();
    }
}

using Plataforma.Models;

namespace Plataforma.Services
{
    public interface INominaService
    {
        Task<List<Empleados>> ObtenerEmpleadosAsync();
        Task<List<Servicio>> ObtenerServiciosAsync();

        Task<List<Contrato>> ObtenerContratosAsync();
        Task CrearContratoAsync(ContratoFormVm vm);

        Task<List<PeriodoNomina>> ObtenerPeriodosAsync();
        Task CrearPeriodoAsync(PeriodoNominaFormVm vm);

        Task<List<ConceptoNomina>> ObtenerConceptosAsync();
        Task CrearConceptoAsync(ConceptoNominaFormVm vm);

        Task<List<DetalleConceptoEmpleado>> ObtenerAsignacionesAsync(int? cedula = null);
        Task AsignarConceptoAsync(AsignarConceptoEmpleadoVm vm);

        Task<GeneracionNovedadesResultado> GenerarNovedadesDesdeCierreCajaAsync(int idPeriodo, string usuario);

        Task<List<LiquidacionNomina>> ObtenerLiquidacionesAsync(int? idPeriodo = null, int? cedula = null);
        Task<List<LiquidacionNomina>> ObtenerUltimasLiquidacionesAsync(int take = 15);
        Task<LiquidacionNomina> GenerarLiquidacionAsync(int cedula, int idPeriodo, string usuario);
        Task<LiquidacionNomina?> ObtenerLiquidacionDetalleAsync(int idLiquidacion);
        Task<List<Area>> ObtenerAreasAsync();
        Task CrearAreaAsync(AreaFormVm vm);

        Task<List<TipoCargo>> ObtenerCargosAsync();
        Task AsignarAreaCargoAsync(AsignarAreaCargoVm vm);
        Task CambiarEstadoPeriodoAsync(int idPeriodo, string nuevoEstado);
    }
}
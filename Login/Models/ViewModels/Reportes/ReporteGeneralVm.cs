namespace Plataforma.ViewModels.Reportes
{
    public class ReporteGeneralVm
    {
        public FiltroReporteVm Filtro { get; set; } = new();

        public List<ReporteVentaServicioVm> VentasPorServicio { get; set; } = new();
        public List<ReporteGananciaPersonaVm> GananciasPorPersona { get; set; } = new();

        public decimal TotalVNeto => VentasPorServicio.Sum(x => x.TotalVNeto);
        public decimal TotalSubTotal => VentasPorServicio.Sum(x => x.TotalSubTotal);
        public decimal TotalUtilidad => VentasPorServicio.Sum(x => x.TotalUtilidad);
        public decimal TotalParticipaciones => GananciasPorPersona.Sum(x => x.ValorGanado);
    }
}
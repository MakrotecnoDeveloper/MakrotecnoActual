namespace Plataforma.ViewModels.Reportes
{
    public class ReporteVentaServicioVm
    {
        public int? IdServicio { get; set; }
        public string Servicio { get; set; } = string.Empty;
        public decimal TotalVNeto { get; set; }
        public decimal TotalSubTotal { get; set; }
        public decimal TotalUtilidad { get; set; }
        public int CantidadCierres { get; set; }
    }
}
namespace Plataforma.ViewModels.Reportes
{
    public class ReporteGananciaPersonaVm
    {
        public int Cedula { get; set; }
        public string Empleado { get; set; } = string.Empty;
        public int? IdServicio { get; set; }
        public string Servicio { get; set; } = string.Empty;
        public string Concepto { get; set; } = string.Empty;
        public decimal BaseUtilidad { get; set; }
        public decimal PorcentajePromedio { get; set; }
        public decimal ValorGanado { get; set; }
        public int CantidadNovedades { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace Plataforma.ViewModels.Reportes
{
    public class FiltroReporteVm
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; } = DateTime.Today;

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; } = DateTime.Today;
    }
}
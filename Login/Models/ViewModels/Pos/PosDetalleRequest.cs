using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models.ViewModels.Pos
{
    public class PosDetalleRequest
    {
        [Required]
        public string Codigo { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal Cantidad { get; set; }

        [Range(0, 100)]
        public decimal IvaPorcentaje { get; set; }
    }
}
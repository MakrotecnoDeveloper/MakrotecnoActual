using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models.ViewModels.Pos
{
    public class PosCrearVentaRequest
    {
        [Required]
        public int IdCliente { get; set; }

        public string? Conceptos { get; set; }

        public decimal MontoNequi { get; set; }
        public decimal MontoTarjeta { get; set; }
        public decimal MontoEfectivo { get; set; }
        public decimal MontoCredito { get; set; }

        public DateTime? FechaVencimientoCredito { get; set; }

        public List<PosDetalleRequest> Items { get; set; } = new();
    }
}
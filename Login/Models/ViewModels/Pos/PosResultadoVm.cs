namespace Plataforma.Models.ViewModels.Pos
{
    public class PosResultadoVm
    {
        public bool Ok { get; set; }
        public string Mensaje { get; set; } = string.Empty;

        public int? IdVenta { get; set; }
        public int? IdFactura { get; set; }
        public string? NumeroFactura { get; set; }
    }
}
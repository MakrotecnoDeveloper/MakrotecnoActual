namespace Plataforma.Models.ViewModels.Pos
{
    public class PosProductoVm
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Categoria { get; set; } = "Sin categoría";
        public decimal ValorVenta { get; set; }
        public decimal? ValorUnidad { get; set; }
        public decimal StockDisponible { get; set; }
        public string? ImagenPath { get; set; }
    }
}
namespace MakroTecno.ViewModels.Integraciones
{
    public class RappiProductoExportViewModel
    {
        public string Categoria { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string Sku { get; set; } = string.Empty;

        public string? Marca { get; set; }

        public string? Ean { get; set; }

        public string Descripcion { get; set; } = string.Empty;

        public string EsPesable { get; set; } = "NO";

        public string EsPreempaquetado { get; set; } = "SI";

        public decimal Cantidad { get; set; } = 1;

        public string UnidadMedida { get; set; } = "Und (unidades)";

        public decimal StockSede { get; set; }

        public decimal? Precio { get; set; }

        public string EstadoValidacion { get; set; } = "OK";

        public string? Observacion { get; set; }
    }
}
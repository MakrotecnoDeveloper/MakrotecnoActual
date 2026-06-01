namespace MakroTecno.ViewModels.Integraciones
{
    public class RappiActualizacionDetalleViewModel
    {
        public int FilaExcel { get; set; }

        public string? IdTiendaRappi { get; set; }

        public string? NombreTiendaRappi { get; set; }

        public string? IdProductoRappi { get; set; }

        public string? SkuRappi { get; set; }

        public string? SkuErpDetectado { get; set; }

        public string? NombreProductoRappi { get; set; }

        public string? ProductoErpId { get; set; }

        public string? NombreProductoErp { get; set; }

        public decimal? PrecioAnterior { get; set; }

        public decimal? PrecioNuevo { get; set; }

        public decimal? DescuentoNuevo { get; set; }

        public decimal? StockSede { get; set; }

        public string? DisponibilidadAnterior { get; set; }

        public string? DisponibilidadNueva { get; set; }

        public string Estado { get; set; } = "OK";

        public string? Observacion { get; set; }
    }
}
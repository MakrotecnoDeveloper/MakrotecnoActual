namespace Plataforma.Models
{
    public class ImportacionesProducto
    {
        public int IdImportacion { get; set; }

        public string IdEmpresa { get; set; } = string.Empty;

        public int? IdProveedor { get; set; }

        public int? IdCatePro { get; set; }

        public int? IdUnidad { get; set; }

        public string TipoOperacion { get; set; } = string.Empty;

        public string TipoEntrada { get; set; } = string.Empty;

        public string? TextoOriginal { get; set; }

        public string? NombreArchivo { get; set; }

        public string? CamposSeleccionados { get; set; }

        public int TotalRegistros { get; set; }

        public int TotalProcesados { get; set; }

        public int TotalErrores { get; set; }

        public string? Usuario { get; set; }

        public DateTime FechaImportacion { get; set; }

        public int Estado { get; set; }

        public ICollection<ImportacionesProductosDetalle> Detalles { get; set; } = new List<ImportacionesProductosDetalle>();
    }
}
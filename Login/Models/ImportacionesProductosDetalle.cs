namespace Plataforma.Models
{
    public class ImportacionesProductosDetalle
    {
        public int IdDetalle { get; set; }

        public int IdImportacion { get; set; }

        public string CodProducto { get; set; }

        public string NombreProducto { get; set; }

        public string LineaOriginal { get; set; }

        public string DatosProcesados { get; set; }

        public string EstadoProceso { get; set; } = string.Empty;

        public string Observacion { get; set; }

        public DateTime FechaRegistro { get; set; }

        public ImportacionesProducto Importacion { get; set; }
    }
}
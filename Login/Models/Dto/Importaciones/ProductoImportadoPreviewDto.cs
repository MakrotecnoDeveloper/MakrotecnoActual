// Models/ProductoImportadoPreviewDto.cs

namespace Plataforma.Models
{
    public class ProductoImportadoPreviewDto
    {
        public string CodProducto { get; set; }
        public string NombreProducto { get; set; }
        public decimal? CantidadProducto { get; set; }
        public decimal? ValorNetoProducto { get; set; }
        public decimal? ValorVentaProducto { get; set; }
        public decimal? ValorUnidad { get; set; }

        public string IdEmpresa { get; set; }
        public int Estado { get; set; }
        public string Ubicacion { get; set; }
        public int? IdCatePro { get; set; }
        public int? IdProveedor { get; set; }
        public string ImagenPath { get; set; }
        public string AutenticidadProducto { get; set; }
        public string CondicionProducto { get; set; }
        public int? EstadoWeb { get; set; }
        public int? IdUnidad { get; set; }
        public decimal? Iva { get; set; }

        public string EstadoProceso { get; set; } // Nuevo, Actualizar, NoExiste, Error
        public string Observacion { get; set; }
        public string LineaOriginal { get; set; }
    }
}
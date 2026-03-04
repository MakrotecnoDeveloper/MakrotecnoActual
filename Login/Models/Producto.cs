using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("productos", Schema = "dbo")]
public class Producto
{
        [Key]
        [Column("cod_producto")]
        public string? Cod_Producto { get; set; }
        [Column("nombreProducto")]
        public string? NombreProducto { get; set; }
        [Column("cantidadProducto")]
        public decimal CantidadProducto { get; set; }
        [Column("valorNetoProducto")]
        public decimal? ValorNetoProducto { get; set; }
        [Column("valorVentaProducto")]
        public decimal? ValorVentaProducto { get; set; }
        [Column("valorUnidad")]
        public int ValorUnidad {  get; set; }
        [Column("id_empresa")]
        public string? ID_Empresa { get; set; }
        [Column("estado")]
        public int Estado {  get; set; }
        [Column("Ubicacion")]
        public string? Ubicacion { get; set; }
        [Column("idCatePro")]
        public int IdCatepro {  get; set; }
        public int idProveedor { get; set; }
        [Column("imagenPath")]
        public string? ImagenPath { get; set; }
        public string? AutenticidadProducto { get; set; }
        public string? CondicionProducto { get; set; }
        public int EstadoWeb { get; set; }
    public ICollection<InventarioSede> Inventarios { get; set; } = new List<InventarioSede>();
}

public class StickerPrintVm
{
    public string CodProducto { get; set; } = "";
    public string NombreProducto { get; set; } = "";
    public decimal? Precio { get; set; }              // 👈 Precio en sticker
    public int Copias { get; set; } = 1;

    // QR (PNG en base64)
    public string QrBase64 { get; set; } = "";

    // Branding
    public string LogoUrl { get; set; } = "/img/Logo.png";  // 👈 tu logo real
    public string BrandHex { get; set; } = "#111827";                  // 👈 color institucional
    public int AnchoMm { get; set; } = 50;
    public int AltoMm { get; set; } = 25;                               // 👈 alto etiqueta (mm)
}

public class StickerItemVm
{
    public string Codigo { get; set; }
    public string Nombre { get; set; }
    public string Proveedor { get; set; }
    public decimal? Precio { get; set; }
}
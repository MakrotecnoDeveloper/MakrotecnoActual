using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("factura", Schema = "dbo")]
public class Factura
{
    [Key]
    public int IdFactura { get; set; }

    [Required]
    [StringLength(50)]
    public string NumeroFactura { get; set; } = string.Empty;

    [Required]
    public int IdVenta { get; set; }

    [Required]
    public DateTime FechaEmision { get; set; } = DateTime.Now;

    [Column(TypeName = "decimal(18,2)")]
    public decimal SubTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal IVA { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }

    [Required]
    [StringLength(30)]
    public string EstadoFactura { get; set; } = "Generada";

    // NUEVO: FacturaVenta / FacturaElectronica / DocumentoSoporte / NotaCredito / NotaDebito
    [Required]
    [StringLength(30)]
    public string TipoDocumento { get; set; } = "FacturaVenta";

    // NUEVO: POS / VentaNormal / ServicioTecnico / AgendaCitas / Produccion / Manual
    [StringLength(50)]
    public string OrigenModulo { get; set; } = "VentaNormal";

    // NUEVO: copia de la referencia para consulta rápida
    [StringLength(50)]
    public string? CodigoReferenciaOrigen { get; set; }

    // NUEVO: observación que puede imprimirse en factura
    [StringLength(500)]
    public string? Observacion { get; set; }

    public bool EsElectronica { get; set; } = false;

    [Required]
    [StringLength(30)]
    public string EstadoDian { get; set; } = "NoAplica";

    [StringLength(150)]
    public string? Cufe { get; set; }

    [StringLength(150)]
    public string? TrackId { get; set; }

    public DateTime? FechaEnvioDian { get; set; }

    public DateTime? FechaRespuestaDian { get; set; }

    public string? MensajeDian { get; set; }

    [ForeignKey(nameof(IdVenta))]
    public virtual Ventas Venta { get; set; } = default!;

    public virtual ICollection<AdicionFactura> Adiciones { get; set; } = new List<AdicionFactura>();
}

public class DetalleFacturaViewModel
{
    public Factura Factura { get; set; }
    public Clientes Cliente { get; set; } // puede ser null si no se encontró
    public Empresas Empresa { get; set; }
}
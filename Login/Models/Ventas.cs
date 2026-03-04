using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;

[Table("ventas", Schema = "dbo")]
public class Ventas
{
    [Key]
    public int IdVenta { get; set; }

    public int IdCliente { get; set; }

    // SOLO INFORMATIVO: Efectivo / Transferencia / Credito / Mixto
    public string MetodoPago { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }

    public string EstadoVenta { get; set; }

    public int Cedula { get; set; }

    public DateTime FechaVenta { get; set; }

    public int CedulaCliente { get; set; }

    public string Conceptos { get; set; }

    // === CAMPOS NUEVOS (DIAN / FUTURO) ===
    public string? NumeroFactura { get; set; }

    public DateTime? FechaEmisionFactura { get; set; }

    // Normal | Devolucion | NotaCredito
    public string TipoVenta { get; set; } = "Normal";

    // === NAVEGACIONES ===
    public virtual ICollection<Pedidos> Pedidos { get; set; }

    public virtual ICollection<CxcVentas> CuentasPorCobrar { get; set; }
}

public class RegistrarPagosViewModel
{
    public int IdVenta { get; set; }
    public int IdCliente { get; set; }

    public decimal TotalVenta { get; set; }

    [Range(0, double.MaxValue)]
    public decimal MontoEfectivo { get; set; }

    [Range(0, double.MaxValue)]
    public decimal MontoTransferencia { get; set; }

    [Range(0, double.MaxValue)]
    public decimal MontoCredito { get; set; }

    public DateTime? FechaVencimientoCredito { get; set; }
}

public class DetalleVentaViewModel
{
    public int IdVenta { get; set; }
    public int? IdFactura { get; set; }
    public string EstadoVenta { get; set; }
    public string NumeroFactura { get; set; }
    public Ventas Venta { get; set; }

    // ✅ NUEVO
    public Clientes Cliente { get; set; }

    // ✅ Crédito
    public decimal SaldoPendienteCredito { get; set; }
    public DateTime? FechaVencimientoCredito { get; set; }
    public string EstadoCxc { get; set; }
}



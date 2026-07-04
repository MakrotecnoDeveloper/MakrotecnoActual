using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;

[Table("ventas", Schema = "dbo")]
public class Ventas
{
    [Key]
    public int IdVenta { get; set; }

    public int IdCliente { get; set; }

    // Efectivo / Transferencia / Crédito / Mixto / Pendiente
    [StringLength(100)]
    public string MetodoPago { get; set; } = "Pendiente";

    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }

    [StringLength(30)]
    public string EstadoVenta { get; set; } = "Pendiente";

    public int Cedula { get; set; }

    public DateTime FechaVenta { get; set; }

    public int? CedulaCliente { get; set; }

    public string Conceptos { get; set; } = string.Empty;

    // Número de factura rápido para consultas
    public string? NumeroFactura { get; set; }

    public DateTime? FechaEmisionFactura { get; set; }

    // Normal / Devolucion / NotaCredito
    [StringLength(30)]
    public string TipoVenta { get; set; } = "Normal";

    // NUEVO: Producto / Servicio / Mixta
    [StringLength(30)]
    public string TipoOperacion { get; set; } = "Producto";

    // NUEVO: POS / VentaNormal / ServicioTecnico / AgendaCitas / Produccion / Manual
    [StringLength(50)]
    public string OrigenModulo { get; set; } = "VentaNormal";

    // NUEVO: id del registro origen, por ejemplo IdOrden, IdCita, IdProduccion
    public int? IdOrigenModulo { get; set; }

    // NUEVO: referencia visible, por ejemplo OS-00025, CITA-00040
    [StringLength(50)]
    public string? CodigoReferenciaOrigen { get; set; }

    // NUEVO: observación comercial general
    [StringLength(500)]
    public string? ObservacionVenta { get; set; }

    public virtual ICollection<Pedidos> Pedidos { get; set; } = new List<Pedidos>();

    public virtual ICollection<CxcVentas> CuentasPorCobrar { get; set; } = new List<CxcVentas>();
}

public class RegistrarPagosViewModel
{
    public int IdVenta { get; set; }
    public int IdCliente { get; set; }

    public decimal TotalVenta { get; set; }
    public string? OrigenModulo { get; set; }
    public string? TipoOperacion { get; set; }
    public string? CodigoReferenciaOrigen { get; set; }
    public string? ObservacionVenta { get; set; }

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

public class GeneracionNovedadesResultado
{
    public int VentasProcesadas { get; set; }
    public int NovedadesCreadas { get; set; }
    public int NovedadesActualizadas { get; set; }
    public int RegistrosSinAsignacion { get; set; }
    public decimal BaseVentasProcesada { get; set; }
    public decimal TotalComisionesGeneradas { get; set; }
}



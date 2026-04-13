namespace Plataforma.Models.ViewModels.Cxc
{
    public class CrearCxcDesdeVentaRequest
    {
        public int IdVenta { get; set; }
        public int IdCliente { get; set; }
        public decimal TotalCredito { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string? Observacion { get; set; }
    }

    public class RegistrarPagoCxcViewModel
    {
        public int IdCxc { get; set; }
        public int IdMetodo { get; set; }
        public decimal MontoPago { get; set; }
        public DateTime FechaPago { get; set; } = DateTime.Now;
        public string? Observacion { get; set; }
    }

    public class CxcPagoItemVm
    {
        public int IdPago { get; set; }
        public DateTime FechaPago { get; set; }
        public string MetodoPago { get; set; } = "";
        public decimal MontoPago { get; set; }
        public string? Observacion { get; set; }
    }

    public class CxcDetalleViewModel
    {
        public int IdCxc { get; set; }
        public int IdVenta { get; set; }
        public int IdCliente { get; set; }
        public string Cliente { get; set; } = "";
        public int? CedulaCliente { get; set; }

        public decimal Total { get; set; }
        public decimal TotalPagado { get; set; }
        public decimal SaldoPendiente { get; set; }

        public string EstadoCxc { get; set; } = "";
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string? Observacion { get; set; }

        public List<CxcPagoItemVm> Pagos { get; set; } = new();
    }

    public class CxcListadoItemVm
    {
        public int IdCxc { get; set; }
        public int IdVenta { get; set; }
        public string Cliente { get; set; } = "";
        public int? CedulaCliente { get; set; }
        public decimal Total { get; set; }
        public decimal SaldoPendiente { get; set; }
        public string EstadoCxc { get; set; } = "";
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaVencimiento { get; set; }
    }

    public class CxcIndexViewModel
    {
        public string? Texto { get; set; }
        public string? Estado { get; set; }
        public List<CxcListadoItemVm> Items { get; set; } = new();
    }
}
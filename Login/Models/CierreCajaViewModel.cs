using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;
public class CierreCajaViewModel
{
    [Required]
    public decimal Efectivo { get; set; }

    [Required]
    public decimal Transferencia { get; set; }

    public decimal GastoEfectivo { get; set; }

    public decimal GastoTransferencia { get; set; }

    public decimal TotalFacturado { get; set; }  // solo lectura

    public DateTime Fecha { get; set; } = DateTime.Today;
}
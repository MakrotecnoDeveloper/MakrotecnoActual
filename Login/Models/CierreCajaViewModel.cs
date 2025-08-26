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
    public List<ConceptoServicioVM> Conceptos { get; set; } = new();
}

public class ConceptoServicioVM
{
    public int IdServicio { get; set; }
    public string NombreServicio { get; set; } = string.Empty;

    // Lo que pediste:
    public decimal TotalSubTotal { get; set; } // primer input
    public decimal TotalVNeto { get; set; }    // segundo input
}

public class CierreConConceptosVM
{
    public int IdFlujoCaja { get; set; }
    public DateTime Fecha { get; set; }
    public int Cedula { get; set; }
    public string Concepto { get; set; } = "";
    public decimal Monto { get; set; }

    public List<ConceptoServicioVM> Conceptos { get; set; } = new();
    public decimal TotalSubTotal => Conceptos.Sum(x => x.TotalSubTotal);
    public decimal TotalVNeto => Conceptos.Sum(x => x.TotalVNeto);
}

public class IndexFlujoCajaVM
{
    public DateTime Fecha { get; set; }
    public List<Plataforma.Models.FlujoCaja> Movimientos { get; set; } = new();

    public decimal TotalIngresos { get; set; }
    public decimal TotalEgresos { get; set; }
    public decimal Balance => TotalIngresos - TotalEgresos;

    // Cierres con desglose (ConceptosJson)
    public List<CierreConConceptosVM> Cierres { get; set; } = new();
}
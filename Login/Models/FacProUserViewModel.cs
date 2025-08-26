namespace Plataforma.Models;
public class FacProUserViewModel
{
    public int TotalSumaCodFactura { get; set; } // Nueva propiedad
        public decimal TotalVentaDia { get; set; }
        public int TotalEmpleados { get; set; }
        public int TotalProductos { get; set; }
        public string? RolEmpleado { get; set; }
        public int IdPDV { get; set; }
        public string? NombrePDV { get; set; }
}
public sealed class SeriePuntoDTO
{
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
}

public sealed class CategoriaValorDTO
{
    public string Etiqueta { get; set; } = "";
    public decimal Valor { get; set; }
}

public sealed class OrdenServicioDTO
{
    public int Id { get; set; }
    public string Cliente { get; set; } = "";   // adapta si tu campo se llama distinto
    public int Tecnico { get; set; } = 0;
    public string Estado { get; set; } = "";
    public DateTime Fecha { get; set; }
}

public sealed class StockBajoDTO
{
    public string Codigo { get; set; } = "";
    public string Nombre { get; set; } = "";
    public decimal Stock { get; set; }
    public int Minimo { get; set; }
}

public sealed class CompraDTO
{
    public int IdCompra { get; set; }
    public string Proveedor { get; set; } = "";
    public DateTime? Fecha { get; set; }
    public decimal Total { get; set; }
}
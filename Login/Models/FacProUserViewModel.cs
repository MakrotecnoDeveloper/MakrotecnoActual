using Plataforma.Models;

namespace Login.Models
{
    public class FacProuserViewModel
    {
        public int TotalSumaCodFactura { get; set; } // Nueva propiedad
        public decimal TotalVentaDia { get; set; }
        public int TotalEmpleados { get; set; }
        public int TotalProductos { get; set; }
        public string RolEmpleado { get; set; }
        public int IdPDV { get; set; }
        public string? NombrePDV { get; set; }
    }
}
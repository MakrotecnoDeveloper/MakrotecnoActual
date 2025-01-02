using System.ComponentModel.DataAnnotations;
namespace Plataforma.Models;
public class Plataformasuscripcion
{
    [Key]
        public int IdPltfSuscripcion {  get; set; }
        public int IdPlataforma { get; set; }
        public string? Descripcion { get; set; }
        public int ValorVenta {  get; set; }
        public int ValorNeto { get; set; }
        public DateTime FechaIniPago { get; set; }
        public DateTime FechaFinPago { get; set; }
        public int Cantidad { get; set; }
        public string? Correo { get; set; }
        public string? Contrasena { get; set; }
        public int CedulaEmpleado { get; set; }
        public int Estado { get; set; }
}

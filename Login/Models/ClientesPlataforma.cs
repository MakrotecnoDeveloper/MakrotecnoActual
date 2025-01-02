using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;
    public class ClientesPlataforma
    {
        [Key]
        public int IdCliPltf {  get; set; }
        public string? NombreCliente { get; set; }
        public string? CelularCliente { get; set; }
        public string? Correo {  get; set; }
        public string? Clave { get; set; }
        public int IdPltfSuscripcion { get; set; }
        public int Cantidad { get; set; }
        public string? Ppm { get; set; }
        public DateTime FechaIniPago { get; set; }
        public DateTime FechaFinPago { get; set; }
        public int ValorVenta { get; set; }
        public int ValorNeto { get; set; }
        public int CedulaEmpleado { get; set; }
        public int Estado {  get; set; }
        public string? ClavePerfil { get; set; }
    }

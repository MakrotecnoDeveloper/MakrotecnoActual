using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;
    public class Plataformas
    {
        [Key]
        public int idPlataforma {  get; set; }
        public string plataforma { get; set; }
        public string descripcion { get; set; }
        public int valorVenta {  get; set; }
        public int valorNeto { get; set; }
        public DateTime fechaIniPago { get; set; }
        public DateTime fechaFinPago { get; set; }
        public int cantidad { get; set; }
        public string correo { get; set; }
        public string contrasena { get; set; }
        public int cedulaEmpleado { get; set; }
        public int estado { get; set; }
    }

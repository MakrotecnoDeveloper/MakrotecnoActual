using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;
    public class ClientesPlataforma
    {
        [Key]
        public int idCliPltf {  get; set; }
        public string nombreCliente { get; set; }
        public string celularCliente { get; set; }
        public string correo {  get; set; }
        public string clave { get; set; }
        public int idPlataforma { get; set; }
        public int cantidad { get; set; }
        public string ppm { get; set; }
        public DateTime fechaIniPago { get; set; }
        public DateTime fechaFinPago { get; set; }
        public int valorVenta { get; set; }
        public int valorNeto { get; set; }
        public int cedulaEmpleado { get; set; }
        public int estado {  get; set; }
        public string clavePerfil { get; set; }
    }

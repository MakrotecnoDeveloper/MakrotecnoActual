using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;
    public class ClientePlataformaDTO
    {
        public int idCliente { get; set; }
        public int IdClientePlataforma { get; set; }
        public string NombreCliente { get; set; }
        public string CorreoPlataforma { get; set; }
        public string ClavePlataforma { get; set; }
        public string ClavePerfil { get; set; }
        public string Celular {  get; set; }
        public DateTime FechaIni {  get; set; }
        public DateTime FechaFin { get; set; }
        public int Plataforma { get; set; }
        public string NombrePlataforma { get; set; }
        public int Estado { get; set; }
    }

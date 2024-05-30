using Plataforma.Models;
using System.ComponentModel.DataAnnotations;

namespace Login.Models
{
    public class Sedeempleado
    {
        [Key]
        public int id_sedeEmpleado {  get; set; }
        public int id_sede {  get; set; }
        public int cedula { get; set; }
        public int id_cargo { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
namespace Plataforma.Models;
public class Sedeempleado
{
        [Key]
        public int Id_sedeEmpleado {  get; set; }
        public int Id_sede {  get; set; }
        public int Cedula { get; set; }
        public int Id_cargo { get; set; }
}
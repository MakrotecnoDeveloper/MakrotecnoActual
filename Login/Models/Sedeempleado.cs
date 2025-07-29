using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("sedeempleado", Schema = "dbo")]
public class Sedeempleado
{
        [Key]
        public int Id_sedeEmpleado {  get; set; }
        public int Id_sede {  get; set; }
        public int Cedula { get; set; }
        public int Id_cargo { get; set; }
}
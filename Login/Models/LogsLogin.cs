using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("logslogin", Schema = "pruebas")]
public class LogsLogin
{
    [Key]
    public int Id_log { get; set; }
    public int Cedula { get; set; }
    public string? Correo { get; set; }
    public DateTime Fecha { get; set; }
    public int Estado { get; set; } // 1 para activo, 0 para inactivo
    public int InfopdvId {  get; set; }
}

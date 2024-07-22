using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;
public partial class VentPlatClient
{
    [Key]
    public int idVenta { get; set; }
    public int idCliPltf { get; set; }
    public string perfil {  get; set; }
    public string clave { get; set; }
}
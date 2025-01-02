using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;
public class Cliente
{
    [Key]
    public int CedulaCliente { get; set; }
    public string? NombreCliente { get; set; }
    public string? EmpresaCliente { get; set; }
    public string? CiudadCliente { get; set; }
    public string? TelefonoCliente { get; set; }
}

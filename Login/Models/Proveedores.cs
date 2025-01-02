using System.ComponentModel.DataAnnotations;
namespace Plataforma.Models;
public class Proveedores
{
    [Key]
    public int IdProveedor {  get; set; }
    public string? Nit {  get; set; }
    public string? RazonSocial { get; set; }
    public string? Direccion { get; set; }
    public string? Celular { get; set; }
    public string? Correo { get; set; }
}
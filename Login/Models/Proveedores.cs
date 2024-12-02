using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;

public partial class Proveedores
{
    [Key]
    public int idProveedor {  get; set; }
    public string nit {  get; set; }
    public string razonSocial { get; set; }
    public string direccion { get; set; }
    public string celular { get; set; }
    public string correo { get; set; }
}
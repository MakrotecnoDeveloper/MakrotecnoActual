using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;
public class CategoriaProductosViewModel
{
    [Required(ErrorMessage = "La descripción es obligatoria")]
    public string Descripcion { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un servicio")]
    public int IdServicio { get; set; }

    // Para el dropdown
    public List<Servicio> Servicios { get; set; } = new();
}
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models
{
    [Table("Areas", Schema = "dbo")]
    public class Area
    {
        [Key]
        public int IdArea { get; set; }

        public string NombreArea { get; set; } = null!;
        public string? DescripcionArea { get; set; }
        public bool Estado { get; set; } = true;

        public virtual ICollection<TipoCargo> TiposCargo { get; set; } = new List<TipoCargo>();
    }

    public class AreaFormVm
    {
        [Required]
        public string NombreArea { get; set; } = null!;

        public string? DescripcionArea { get; set; }

        public bool Estado { get; set; } = true;
    }

    public class AsignarAreaCargoVm
    {
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un cargo.")]
        public int IdTipoCargo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un área.")]
        public int IdArea { get; set; }

        public List<SelectListItem> Cargos { get; set; } = new();
        public List<SelectListItem> Areas { get; set; } = new();
    }
}
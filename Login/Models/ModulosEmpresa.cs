using Plataforma.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MakroTecno.Models
{
    public class ModulosEmpresa
    {
        [Key]
        public int IdModuloEmpresa { get; set; }

        [Required]
        [StringLength(50)]
        public string IdEmpresa { get; set; } = string.Empty;

        [Required]
        public int IdModulo { get; set; }

        public bool Activo { get; set; } = true;

        public bool EsAdicional { get; set; } = false;

        public DateTime FechaAsignacion { get; set; } = DateTime.Now;

        [ForeignKey(nameof(IdEmpresa))]
        public Empresas? Empresa { get; set; }

        [ForeignKey(nameof(IdModulo))]
        public Modulos? Modulo { get; set; }
    }
}
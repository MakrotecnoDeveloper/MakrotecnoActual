using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;
    public class Menu
    {
        [Key]
        public int IdMenu { get; set; }
        public string DescripcionMenu { get; set; }
    }

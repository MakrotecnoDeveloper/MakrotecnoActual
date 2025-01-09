using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;
    public class MenuOption
    {
        [Key]
        public int IdMenuOption { get; set; }
        public string Icon { get; set; }
        public int Id_Tipo { get; set; }
        public string Id_Empresa {  get; set; }
        public int Estado { get; set; }
        public int IdMenu { get; set; }
    }

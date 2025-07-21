using Microsoft.AspNetCore.Mvc.Rendering;

namespace Plataforma.Models
{
    public class ConfiguracionMenuViewModel
    {
        public int IdCargoSeleccionado { get; set; }
        public List<SelectListItem> Cargos { get; set; } = new();
        public List<MenuItemViewModel> Menus { get; set; } = new();
    }

    public class MenuItemViewModel
    {
        public int IdMenu { get; set; }
        public string NombreMenu { get; set; }
        public bool Habilitado { get; set; }
    }
}

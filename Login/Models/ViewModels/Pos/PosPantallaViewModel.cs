using Microsoft.AspNetCore.Mvc.Rendering;

namespace Plataforma.Models.ViewModels.Pos
{
    public class PosPantallaViewModel
    {
        public int IdSede { get; set; }
        public int InfoPdvId { get; set; }
        public string NombreSede { get; set; } = string.Empty;

        public List<SelectListItem> Clientes { get; set; } = new();
        public List<string> Categorias { get; set; } = new();
    }
}
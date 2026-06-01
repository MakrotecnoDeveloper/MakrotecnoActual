using Microsoft.AspNetCore.Mvc.Rendering;

namespace MakroTecno.ViewModels.Integraciones
{
    public class RappiTiendaViewModel
    {
        public int Id { get; set; }

        public string IdTiendaRappi { get; set; } = string.Empty;

        public string NombreTiendaRappi { get; set; } = string.Empty;
        public string? NombreSede { get; set; }

        public int SedeId { get; set; }

        public bool Activa { get; set; } = true;

        public List<SelectListItem> Sedes { get; set; } = new();
    }
}
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MakroTecno.ViewModels.Integraciones
{
    public class RappiCategoriaMapeoViewModel
    {
        public int Id { get; set; }

        public int IdCateProducto { get; set; }

        public string CategoriaRappi { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;
        public string? CategoriaErpNombre { get; set; }

        public List<SelectListItem> CategoriasErp { get; set; } = new();
    }
}
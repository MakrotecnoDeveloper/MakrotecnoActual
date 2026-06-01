using Microsoft.AspNetCore.Mvc.Rendering;

namespace MakroTecno.ViewModels.Integraciones
{
    public class RappiProductoConfigViewModel
    {
        public int Id { get; set; }

        public string ProductoId { get; set; } = string.Empty;

        public string? NombreProducto { get; set; }

        public string? SkuRappi { get; set; }

        public string? NombreRappi { get; set; }

        public string? DescripcionRappi { get; set; }

        public string? MarcaRappi { get; set; }

        public string? Ean { get; set; }

        public bool EsPesable { get; set; }

        public bool EsPreempaquetado { get; set; } = true;

        public decimal CantidadPresentacion { get; set; } = 1;

        public string UnidadMedidaRappi { get; set; } = "Und (unidades)";

        public bool PublicarEnRappi { get; set; } = true;

        public bool Activo { get; set; } = true;

        public List<SelectListItem> Productos { get; set; } = new();
    }
}
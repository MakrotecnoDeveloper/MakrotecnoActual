using Microsoft.AspNetCore.Mvc.Rendering;

namespace MakroTecno.ViewModels.Integraciones
{
    public class RappiActualizacionResultadoViewModel
    {
        public bool Exitoso { get; set; }

        public string? Mensaje { get; set; }

        public int? RappiTiendaId { get; set; }

        public int TotalFilas { get; set; }

        public int TotalActualizados { get; set; }

        public int TotalNoEncontrados { get; set; }

        public int TotalSinPrecio { get; set; }

        public int TotalSinInventario { get; set; }

        public string? ArchivoGenerado { get; set; }

        public List<SelectListItem> TiendasRappi { get; set; } = new();

        public List<RappiActualizacionDetalleViewModel> Detalles { get; set; } = new();
    }
}
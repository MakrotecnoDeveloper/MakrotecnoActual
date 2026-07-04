// Models/ImportacionProductosViewModel.cs

using Microsoft.AspNetCore.Http;

namespace Plataforma.Models
{
    public class ImportacionProductosViewModel
    {
        public string TipoOperacion { get; set; } // Crear / Actualizar
        public string TipoEntrada { get; set; } // Texto / Excel

        public string TextoProductos { get; set; }

        public IFormFile ArchivoExcel { get; set; }

        public int? IdProveedor { get; set; }
        public int? IdCatePro { get; set; }
        public int? IdUnidad { get; set; }

        public string IdEmpresa { get; set; }

        public decimal? MargenPorcentaje { get; set; }
        public decimal? Iva { get; set; }

        public List<string> CamposSeleccionados { get; set; } = new();
        public string CamposOrdenadosJson { get; set; }
    }
}
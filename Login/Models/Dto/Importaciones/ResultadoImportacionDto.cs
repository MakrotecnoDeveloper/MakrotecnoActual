// Models/ResultadoImportacionDto.cs

namespace Plataforma.Models
{
    public class ResultadoImportacionDto
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; }
        public int? IdImportacion { get; set; }
        public string TipoOperacion { get; set; }
        public string CamposOrdenadosJson { get; set; }
        public int TotalLeidos { get; set; }
        public int TotalProcesados { get; set; }
        public int TotalErrores { get; set; }

        public List<ProductoImportadoPreviewDto> Productos { get; set; } = new();
        public List<string> Errores { get; set; } = new();
    }
}
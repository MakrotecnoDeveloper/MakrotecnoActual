namespace MakroTecno.ViewModels.Integraciones
{
    public class RappiExportResultadoViewModel
    {
        public bool Exitoso { get; set; }

        public string? Mensaje { get; set; }

        public int TotalProductos { get; set; }

        public int TotalValidos { get; set; }

        public int TotalConErrores { get; set; }

        public List<string> ArchivosGenerados { get; set; } = new();

        public List<RappiProductoExportViewModel> Productos { get; set; } = new();
    }
}
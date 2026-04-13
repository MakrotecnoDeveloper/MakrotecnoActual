namespace Plataforma.Models.ViewModels.Pos
{
    public class PosProductosPaginadosVm
    {
        public List<PosProductoVm> Items { get; set; } = new();
        public int PaginaActual { get; set; }
        public int TamanoPagina { get; set; }
        public int TotalRegistros { get; set; }
        public int TotalPaginas { get; set; }
    }
}
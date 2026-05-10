using Microsoft.AspNetCore.Mvc.Rendering;

namespace Plataforma.Models.ViewModels.Reportes
{
    public class ReporteVentasViewModel
    {
        public ReporteVentasFiltroViewModel Filtros { get; set; } = new();

        public List<ReporteVentasItemViewModel> Resultados { get; set; } = new();

        public List<SelectListItem> Vendedores { get; set; } = new();
        public List<SelectListItem> Sedes { get; set; } = new();
        public List<SelectListItem> Pdvs { get; set; } = new();
        public List<SelectListItem> EstadosVenta { get; set; } = new();
        public List<SelectListItem> EstadosFactura { get; set; } = new();
        public List<SelectListItem> MetodosPago { get; set; } = new();

        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
        public int RegistrosPorPagina { get; set; } = 20;

        public decimal TotalPagina { get; set; }
        public decimal TotalGeneralFiltrado { get; set; }
    }

    public class ReporteVentasFiltroViewModel
    {
        public bool FiltrarFecha { get; set; }
        public DateTime? FechaInicial { get; set; }
        public DateTime? FechaFinal { get; set; }

        public bool FiltrarVendedor { get; set; }
        public int? CedulaVendedor { get; set; }

        public bool FiltrarSede { get; set; }
        public int? IdSede { get; set; }

        public bool FiltrarPdv { get; set; }
        public int? InfopdvId { get; set; }

        public bool FiltrarMetodoPago { get; set; }
        public string? MetodoPago { get; set; }

        public bool FiltrarEstadoVenta { get; set; }
        public string? EstadoVenta { get; set; }

        public bool FiltrarEstadoFactura { get; set; }
        public string? EstadoFactura { get; set; }

        public bool FiltrarCliente { get; set; }
        public string? Cliente { get; set; }

        public bool FiltrarFactura { get; set; }
        public string? NumeroFactura { get; set; }
    }

    public class ReporteVentasItemViewModel
    {
        public int IdVenta { get; set; }
        public int? IdFactura { get; set; }

        public string? NumeroFactura { get; set; }

        public DateTime FechaVenta { get; set; }
        public DateTime? FechaEmisionFactura { get; set; }

        public int IdCliente { get; set; }
        public int CedulaCliente { get; set; }

        public int CedulaVendedor { get; set; }
        public string? NombreVendedor { get; set; }

        public string? MetodoPago { get; set; }

        public string? NombreSede { get; set; }
        public string? NombrePdv { get; set; }

        public decimal TotalVenta { get; set; }
        public decimal? TotalFactura { get; set; }

        public string? EstadoVenta { get; set; }
        public string? EstadoFactura { get; set; }

        public string? TipoVenta { get; set; }
        public string? Conceptos { get; set; }
    }
}
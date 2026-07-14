using Microsoft.AspNetCore.Mvc.Rendering;

namespace Plataforma.Models.ViewModels.Deporte;

public class EstilosDeportivosAdminVm
{
    public int? IdDeporteSeleccionado { get; set; }

    public string? BuscarDeporte { get; set; }
    public string? BuscarEstilo { get; set; }

    public int PaginaDeportes { get; set; } = 1;
    public int PaginaEstilos { get; set; } = 1;

    public int TotalPaginasDeportes { get; set; }
    public int TotalPaginasEstilos { get; set; }

    public List<DeporteAdminItemVm> Deportes { get; set; } = new();
    public List<EstiloDeportivoItemVm> Estilos { get; set; } = new();

    public EstiloDeportivoFormVm Formulario { get; set; } = new();
}

public class DeporteAdminItemVm
{
    public int IdDeporte { get; set; }
    public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public int CantidadEstilos { get; set; }
}

public class EstiloDeportivoItemVm
{
    public int IdPruebaDeportiva { get; set; }
    public int IdDeporte { get; set; }
    public string Deporte { get; set; } = "";
    public string Nombre { get; set; } = "";
    public decimal? Distancia { get; set; }
    public string? UnidadMedida { get; set; }
    public string? Modalidad { get; set; }
    public bool Activo { get; set; }
}

public class EstiloDeportivoFormVm
{
    public int IdPruebaDeportiva { get; set; }
    public int IdDeporte { get; set; }

    public string Nombre { get; set; } = "";
    public decimal? Distancia { get; set; }
    public string? UnidadMedida { get; set; }
    public string? Modalidad { get; set; }

    public bool Activo { get; set; } = true;
}
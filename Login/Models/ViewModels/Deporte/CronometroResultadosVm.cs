using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Plataforma.Models.ViewModels.Deporte;

public class CronometroResultadosVm
{
    public string? Busqueda { get; set; }
    public string? Cedula { get; set; }
    public bool BusquedaRealizada { get; set; }
    public bool ClienteEncontrado { get; set; }
    public string? Mensaje { get; set; }

    public int? IdCliente { get; set; }
    public string? NombreCliente { get; set; }
    public int? IdDeportista { get; set; }

    public int? IdDeporteSeleccionado { get; set; }
    public int? IdPruebaDeportivaSeleccionada { get; set; }

    public RegistrarTiempoDeportivoVm NuevoTiempo { get; set; } = new();
    public List<ResultadoDeportivoItemVm> Resultados { get; set; } = new();
    public List<SelectListItem> Deportes { get; set; } = new();
    public List<SelectListItem> PruebasDeportivas { get; set; } = new();

    public TimeSpan? MejorTiempo { get; set; }
    public TimeSpan? UltimoTiempo { get; set; }
    public TimeSpan? PromedioTiempo { get; set; }
}

public class RegistrarTiempoDeportivoVm
{
    public string? Cedula { get; set; }
    public int IdPruebaDeportiva { get; set; }
    public string TiempoTexto { get; set; } = null!;
    public DateTime FechaPrueba { get; set; } = DateTime.Now;
    public string? Observaciones { get; set; }
}

public class RegistrarTiemposGrupoVm
{
    public int IdPruebaDeportiva { get; set; }
    public DateTime FechaSesion { get; set; } = DateTime.Now;
    public string? ObservacionesSesion { get; set; }
    public List<RegistrarTiempoGrupoItemVm> Tiempos { get; set; } = new();
}

public class RegistrarTiempoGrupoItemVm
{
    public int IdCliente { get; set; }
    public string TiempoTexto { get; set; } = null!;
    public int? Posicion { get; set; }
    public string? Observaciones { get; set; }
}

public class ResultadoDeportivoItemVm
{
    public int IdResultadoDeportivo { get; set; }
    public DateTime FechaPrueba { get; set; }
    public string Deporte { get; set; } = null!;
    public string Prueba { get; set; } = null!;
    public TimeSpan Tiempo { get; set; }
    public string TiempoFormato => Tiempo.ToString(@"mm\:ss\.ff");
    public int? Posicion { get; set; }
    public string? Observaciones { get; set; }
}

public class ClienteBusquedaDeportivaVm
{
    public int IdCliente { get; set; }
    public string CedulaCliente { get; set; } = "";
    public string NombreCompleto { get; set; } = "";
}


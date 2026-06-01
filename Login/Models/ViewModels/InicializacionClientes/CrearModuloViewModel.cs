public class CrearModuloViewModel
{
    public string NombreModulo { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public bool EsGenerico { get; set; }

    public bool EsEspecializado { get; set; }

    public bool AgregarAlPlan { get; set; }

    public int? IdPlan { get; set; }

    public bool AgregarAEmpresaActual { get; set; }

    public string? IdEmpresa { get; set; }

    public bool AgregarAActividadEconomica { get; set; }

    public int? IdActividad { get; set; }

    // ==============================
    // CONFIGURACIÓN DEL MENÚ
    // ==============================

    public string Header { get; set; } = string.Empty;

    public string Grupo { get; set; } = string.Empty;

    public string IconoHeader { get; set; } = string.Empty;

    public string IconoGrupo { get; set; } = string.Empty;

    public string IconoOpcion { get; set; } = string.Empty;

    public string Controller { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public int OrdenHeader { get; set; }

    public int OrdenGrupo { get; set; }

    public int OrdenOpcion { get; set; }
}
namespace Plataforma.Models.ViewModels.Inicio;

public class InicioPortalVm
{
    public string EmpresaId { get; set; } = "";
    public string NombreEmpresa { get; set; } = "";

    public int SedeId { get; set; }
    public string NombreSede { get; set; } = "";

    public int PdvId { get; set; }
    public string NombrePdv { get; set; } = "";

    public int Cedula { get; set; }
    public string NombreUsuario { get; set; } = "";
    public string NombreRol { get; set; } = "";

    public int TotalSedesEmpresa { get; set; }
    public int TotalPdvSede { get; set; }
    public int TotalEmpleadosSede { get; set; }

    public string EstadoConexion { get; set; } = "Activo";
}
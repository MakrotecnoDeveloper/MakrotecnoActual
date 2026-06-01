namespace MakroTecno.Models.ViewModels.InicializacionClientes
{
    public class InicializacionClienteViewModel
    {
        public string? NitBuscado { get; set; }

        public bool BusquedaRealizada { get; set; }

        public bool ClienteExiste { get; set; }

        public EmpresaResumenViewModel? Empresa { get; set; }

        public LicenciaResumenViewModel? Licencia { get; set; }

        public List<SedeResumenViewModel> Sedes { get; set; } = new();

        public List<PdvResumenViewModel> Pdvs { get; set; } = new();

        public List<EmpleadoResumenViewModel> Empleados { get; set; } = new();

        public List<ModuloResumenViewModel> ModulosDisponibles { get; set; } = new();

        public List<ModuloResumenViewModel> ModulosActivos { get; set; } = new();

        public List<PlanResumenViewModel> Planes { get; set; } = new();

        public List<ActividadEconomicaResumenViewModel> ActividadesEconomicas { get; set; } = new();

        public CrearClienteInicialViewModel NuevoCliente { get; set; } = new();
        public CrearModuloViewModel NuevoModulo { get; set; } = new();
    }

    public class EmpresaResumenViewModel
    {
        public string IdEmpresa { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Pais { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? Estado { get; set; }
        public string? ActividadEconomica { get; set; }
    }

    public class LicenciaResumenViewModel
    {
        public int IdLicencia { get; set; }
        public string NombrePlan { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; } = string.Empty;
    }

    public class SedeResumenViewModel
    {
        public int IdSede { get; set; }
        public string NombreSede { get; set; } = string.Empty;
        public string? Ciudad { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
    }

    public class PdvResumenViewModel
    {
        public int IdInfoPdv { get; set; }
        public string NombreInfoPdv { get; set; } = string.Empty;
        public int IdSede { get; set; }
        public string? NombreSede { get; set; }
    }

    public class EmpleadoResumenViewModel
    {
        public int Cedula { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string? Correo { get; set; }
        public string? Celular { get; set; }
        public string? Cargo { get; set; }
        public string? Sede { get; set; }
    }

    public class ModuloResumenViewModel
    {
        public int IdModulo { get; set; }
        public string NombreModulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool EsGenerico { get; set; }
        public bool EsEspecializado { get; set; }
        public bool ActivoParaCliente { get; set; }
    }

    public class PlanResumenViewModel
    {
        public int IdPlan { get; set; }
        public string NombrePlan { get; set; } = string.Empty;
        public decimal PrecioMensual { get; set; }
        public int LimiteEmpresas { get; set; }
        public int LimiteSedes { get; set; }
        public int LimitePDV { get; set; }
        public int LimiteUsuarios { get; set; }
        public int LimiteProductos { get; set; }
        public int LimiteClientes { get; set; }
    }

    public class ActividadEconomicaResumenViewModel
    {
        public int IdActividad { get; set; }
        public string NombreActividad { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }

    public class CrearClienteInicialViewModel
    {
        public string IdEmpresa { get; set; } = string.Empty;
        public string NombreEmpresa { get; set; } = string.Empty;
        public string Pais { get; set; } = "Colombia";
        public string? DireccionEmpresa { get; set; }
        public string? TelefonoEmpresa { get; set; }
        public int ActividadEconomicaId { get; set; }
        public int IdPlan { get; set; }

        public string NombreSede { get; set; } = "Principal";
        public string? CiudadSede { get; set; }
        public string? DireccionSede { get; set; }
        public string? TelefonoSede { get; set; }

        public string NombrePdv { get; set; } = "Caja";

        public int CedulaAdministrador { get; set; }
        public string NombreAdministrador { get; set; } = string.Empty;
        public string ApellidoAdministrador { get; set; } = string.Empty;
        public string? GeneroAdministrador { get; set; }
        public string? CorreoAdministrador { get; set; }
        public string? RhAdministrador { get; set; }
        public string? CelularAdministrador { get; set; }
        public string ContrasenaAdministrador { get; set; } = "123";

        public DateTime FechaInicioLicencia { get; set; } = DateTime.Today;
        public DateTime FechaFinLicencia { get; set; } = DateTime.Today.AddYears(1);
    }
}
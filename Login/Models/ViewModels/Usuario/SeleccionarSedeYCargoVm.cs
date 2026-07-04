namespace Plataforma.Models.ViewModels.Usuario;

public class SeleccionarSedeYCargoVm
{
    public int Cedula { get; set; }
    public List<SedeEmpresaVm> Sedes { get; set; } = new();
    public List<CargoEmpresaVm> Cargos { get; set; } = new();
}

public class SedeEmpresaVm
{
    public int IdSede { get; set; }
    public string NombreSede { get; set; } = "";
    public string IdEmpresa { get; set; } = "";
    public string NombreEmpresa { get; set; } = "";
}

public class CargoEmpresaVm
{
    public int IdCargo { get; set; }
    public string NombreCargo { get; set; } = "";
    public string DescripcionCargo { get; set; } = "";
    public string IdEmpresa { get; set; } = "";
}
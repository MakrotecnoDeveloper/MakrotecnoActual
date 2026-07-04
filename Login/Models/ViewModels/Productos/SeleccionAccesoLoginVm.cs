namespace Plataforma.Models.ViewModels.Productos;

public class SeleccionAccesoLoginVm
{
    public int Cedula { get; set; }
    public string Password { get; set; } = "";

    public List<EmpresaLoginVm> Empresas { get; set; } = new();
    public List<SedeLoginVm> Sedes { get; set; } = new();
    public List<PdvLoginVm> Pdvs { get; set; } = new();
}

public class EmpresaLoginVm
{
    public string EmpresaId { get; set; } = "";
    public string NombreEmpresa { get; set; } = "";
}

public class SedeLoginVm
{
    public int SedeId { get; set; }
    public string NombreSede { get; set; } = "";
    public string EmpresaId { get; set; } = "";
}

public class PdvLoginVm
{
    public int PdvId { get; set; }
    public string NombrePdv { get; set; } = "";
    public int SedeId { get; set; }
}

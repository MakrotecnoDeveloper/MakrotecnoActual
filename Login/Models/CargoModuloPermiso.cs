using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;
public class CargoModuloPermiso
{
    public int Id { get; set; }

    public string? EmpresaId { get; set; }
    public int CargoId { get; set; }
    public int ModuloId { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaModificacion { get; set; }
}

public class ModuloPermisoItemVM
{
    public int ModuloId { get; set; }
    public string NombreModulo { get; set; } = string.Empty;

    // true = viene del plan (genérico), false = por actividad
    public bool EsGenerico { get; set; }

    // si el checkbox está marcado o no
    public bool Activo { get; set; }
    public string BloqueModulo { get; set; } = "General";
}

public class PermisosMenuViewModel
{
    public string? EmpresaId { get; set; }

    public int? CargoIdSeleccionado { get; set; }

    public List<SelectListItem> Cargos { get; set; } = new();

    public List<ModuloPermisoItemVM> ModulosGenericos { get; set; } = new();
    public List<ModuloPermisoItemVM> ModulosActividad { get; set; } = new();
}


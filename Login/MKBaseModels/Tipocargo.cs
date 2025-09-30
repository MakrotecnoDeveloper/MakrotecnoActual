using System;
using System.Collections.Generic;

namespace Plataforma.MKBaseModels;

public partial class Tipocargo
{
    public int IdTipo { get; set; }

    public string NombreCargo { get; set; } = null!;

    public string DescripcionCargo { get; set; } = null!;

    public string IdEmpresa { get; set; } = null!;

    public virtual Empresa IdEmpresaNavigation { get; set; } = null!;

    public virtual ICollection<MkPermisosMenu> MkPermisosMenus { get; set; } = new List<MkPermisosMenu>();

    public virtual ICollection<Sedeempleado> Sedeempleados { get; set; } = new List<Sedeempleado>();
}

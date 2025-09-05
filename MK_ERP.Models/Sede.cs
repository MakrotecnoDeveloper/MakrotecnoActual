using System;
using System.Collections.Generic;

namespace MK_ERP.Models;

public partial class Sede
{
    public int IdSede { get; set; }

    public string IdEmpresa { get; set; } = null!;

    public string NombreSede { get; set; } = null!;

    public string Ciudad { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public virtual Empresa IdEmpresaNavigation { get; set; } = null!;

    public virtual ICollection<Infopdv> Infopdvs { get; set; } = new List<Infopdv>();

    public virtual ICollection<Sedeempleado> Sedeempleados { get; set; } = new List<Sedeempleado>();
}

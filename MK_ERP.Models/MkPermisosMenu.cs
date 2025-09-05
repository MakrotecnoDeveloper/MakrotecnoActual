using System;
using System.Collections.Generic;

namespace MK_ERP.Models;

public partial class MkPermisosMenu
{
    public int Id { get; set; }

    public int IdTipoCargo { get; set; }

    public int IdMenu { get; set; }

    public bool Estado { get; set; }

    public virtual MkMenu IdMenuNavigation { get; set; } = null!;

    public virtual Tipocargo IdTipoCargoNavigation { get; set; } = null!;
}

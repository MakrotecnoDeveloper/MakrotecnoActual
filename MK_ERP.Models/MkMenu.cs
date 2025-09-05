using System;
using System.Collections.Generic;

namespace MK_ERP.Models;

public partial class MkMenu
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<MkPermisosMenu> MkPermisosMenus { get; set; } = new List<MkPermisosMenu>();
}

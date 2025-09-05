using System;
using System.Collections.Generic;

namespace MK_ERP.Models;

public partial class Infopdv
{
    public int InfopdvId { get; set; }

    public string NombreInfoPdv { get; set; } = null!;

    public int? IdSede { get; set; }

    public virtual Sede? IdSedeNavigation { get; set; }

    public virtual ICollection<Logslogin> Logslogins { get; set; } = new List<Logslogin>();

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

    public virtual ICollection<Syncpdv> Syncpdvs { get; set; } = new List<Syncpdv>();
}

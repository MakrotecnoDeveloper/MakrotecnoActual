using System;
using System.Collections.Generic;

namespace MK_ERP.Models;

public partial class Servicio
{
    public int IdServicio { get; set; }

    public string? DescripcionServicio { get; set; }

    public string? NombreServicio { get; set; }

    public virtual ICollection<Categoriaproducto> Categoriaproductos { get; set; } = new List<Categoriaproducto>();
}

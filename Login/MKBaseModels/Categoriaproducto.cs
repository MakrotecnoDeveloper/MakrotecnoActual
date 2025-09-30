using System;
using System.Collections.Generic;

namespace Plataforma.MKBaseModels;

public partial class Categoriaproducto
{
    public int IdCateProducto { get; set; }

    public string Descripcion { get; set; } = null!;

    public int? Idservicio { get; set; }

    public virtual Servicio? IdservicioNavigation { get; set; }

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}

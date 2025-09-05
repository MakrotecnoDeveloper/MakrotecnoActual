using System;
using System.Collections.Generic;

namespace MK_ERP.Models;

public partial class Producto
{
    public string CodProducto { get; set; } = null!;

    public string NombreProducto { get; set; } = null!;

    public decimal? CantidadProducto { get; set; }

    public float ValorNetoProducto { get; set; }

    public float ValorVentaProducto { get; set; }

    public int ValorUnidad { get; set; }

    public string IdEmpresa { get; set; } = null!;

    public int Estado { get; set; }

    public string Ubicacion { get; set; } = null!;

    public int? IdCatePro { get; set; }

    public virtual ICollection<Detallecompra> Detallecompras { get; set; } = new List<Detallecompra>();

    public virtual Categoriaproducto? IdCateProNavigation { get; set; }

    public virtual Empresa IdEmpresaNavigation { get; set; } = null!;

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}

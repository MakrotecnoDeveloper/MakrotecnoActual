using Login.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;

public partial class PedidoViewModel
{
    public Factura Factura { get; set; }
    public List<Producto> Productos { get; set; }
    public int? EstadoPDV { get; set; }
}
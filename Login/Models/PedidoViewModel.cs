using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;

public partial class PedidoViewModel
{
    public Factura Factura { get; set; }
    public List<Producto> Productos { get; set; }
}
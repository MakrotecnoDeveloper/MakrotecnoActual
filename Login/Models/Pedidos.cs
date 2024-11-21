using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;

public partial class Pedidos
{
    [Key]
    public int cod_pedido { get; set; }
    public int cod_factura { get; set; }
    public string? cod_producto { get; set; }
    public int cantidad { get; set; }
    public int valorNeto { get; set; }
    public int valorVenta { get; set; }
    public string? estado { get; set; }
    public int InfopdvId { get; set; }
    public DateTime fechaIngreso { get; set; }
}

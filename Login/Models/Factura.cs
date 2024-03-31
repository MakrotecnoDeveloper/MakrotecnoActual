using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;

public partial class Factura
{
    [Key]
    public int cod_factura { get; set; }
    public int cedula_cliente { get; set; }
    public int cedula { get; set; }
    public DateTime fechaVenta { get; set; }
    public string? estado { get; set; }
}

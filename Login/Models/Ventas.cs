using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;
public partial class Ventas
{
    [Key]
    public int id_venta { get; set; }
    public int ventaTotal { get; set; }
    public int ventaMakrotecno { get; set; }
    public int netoMakrotecno { get; set; }
    public int ventaRecargas { get; set; }
    public int ventaTienda { get; set; }
    public int ventaPasivos { get; set; }
    public DateTime fechaVenta { get; set; }
}

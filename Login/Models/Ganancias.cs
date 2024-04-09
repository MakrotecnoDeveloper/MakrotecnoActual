using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;

public partial class Ganancias
{
    [Key]
    public int id_ganancias { get; set; }
    public int id_venta { get; set; }
    public int gananciaMakrotecno { get; set; }
    public int gananciaTotal { get; set; }
    public int gananciaMaria { get; set; }
    public int gananciaVictor { get; set; }
    public int gananciaTeresa { get; set; }
}
using System;
using System.Collections.Generic;

namespace MK_ERP.Models;

public partial class Empresa
{
    public string IdEmpresa { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Pais { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public virtual ICollection<Empleadoempresa> Empleadoempresas { get; set; } = new List<Empleadoempresa>();

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();

    public virtual ICollection<Sede> Sedes { get; set; } = new List<Sede>();

    public virtual ICollection<Tipocargo> Tipocargos { get; set; } = new List<Tipocargo>();
}

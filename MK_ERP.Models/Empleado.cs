using System;
using System.Collections.Generic;

namespace MK_ERP.Models;

public partial class Empleado
{
    public int Cedula { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Genero { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Rh { get; set; } = null!;

    public string Celular { get; set; } = null!;

    public string Contrasena { get; set; } = null!;

    public virtual ICollection<CierreCaja> CierreCajas { get; set; } = new List<CierreCaja>();

    public virtual ICollection<Clientesplataforma> Clientesplataformas { get; set; } = new List<Clientesplataforma>();

    public virtual ICollection<Diagnosticoproblema> Diagnosticoproblemas { get; set; } = new List<Diagnosticoproblema>();

    public virtual ICollection<Empleadoempresa> Empleadoempresas { get; set; } = new List<Empleadoempresa>();

    public virtual ICollection<FlujoCaja> FlujoCajas { get; set; } = new List<FlujoCaja>();

    public virtual ICollection<Ganancia> Ganancia { get; set; } = new List<Ganancia>();

    public virtual ICollection<Logslogin> Logslogins { get; set; } = new List<Logslogin>();

    public virtual ICollection<Plataformasuscripcion> Plataformasuscripcions { get; set; } = new List<Plataformasuscripcion>();

    public virtual ICollection<Sedeempleado> Sedeempleados { get; set; } = new List<Sedeempleado>();

    public virtual ICollection<Syncpdv> Syncpdvs { get; set; } = new List<Syncpdv>();

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}

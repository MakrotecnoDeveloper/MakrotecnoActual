using System;
using System.Collections.Generic;

namespace MK_ERP.Models;

public partial class Clientesplataforma
{
    public int IdCliPltf { get; set; }

    public string NombreCliente { get; set; } = null!;

    public string CelularCliente { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Clave { get; set; } = null!;

    public int? IdPltfSuscripcion { get; set; }

    public int Cantidad { get; set; }

    public string Ppm { get; set; } = null!;

    public DateTime FechaIniPago { get; set; }

    public DateTime FechaFinPago { get; set; }

    public int ValorVenta { get; set; }

    public int ValorNeto { get; set; }

    public int CedulaEmpleado { get; set; }

    public int Estado { get; set; }

    public string ClavePerfil { get; set; } = null!;

    public virtual Empleado CedulaEmpleadoNavigation { get; set; } = null!;

    public virtual Plataformasuscripcion? IdPltfSuscripcionNavigation { get; set; }
}

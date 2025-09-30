using System;
using System.Collections.Generic;

namespace Plataforma.MKBaseModels;

public partial class Cliente
{
    public int? CedulaCliente { get; set; }

    public string? NombreCliente { get; set; }

    public string? EmpresaCliente { get; set; }

    public string? CiudadCliente { get; set; }

    public string? TelefonoCliente { get; set; }

    public string? CorreoCliente { get; set; }

    public string? DireccionCliente { get; set; }

    public int IdCliente { get; set; }

    public virtual ICollection<Dispositivo> Dispositivos { get; set; } = new List<Dispositivo>();

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}

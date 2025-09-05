using System;
using System.Collections.Generic;

namespace MK_ERP.Models;

public partial class Cliente
{
    public int CedulaCliente { get; set; }

    public string NombreCliente { get; set; } = null!;

    public string EmpresaCliente { get; set; } = null!;

    public string CiudadCliente { get; set; } = null!;

    public string TelefonoCliente { get; set; } = null!;

    public virtual ICollection<Dispositivo> Dispositivos { get; set; } = new List<Dispositivo>();

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}

using System;
using System.Collections.Generic;

namespace MK_ERP.Models;

public partial class Dispositivo
{
    public string? Marca { get; set; }

    public string? Modelo { get; set; }

    public string? Imei { get; set; }

    public string? Accesorios { get; set; }

    public int CedulaCliente { get; set; }

    public int IdDispositivo { get; set; }

    public virtual Cliente CedulaClienteNavigation { get; set; } = null!;

    public virtual ICollection<Ordenservicio> Ordenservicios { get; set; } = new List<Ordenservicio>();
}

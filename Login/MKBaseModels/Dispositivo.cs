using System;
using System.Collections.Generic;

namespace Plataforma.MKBaseModels;

public partial class Dispositivo
{
    public string? Marca { get; set; }

    public string? Modelo { get; set; }

    public string? Imei { get; set; }

    public int CedulaCliente { get; set; }

    public int IdDispositivo { get; set; }

    public string? Clave { get; set; }

    public string? Patron { get; set; }

    public DateTime FechaIngreso { get; set; }

    public string? Detalle { get; set; }

    public int TipoDispositivo { get; set; }

    public int? IdCliente { get; set; }

    public virtual Cliente? IdClienteNavigation { get; set; }

    public virtual ICollection<OrdenServicio> OrdenServicios { get; set; } = new List<OrdenServicio>();

    public virtual TipoDispositivo TipoDispositivoNavigation { get; set; } = null!;
}

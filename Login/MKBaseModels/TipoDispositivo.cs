using System;
using System.Collections.Generic;

namespace Plataforma.MKBaseModels;

public partial class TipoDispositivo
{
    public int TipoDispositivo1 { get; set; }

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<Dispositivo> Dispositivos { get; set; } = new List<Dispositivo>();
}

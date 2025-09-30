using System;
using System.Collections.Generic;

namespace Plataforma.MKBaseModels;

public partial class MkPromocione
{
    public int Id { get; set; }

    public string NombreArchivo { get; set; } = null!;

    public bool? Estado { get; set; }

    public string? Descripcion { get; set; }
}

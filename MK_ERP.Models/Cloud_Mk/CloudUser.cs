using System;
using System.Collections.Generic;

namespace MK_ERP.Models.Cloud_Mk;

public partial class CloudUser
{
    public int Id { get; set; }

    public string UsuarioCodigo { get; set; } = null!;

    public string BaseDatosCodigo { get; set; } = null!;

    public DateTime? FechaRegistro { get; set; }
}

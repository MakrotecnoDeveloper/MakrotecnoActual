//Representacion de la tabla Usuario de la base de datos
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;

public partial class Cliente
{
    [Key]
    public int cedulaCliente { get; set; }
    public string? nombreCliente { get; set; }
    public string? empresaCliente { get; set; }
    public string? ciudadCliente { get; set; }
    public string? telefonoCliente { get; set; }
}

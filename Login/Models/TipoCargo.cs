//Representacion de la tabla Usuario de la base de datos
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;

public partial class TipoCargo
{
    [Key]
    public int id_tipo { get; set; }
    public string? nombreCargo { get; set; }
    public string? descripcionCargo { get; set; }
    public string? id_empresa { get; set; }
}

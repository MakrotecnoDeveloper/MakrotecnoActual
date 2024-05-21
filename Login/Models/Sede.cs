//Representacion de la tabla Usuario de la base de datos
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;

public partial class Sede
{
    [Key]
    public int id_sede { get; set; }
    public string? id_empresa { get; set; }
    public string? nombreSede { get; set; }
    public string? ciudad { get; set; }
    public string? direccion { get; set; }
    public string? telefono { get; set; }
}
//Representacion de la tabla Usuario de la base de datos
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;

public partial class EmpleadoEmpresa
{
    [Key]
    public int id_empleadoE {  get; set; }
    public int cedula { get; set; }
    public string? id_empresa { get; set; }
}

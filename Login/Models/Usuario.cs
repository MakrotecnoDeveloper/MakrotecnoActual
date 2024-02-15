//Representacion de la tabla Usuario de la base de datos
using System;
using System.Collections.Generic;

namespace Plataforma.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string? NombreUsuario { get; set; }

    public string? Correo { get; set; }

    public string? Clave { get; set; }
    public ROL IdRol { get; set; }
}

using System;
using System.Collections.Generic;

namespace Plataforma.Models;

public class Deporte
{
    public int IdDeporte { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public ICollection<Deportista> Deportistas { get; set; } = new List<Deportista>();
    public ICollection<PruebaDeportiva> PruebasDeportivas { get; set; } = new List<PruebaDeportiva>();

}

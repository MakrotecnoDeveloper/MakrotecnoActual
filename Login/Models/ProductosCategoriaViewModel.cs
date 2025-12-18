namespace Plataforma.Models
{
    public class ProductosCategoriaViewModel
    {
        public List<CategoriaProductos> CategoriaProductos { get; set; }
        public List<Producto> Productos { get; set; }
        public int Servicio { get; set; } // o incluso un objeto Servicio completo
        public List<Servicio> Servicios { get; set; }

    }
}

public class CategoriaWebEstadoViewModel
{
    public int IdCateProducto { get; set; }
    public string Descripcion { get; set; }

    // true = EstadoWeb 1 (activo en web)
    // false = EstadoWeb 0 (inactivo en web)
    public bool Habilitada { get; set; }
    public bool TieneProductos { get; set; }
}


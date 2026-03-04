using Plataforma.Models;

namespace Plataforma.Models
{
    public class ProductosCategoriaViewModel
    {
        public List<CategoriaProductos> CategoriaProductos { get; set; }
        public List<ProductoTiendaDTO> Productos { get; set; }
        public int Servicio { get; set; } // o incluso un objeto Servicio completo
        public List<Servicio> Servicios { get; set; }
        public List<Sede> Sedes { get; set; }
        public int SedeSeleccionada { get; set; }

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

public class ProductoTiendaDTO
{
    public string Cod_Producto { get; set; }
    public string NombreProducto { get; set; }
    public string ImagenPath { get; set; }
    public string CondicionProducto { get; set; }

    public decimal CantidadProducto { get; set; }
    public decimal? PrecioVenta { get; set; }
    public int IdCatepro { get; set; }
}

public class ProductosIndexViewModel
{
    public List<Producto> Productos { get; set; }
    public List<CategoriaProductos> CategoriaProductos { get; set; }
}


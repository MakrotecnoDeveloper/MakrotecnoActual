using Microsoft.AspNetCore.Mvc.Rendering;

namespace Plataforma.Models;
public class ProductoViewModel
{
    public string? Codigo { get; set; }
    public string? Stock { get; set; }
    public string? UnidadMedida {  get; set; }
    public int VNeto { get; set; }
    public int VVenta { get; set; }
    public decimal VTotal { get; set; }
}

public class ProductoStockVm
{
    public string ProductoId { get; set; }
    public string Nombre { get; set; } = default!;
    public decimal Cantidad { get; set; }              // total o por sede según filtro
    public int? SedeId { get; set; }                   // si viene filtrado por sede
    public string? SedeNombre { get; set; }            // opcional para mostrar
}

public class PagedResult<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalRows { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalRows / PageSize);
    public List<T> Rows { get; set; } = new();
}

public class ProductosIndexVm
{
    public int? FiltroSedeId { get; set; }
    public IEnumerable<SelectListItem> Sedes { get; set; } = Enumerable.Empty<SelectListItem>();
    public string? Q { get; set; } // búsqueda
    public IEnumerable<Producto> Productos { get; set; } = Enumerable.Empty<Producto>();
    public IEnumerable<Sede> Sede { get; set; } = Enumerable.Empty<Sede>();
}

public class ProductoInsertDto
{
    // Campos que pediste
    public string? Cod_Producto { get; set; }
    public string? NombreProducto { get; set; }
    public decimal CantidadProducto { get; set; }
    public float ValorNetoProducto { get; set; }
    public float ValorVentaProducto { get; set; }
    public int ValorUnidad { get; set; }
    public string? ID_Empresa { get; set; }
    public int Estado { get; set; }
    public string? Ubicacion { get; set; }
    public int IdCatepro { get; set; }
    public int idProveedor { get; set; }

    // Solo para validación y autocompletado
    public int ServicioId { get; set; }
}

public class ResultadoAsignacion
{
    public bool Exito { get; set; }
    public string Mensaje { get; set; }
}

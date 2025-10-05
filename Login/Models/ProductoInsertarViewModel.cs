namespace Plataforma.Models;
public class ProductoInsertarViewModel
{
    public List<Servicio> Servicios { get; set; }
    public List<CategoriaProductos> Categorias { get; set; }
    public List<Proveedores> Proveedores { get; set; }
    public List<Empresas> Empresas { get; set; }
}
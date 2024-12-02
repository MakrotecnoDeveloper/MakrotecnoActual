using Plataforma.Models;

namespace Login.Models
{
    public class ProveedorProductosViewModel
    {
        public List<Proveedores> Proveedores { get; set; }
        public List<Producto> Producto { get; set; }
        public int Cod_Factura {  get; set; }
    }
}

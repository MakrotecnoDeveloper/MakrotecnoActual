namespace Plataforma.Models;
public class CompraViewModel
{
    public int IdProveedor { get; set; }
    public string CodFacturaExterno { get; set; }
    public List<DetalleCompraViewModel> Detalles { get; set; } = new List<DetalleCompraViewModel>();
}

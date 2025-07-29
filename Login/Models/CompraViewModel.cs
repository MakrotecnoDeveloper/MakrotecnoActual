namespace Plataforma.Models;
public class CompraViewModel
{
    public int IdProveedor { get; set; }
    public string CodFacturaExterno { get; set; }
    public List<DetalleCompraViewModel> Productos { get; set; } = new List<DetalleCompraViewModel>();
}

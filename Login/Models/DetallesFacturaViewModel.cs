using Plataforma.Models;

namespace Login.Models
{
    public class DetallesFacturaViewModel
    {
        public Factura Factura { get; set; }
        public List<HistoricoCompras> Compras { get; set; }
    }
}
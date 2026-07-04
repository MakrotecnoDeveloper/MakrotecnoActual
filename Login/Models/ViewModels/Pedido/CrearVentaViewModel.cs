using Plataforma.Models;

namespace Plataforma.ViewModels.Pedido
{
    public class CrearVentaViewModel
    {
        public int IdCliente { get; set; }

        public string? Conceptos { get; set; }

        public int Cedula { get; set; }

        public string? ObservacionVenta { get; set; }
        public int IdVentaCreada { get; set; }

        public List<Clientes> Clientes { get; set; } = new();
    }
}
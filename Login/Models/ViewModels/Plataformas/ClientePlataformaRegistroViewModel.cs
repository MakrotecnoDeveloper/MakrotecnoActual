namespace Plataforma.Models.ViewModels.Plataformas
{
    public class ClientePlataformaRegistroViewModel
    {
        public int? IdClienteStreaming { get; set; }

        public string? NombreCliente { get; set; }

        public string? CelularCliente { get; set; }

        public string? CorreoCliente { get; set; }

        public string? ClavePerfil { get; set; }

        public int IdPltfSuscripcion { get; set; }

        public int Cantidad { get; set; }

        public string? Ppm { get; set; }

        public DateTime FechaIniPago { get; set; }

        public DateTime FechaFinPago { get; set; }

        public int ValorVenta { get; set; }

        public int ValorNeto { get; set; }

        public int CedulaEmpleado { get; set; }

        public int Estado { get; set; } = 1;

        public List<Plataformasuscripcion>? Plataformas { get; set; }
    }
}

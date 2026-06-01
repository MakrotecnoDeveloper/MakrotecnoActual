namespace Plataforma.Models.Dto.Streaming
{
    public class CuentaClienteStreamingDTO
    {
        public int IdClienteStreaming { get; set; }

        public string? NombreCliente { get; set; }

        public string? CelularCliente { get; set; }

        public string? CorreoCliente { get; set; }

        public int IdClientePlataforma { get; set; }

        public int IdPltfSuscripcion { get; set; }

        public int IdPlataforma { get; set; }

        public string? NombrePlataforma { get; set; }

        public string? NombreSuscripcion { get; set; }

        public string? CorreoPlataforma { get; set; }

        public string? ClavePlataforma { get; set; }

        public string? ClavePerfil { get; set; }

        public int Cantidad { get; set; }

        public string? Ppm { get; set; }

        public DateTime FechaIni { get; set; }

        public DateTime FechaFin { get; set; }

        public int ValorVenta { get; set; }

        public int ValorNeto { get; set; }

        public int Estado { get; set; }
    }
}

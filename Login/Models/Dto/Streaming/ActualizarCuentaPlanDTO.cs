namespace Plataforma.Models.Dto.Streaming
{
    public class ActualizarCuentaPlanDTO
    {
        public int IdPltfSuscripcion { get; set; }

        public string? Descripcion { get; set; }

        public string? TipoCuenta { get; set; }

        public string? Correo { get; set; }

        public string? Contrasena { get; set; }

        public int Cantidad { get; set; }

        public int ValorVenta { get; set; }

        public int ValorNeto { get; set; }

        public int Estado { get; set; }
    }
    public class AsignarClienteACuentaDTO
    {
        public int IdPltfSuscripcion { get; set; }

        public int IdClienteStreaming { get; set; }

        public DateTime FechaIniPago { get; set; }

        public DateTime FechaFinPago { get; set; }

        public string? ClavePerfil { get; set; }

        public int Cantidad { get; set; }

        public string? Ppm { get; set; }

        public int ValorVenta { get; set; }

        public int ValorNeto { get; set; }

        public int Estado { get; set; }
    }
}

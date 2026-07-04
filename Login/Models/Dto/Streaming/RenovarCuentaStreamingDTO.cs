namespace Plataforma.Models.Dto.Streaming
{
    public class RenovarCuentaStreamingDTO
    {
        public int IdClientePlataforma { get; set; }

        public int IdPltfSuscripcion { get; set; }

        public string? TipoCuenta { get; set; }

        public string? CorreoPlataforma { get; set; }

        public string? ContrasenaPlataforma { get; set; }

        public string? ClavePerfil { get; set; }

        public DateTime FechaIniPago { get; set; }

        public DateTime FechaFinPago { get; set; }

        public int ValorVenta { get; set; }

        public int ValorNeto { get; set; }

        public int Estado { get; set; }
    }
}

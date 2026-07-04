namespace Plataforma.Models.Dto.Streaming
{
    public class EstructuraPlataformaDTO
    {
        public int IdPltfSuscripcion { get; set; }

        public int IdPlataforma { get; set; }

        public string? NombrePlataforma { get; set; }

        public string? DescripcionCuenta { get; set; }

        public string? CorreoPlataforma { get; set; }

        public string? ContrasenaPlataforma { get; set; }

        public string? TipoCuenta { get; set; }

        public int Cantidad { get; set; }

        public int ValorVenta { get; set; }

        public int ValorNeto { get; set; }

        public int EstadoCuenta { get; set; }

        public int TotalAsignados { get; set; }

        public int CuposDisponibles { get; set; }

        public int TotalVencidos { get; set; }

        public int TotalProximos { get; set; }

        public int TotalAlDia { get; set; }

        public List<PerfilCuentaDTO> Perfiles { get; set; } = new();
    }

    public class PerfilCuentaDTO
    {
        public int IdClientePlataforma { get; set; }

        public int IdClienteStreaming { get; set; }

        public string? NombreCliente { get; set; }

        public string? CelularCliente { get; set; }

        public string? CorreoCliente { get; set; }

        public string? ClavePerfil { get; set; }

        public string? Ppm { get; set; }

        public int Cantidad { get; set; }

        public DateTime FechaIniPago { get; set; }

        public DateTime FechaFinPago { get; set; }

        public int ValorVenta { get; set; }

        public int ValorNeto { get; set; }

        public int Estado { get; set; }

        public string? EstadoCalculado { get; set; }

        public int DiasParaVencer { get; set; }
    }
}

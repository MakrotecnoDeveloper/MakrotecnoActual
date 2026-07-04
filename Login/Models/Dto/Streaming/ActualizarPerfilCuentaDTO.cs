namespace Plataforma.Models.Dto.Streaming
{
    public class ActualizarPerfilCuentaDTO
    {
        public int IdClientePlataforma { get; set; }

        public int IdPltfSuscripcion { get; set; }

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

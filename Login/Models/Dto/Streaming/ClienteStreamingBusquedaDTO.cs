namespace Plataforma.Models.Dto.Streaming
{
    public class ClienteStreamingBusquedaDTO
    {
        public string IdsClienteStreaming { get; set; } = string.Empty;

        public string? NombreCliente { get; set; }

        public string? CelularCliente { get; set; }

        public string? CorreoCliente { get; set; }
    }
}

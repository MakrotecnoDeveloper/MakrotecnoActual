namespace Plataforma.ViewModels.Cotizacion;

public class CotizacionSeguimientoItemVm
{
    public DateTime FechaRegistro { get; set; }
    public string EstadoAnterior { get; set; } = string.Empty;
    public string EstadoNuevo { get; set; } = string.Empty;
    public string Observacion { get; set; } = string.Empty;
    public string? UsuarioRegistro { get; set; }
}
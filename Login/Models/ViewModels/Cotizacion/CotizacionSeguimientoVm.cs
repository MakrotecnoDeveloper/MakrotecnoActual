using System.ComponentModel.DataAnnotations;

namespace Plataforma.ViewModels.Cotizacion;

public class CotizacionSeguimientoVm
{
    public int IdCotizacion { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un estado.")]
    public string EstadoNuevo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe ingresar una observación.")]
    public string Observacion { get; set; } = string.Empty;
}
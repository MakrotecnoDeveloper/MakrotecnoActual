using Plataforma.Models.Dto.Pedido;
using Plataforma.ViewModels.Cotizacion;
using System.Security.Claims;

namespace Plataforma.Servicios.Contrato;

public interface ICotizacionService
{
    Task<List<CotizacionIndexVm>> ObtenerCotizacionesAsync(
        ClaimsPrincipal usuario,
        string? estado,
        string? buscar);

    Task<CotizacionCrearVm> ConstruirCrearVmAsync(ClaimsPrincipal usuario);

    Task<int> CrearCotizacionAsync(CotizacionCrearVm vm, ClaimsPrincipal usuario);

    Task<CotizacionDetalleVm?> ObtenerDetalleAsync(int idCotizacion, ClaimsPrincipal usuario);

    Task<bool> RegistrarSeguimientoAsync(CotizacionSeguimientoVm vm, ClaimsPrincipal usuario);

    Task<CotizacionConvertirVentaVm?> ObtenerParaConvertirVentaAsync(int idCotizacion, ClaimsPrincipal usuario);

    Task<int> ConvertirCotizacionAVentaAsync(CotizacionConvertirVentaVm vm, ClaimsPrincipal usuario);

    Task<bool> AnularCotizacionAsync(int idCotizacion, string observacion, ClaimsPrincipal usuario);
}
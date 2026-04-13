using Plataforma.Models;
using Plataforma.Models.ViewModels.Pos;
using System.Security.Claims;
namespace Plataforma.Servicios.Contrato
{
    public interface IPosService
    {
        Task<PosPantallaViewModel> ObtenerPantallaAsync(ClaimsPrincipal usuario);
        Task<PosProductosPaginadosVm> BuscarProductosAsync(string? texto, string? categoria, int pagina, int tamanoPagina, ClaimsPrincipal usuario);
        Task<PosProductoVm?> BuscarProductoPorCodigoAsync(string codigo, ClaimsPrincipal usuario);
        Task<PosResultadoVm> FacturarVentaPosAsync(PosCrearVentaRequest request, ClaimsPrincipal usuario);
    }
}
using Plataforma.ViewModels.CuentasCobro;
using System.Security.Claims;

namespace Plataforma.Servicios.Contrato;

public interface ICuentaCobroService
{
    Task<List<CuentaCobroIndexVm>> ObtenerCuentasCobroAsync(ClaimsPrincipal usuario);

    Task<CuentaCobroCrearVm> ConstruirDesdeFacturaAsync(int idFactura, ClaimsPrincipal usuario);

    Task<int> CrearDesdeFacturaAsync(CuentaCobroCrearVm vm, ClaimsPrincipal usuario);

    Task<CuentaCobroDetalleVm?> ObtenerDetalleAsync(int idCuentaCobro, ClaimsPrincipal usuario);

    Task<bool> CambiarEstadoAsync(int idCuentaCobro, string nuevoEstado, ClaimsPrincipal usuario);
}
using Plataforma.ViewModels.CuentasBancarias;
using System.Security.Claims;

namespace Plataforma.Servicios.Contrato;

public interface ICuentaBancariaEmpresaService
{
    Task<List<CuentaBancariaEmpresaVm>> ObtenerTodasAsync(ClaimsPrincipal usuario);

    Task<CuentaBancariaEmpresaVm?> ObtenerPorIdAsync(int id, ClaimsPrincipal usuario);

    Task<int> CrearAsync(CuentaBancariaEmpresaVm vm, ClaimsPrincipal usuario);

    Task<bool> ActualizarAsync(CuentaBancariaEmpresaVm vm, ClaimsPrincipal usuario);

    Task<bool> CambiarEstadoAsync(int id, ClaimsPrincipal usuario);

    Task<bool> MarcarPrincipalAsync(int id, ClaimsPrincipal usuario);
}
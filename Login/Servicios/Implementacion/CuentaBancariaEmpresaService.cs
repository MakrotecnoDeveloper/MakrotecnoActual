using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using Plataforma.ViewModels.CuentasBancarias;
using System.Security.Claims;

namespace Plataforma.Servicios.Implementacion;

public class CuentaBancariaEmpresaService : ICuentaBancariaEmpresaService
{
    private readonly BaseAdmContext _dbContext;

    public CuentaBancariaEmpresaService(BaseAdmContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<CuentaBancariaEmpresaVm>> ObtenerTodasAsync(ClaimsPrincipal usuario)
    {
        var ctx = ObtenerContextoClaims(usuario);

        return await _dbContext.CuentasBancariasEmpresa
            .AsNoTracking()
            .Where(x => x.IdEmpresa == ctx.EmpresaId)
            .OrderByDescending(x => x.EsPrincipal)
            .ThenBy(x => x.Banco)
            .Select(x => new CuentaBancariaEmpresaVm
            {
                IdCuentaBancariaEmpresa = x.IdCuentaBancariaEmpresa,
                TitularCuenta = x.TitularCuenta,
                DocumentoTitular = x.DocumentoTitular,
                Banco = x.Banco,
                TipoCuenta = x.TipoCuenta,
                NumeroCuenta = x.NumeroCuenta,
                Convenio = x.Convenio,
                Observacion = x.Observacion,
                EsPrincipal = x.EsPrincipal,
                Estado = x.Estado,
                FechaCreacion = x.FechaCreacion
            })
            .ToListAsync();
    }

    public async Task<CuentaBancariaEmpresaVm?> ObtenerPorIdAsync(int id, ClaimsPrincipal usuario)
    {
        var ctx = ObtenerContextoClaims(usuario);

        var cuenta = await _dbContext.CuentasBancariasEmpresa
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.IdCuentaBancariaEmpresa == id &&
                x.IdEmpresa == ctx.EmpresaId);

        if (cuenta == null)
            return null;

        return new CuentaBancariaEmpresaVm
        {
            IdCuentaBancariaEmpresa = cuenta.IdCuentaBancariaEmpresa,
            TitularCuenta = cuenta.TitularCuenta,
            DocumentoTitular = cuenta.DocumentoTitular,
            Banco = cuenta.Banco,
            TipoCuenta = cuenta.TipoCuenta,
            NumeroCuenta = cuenta.NumeroCuenta,
            Convenio = cuenta.Convenio,
            Observacion = cuenta.Observacion,
            EsPrincipal = cuenta.EsPrincipal,
            Estado = cuenta.Estado,
            FechaCreacion = cuenta.FechaCreacion
        };
    }

    public async Task<int> CrearAsync(CuentaBancariaEmpresaVm vm, ClaimsPrincipal usuario)
    {
        var ctx = ObtenerContextoClaims(usuario);

        if (vm.EsPrincipal)
        {
            var principales = await _dbContext.CuentasBancariasEmpresa
                .Where(x => x.IdEmpresa == ctx.EmpresaId && x.EsPrincipal)
                .ToListAsync();

            foreach (var item in principales)
            {
                item.EsPrincipal = false;
            }
        }

        var cuenta = new CuentaBancariaEmpresa
        {
            IdEmpresa = ctx.EmpresaId,
            TitularCuenta = vm.TitularCuenta.Trim(),
            DocumentoTitular = vm.DocumentoTitular?.Trim(),
            Banco = vm.Banco.Trim(),
            TipoCuenta = vm.TipoCuenta.Trim(),
            NumeroCuenta = vm.NumeroCuenta.Trim(),
            Convenio = vm.Convenio?.Trim(),
            Observacion = vm.Observacion?.Trim(),
            EsPrincipal = vm.EsPrincipal,
            Estado = true,
            FechaCreacion = DateTime.Now
        };

        _dbContext.CuentasBancariasEmpresa.Add(cuenta);
        await _dbContext.SaveChangesAsync();

        return cuenta.IdCuentaBancariaEmpresa;
    }

    public async Task<bool> ActualizarAsync(CuentaBancariaEmpresaVm vm, ClaimsPrincipal usuario)
    {
        var ctx = ObtenerContextoClaims(usuario);

        var cuenta = await _dbContext.CuentasBancariasEmpresa
            .FirstOrDefaultAsync(x =>
                x.IdCuentaBancariaEmpresa == vm.IdCuentaBancariaEmpresa &&
                x.IdEmpresa == ctx.EmpresaId);

        if (cuenta == null)
            return false;

        if (vm.EsPrincipal)
        {
            var principales = await _dbContext.CuentasBancariasEmpresa
                .Where(x =>
                    x.IdEmpresa == ctx.EmpresaId &&
                    x.IdCuentaBancariaEmpresa != cuenta.IdCuentaBancariaEmpresa &&
                    x.EsPrincipal)
                .ToListAsync();

            foreach (var item in principales)
            {
                item.EsPrincipal = false;
            }
        }

        cuenta.TitularCuenta = vm.TitularCuenta.Trim();
        cuenta.DocumentoTitular = vm.DocumentoTitular?.Trim();
        cuenta.Banco = vm.Banco.Trim();
        cuenta.TipoCuenta = vm.TipoCuenta.Trim();
        cuenta.NumeroCuenta = vm.NumeroCuenta.Trim();
        cuenta.Convenio = vm.Convenio?.Trim();
        cuenta.Observacion = vm.Observacion?.Trim();
        cuenta.EsPrincipal = vm.EsPrincipal;
        cuenta.Estado = vm.Estado;

        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CambiarEstadoAsync(int id, ClaimsPrincipal usuario)
    {
        var ctx = ObtenerContextoClaims(usuario);

        var cuenta = await _dbContext.CuentasBancariasEmpresa
            .FirstOrDefaultAsync(x =>
                x.IdCuentaBancariaEmpresa == id &&
                x.IdEmpresa == ctx.EmpresaId);

        if (cuenta == null)
            return false;

        cuenta.Estado = !cuenta.Estado;

        if (!cuenta.Estado)
            cuenta.EsPrincipal = false;

        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> MarcarPrincipalAsync(int id, ClaimsPrincipal usuario)
    {
        var ctx = ObtenerContextoClaims(usuario);

        var cuenta = await _dbContext.CuentasBancariasEmpresa
            .FirstOrDefaultAsync(x =>
                x.IdCuentaBancariaEmpresa == id &&
                x.IdEmpresa == ctx.EmpresaId &&
                x.Estado);

        if (cuenta == null)
            return false;

        var cuentasEmpresa = await _dbContext.CuentasBancariasEmpresa
            .Where(x => x.IdEmpresa == ctx.EmpresaId)
            .ToListAsync();

        foreach (var item in cuentasEmpresa)
        {
            item.EsPrincipal = false;
        }

        cuenta.EsPrincipal = true;

        await _dbContext.SaveChangesAsync();

        return true;
    }

    private (int Cedula, string EmpresaId, int SedeId, int PdvId) ObtenerContextoClaims(ClaimsPrincipal usuario)
    {
        var cedulaStr = usuario.FindFirst("Cedula")?.Value;
        var empresaId = usuario.FindFirst("EmpresaId")?.Value;
        var sedeIdStr = usuario.FindFirst("SedeId")?.Value;
        var pdvIdStr = usuario.FindFirst("PdvId")?.Value;

        if (!int.TryParse(cedulaStr, out var cedula))
            throw new Exception("No se pudo obtener la cédula del usuario autenticado.");

        if (string.IsNullOrWhiteSpace(empresaId))
            throw new Exception("No se pudo obtener la empresa del usuario autenticado.");

        if (!int.TryParse(sedeIdStr, out var sedeId) || sedeId <= 0)
            throw new Exception("No se pudo obtener la sede actual del usuario.");

        if (!int.TryParse(pdvIdStr, out var pdvId) || pdvId <= 0)
            throw new Exception("No se pudo obtener el PDV actual del usuario.");

        return (cedula, empresaId, sedeId, pdvId);
    }
}
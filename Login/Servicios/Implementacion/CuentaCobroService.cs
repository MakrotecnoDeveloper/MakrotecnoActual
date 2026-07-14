using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using Plataforma.ViewModels.CuentasCobro;
using System.Security.Claims;

namespace Plataforma.Servicios.Implementacion;

public class CuentaCobroService : ICuentaCobroService
{
    private readonly BaseAdmContext _dbContext;

    public CuentaCobroService(BaseAdmContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<CuentaCobroIndexVm>> ObtenerCuentasCobroAsync(ClaimsPrincipal usuario)
    {
        var ctx = ObtenerContextoClaims(usuario);

        return await _dbContext.CuentasCobro
            .AsNoTracking()
            .Where(x =>
                x.IdEmpresa == ctx.EmpresaId &&
                x.SedeId == ctx.SedeId &&
                x.InfopdvId == ctx.PdvId)
            .OrderByDescending(x => x.FechaCuentaCobro)
            .Select(x => new CuentaCobroIndexVm
            {
                IdCuentaCobro = x.IdCuentaCobro,
                NumeroCuentaCobro = x.NumeroCuentaCobro,
                NumeroFactura = x.Factura.NumeroFactura,
                NombreCliente = x.NombreCliente ?? "",
                FechaCuentaCobro = x.FechaCuentaCobro,
                ValorNetoPagar = x.ValorNetoPagar,
                EstadoCuentaCobro = x.EstadoCuentaCobro
            })
            .ToListAsync();
    }

    public async Task<CuentaCobroCrearVm> ConstruirDesdeFacturaAsync(int idFactura, ClaimsPrincipal usuario)
    {
        var ctx = ObtenerContextoClaims(usuario);

        var factura = await _dbContext.Factura
            .Include(f => f.Venta)
                .ThenInclude(v => v.Pedidos)
                    .ThenInclude(p => p.Producto)
            .FirstOrDefaultAsync(f => f.IdFactura == idFactura);

        if (factura == null)
            throw new Exception("La factura no existe.");

        if (factura.Venta == null)
            throw new Exception("La factura no tiene venta asociada.");

        var venta = factura.Venta;

        var cliente = await _dbContext.Clientes
            .FirstOrDefaultAsync(c => c.IdCliente == venta.IdCliente);

        if (cliente == null)
            throw new Exception("No se encontró el cliente de la venta.");

        var yaExiste = await _dbContext.CuentasCobro
            .AnyAsync(x => x.IdFactura == idFactura);

        if (yaExiste)
            throw new Exception("Esta factura ya tiene una cuenta de cobro generada.");

        var cuentas = await _dbContext.CuentasBancariasEmpresa
            .Where(x => x.IdEmpresa == ctx.EmpresaId && x.Estado)
            .OrderByDescending(x => x.EsPrincipal)
            .ThenBy(x => x.Banco)
            .Select(x => new SelectListItem
            {
                Value = x.IdCuentaBancariaEmpresa.ToString(),
                Text = $"{x.Banco} - {x.TipoCuenta} - {x.NumeroCuenta}"
            })
            .ToListAsync();

        if (!cuentas.Any())
            throw new Exception("No hay cuentas bancarias activas configuradas para esta empresa.");

        var detalles = venta.Pedidos.Select(p => new CuentaCobroDetalleItemVm
        {
            CodigoProducto = p.Codigo,
            NombreProducto = p.Producto?.NombreProducto ?? p.Codigo ?? "Producto",
            Cantidad = p.Stock,
            ValorUnitario = p.VVenta,
            IvaValor = p.IvaValor ?? 0,
            TotalLinea = p.SubTotal
        }).ToList();

        var detalleServicio = string.Join(", ", detalles.Select(d => d.NombreProducto));

        return new CuentaCobroCrearVm
        {
            IdFactura = factura.IdFactura,
            IdVenta = venta.IdVenta,
            IdCliente = venta.IdCliente,
            NumeroFactura = factura.NumeroFactura,
            NumeroCuentaCobro = await GenerarNumeroCuentaCobroAsync(),

            FechaCuentaCobro = DateTime.Now,
            Ciudad = "Cali",

            NombreCliente = cliente.NombreCliente,
            DocumentoCliente = cliente.CedulaCliente?.ToString(),
            DireccionCliente = cliente.DireccionCliente,
            TelefonoCliente = cliente.TelefonoCliente,
            CorreoCliente = cliente.CorreoCliente,
            BancoCliente = "N/A",

            DetalleServicio = detalleServicio,

            ValorTotalServicio = factura.Total,
            RetencionFuente = 0,
            OtrasDeducciones = 0,
            ValorNetoPagar = factura.Total,

            CodigoDocumento = "DCCC-000-01",
            VersionDocumento = "1",
            FechaAprobacionDocumento = new DateTime(2024, 5, 10),

            Declaracion = $"Declaro bajo la gravedad de juramento que los productos y/o servicios relacionados en la presente cuenta de cobro fueron efectivamente suministrados a {cliente.NombreCliente}, y que no tengo vínculo laboral con la empresa.",

            CedulaContratista = ctx.Cedula.ToString(),
            CiudadExpedicionContratista = "Cali",

            CuentasBancarias = cuentas,
            IdCuentaBancariaEmpresa = int.Parse(cuentas.First().Value),

            Detalles = detalles
        };
    }

    public async Task<int> CrearDesdeFacturaAsync(CuentaCobroCrearVm vm, ClaimsPrincipal usuario)
    {
        var ctx = ObtenerContextoClaims(usuario);

        var factura = await _dbContext.Factura
            .Include(f => f.Venta)
                .ThenInclude(v => v.Pedidos)
                    .ThenInclude(p => p.Producto)
            .FirstOrDefaultAsync(f => f.IdFactura == vm.IdFactura);

        if (factura == null)
            throw new Exception("La factura no existe.");

        var venta = factura.Venta;

        if (venta == null)
            throw new Exception("La factura no tiene venta asociada.");

        var cuentaBancaria = await _dbContext.CuentasBancariasEmpresa
            .FirstOrDefaultAsync(x =>
                x.IdCuentaBancariaEmpresa == vm.IdCuentaBancariaEmpresa &&
                x.IdEmpresa == ctx.EmpresaId &&
                x.Estado);

        if (cuentaBancaria == null)
            throw new Exception("La cuenta bancaria seleccionada no existe o está inactiva.");

        var yaExiste = await _dbContext.CuentasCobro
            .AnyAsync(x => x.IdFactura == vm.IdFactura);

        if (yaExiste)
            throw new Exception("Esta factura ya tiene una cuenta de cobro generada.");

        var valorNeto = vm.ValorTotalServicio - vm.RetencionFuente - vm.OtrasDeducciones;

        if (valorNeto < 0)
            throw new Exception("El valor neto a pagar no puede ser negativo.");

        var cuentaCobro = new CuentaCobro
        {
            NumeroCuentaCobro = await GenerarNumeroCuentaCobroAsync(),

            CodigoDocumento = vm.CodigoDocumento,
            VersionDocumento = vm.VersionDocumento,
            FechaAprobacionDocumento = vm.FechaAprobacionDocumento,

            IdFactura = factura.IdFactura,
            IdVenta = venta.IdVenta,
            IdCliente = venta.IdCliente,
            IdCuentaBancariaEmpresa = cuentaBancaria.IdCuentaBancariaEmpresa,

            FechaCuentaCobro = vm.FechaCuentaCobro,
            Ciudad = vm.Ciudad,

            NombreCliente = vm.NombreCliente,
            DocumentoCliente = vm.DocumentoCliente,
            DireccionCliente = vm.DireccionCliente,
            TelefonoCliente = vm.TelefonoCliente,
            CorreoCliente = vm.CorreoCliente,
            BancoCliente = vm.BancoCliente,

            DetalleServicio = vm.DetalleServicio,

            ValorTotalServicio = vm.ValorTotalServicio,
            RetencionFuente = vm.RetencionFuente,
            OtrasDeducciones = vm.OtrasDeducciones,
            ValorNetoPagar = valorNeto,

            Declaracion = vm.Declaracion,
            CedulaContratista = vm.CedulaContratista,
            CiudadExpedicionContratista = vm.CiudadExpedicionContratista,

            EstadoCuentaCobro = "Pendiente",

            IdEmpresa = ctx.EmpresaId,
            SedeId = ctx.SedeId,
            InfopdvId = ctx.PdvId,
            CedulaUsuario = ctx.Cedula,
            CreadoPor = usuario.Identity?.Name,
            FechaCreacion = DateTime.Now,

            Detalles = venta.Pedidos.Select(p => new CuentaCobroDetalle
            {
                CodigoProducto = p.Codigo,
                NombreProducto = p.Producto?.NombreProducto ?? p.Codigo ?? "Producto",
                Cantidad = p.Stock,
                ValorUnitario = p.VVenta,
                IvaValor = p.IvaValor ?? 0,
                TotalLinea = p.SubTotal
            }).ToList()
        };

        _dbContext.CuentasCobro.Add(cuentaCobro);
        await _dbContext.SaveChangesAsync();

        return cuentaCobro.IdCuentaCobro;
    }

    public async Task<CuentaCobroDetalleVm?> ObtenerDetalleAsync(int idCuentaCobro, ClaimsPrincipal usuario)
    {
        var ctx = ObtenerContextoClaims(usuario);

        var cuenta = await _dbContext.CuentasCobro
            .Include(x => x.Detalles)
            .Include(x => x.CuentaBancariaEmpresa)
            .Include(x => x.Factura)
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.IdCuentaCobro == idCuentaCobro &&
                x.IdEmpresa == ctx.EmpresaId &&
                x.SedeId == ctx.SedeId &&
                x.InfopdvId == ctx.PdvId);

        if (cuenta == null)
            return null;

        var empresa = await _dbContext.Empresas
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id_empresa == ctx.EmpresaId);

        return new CuentaCobroDetalleVm
        {
            IdCuentaCobro = cuenta.IdCuentaCobro,
            NumeroCuentaCobro = cuenta.NumeroCuentaCobro,
            IdFactura = cuenta.IdFactura,
            NumeroFactura = cuenta.Factura.NumeroFactura,

            FechaCuentaCobro = cuenta.FechaCuentaCobro,
            Ciudad = cuenta.Ciudad,

            CodigoDocumento = cuenta.CodigoDocumento,
            VersionDocumento = cuenta.VersionDocumento,
            FechaAprobacionDocumento = cuenta.FechaAprobacionDocumento,

            NombreCliente = cuenta.NombreCliente,
            DocumentoCliente = cuenta.DocumentoCliente,
            DireccionCliente = cuenta.DireccionCliente,
            TelefonoCliente = cuenta.TelefonoCliente,
            CorreoCliente = cuenta.CorreoCliente,
            BancoCliente = cuenta.BancoCliente,

            DetalleServicio = cuenta.DetalleServicio,

            ValorTotalServicio = cuenta.ValorTotalServicio,
            RetencionFuente = cuenta.RetencionFuente,
            OtrasDeducciones = cuenta.OtrasDeducciones,
            ValorNetoPagar = cuenta.ValorNetoPagar,

            Declaracion = cuenta.Declaracion,
            CedulaContratista = cuenta.CedulaContratista,
            CiudadExpedicionContratista = cuenta.CiudadExpedicionContratista,
            FirmaPath = cuenta.FirmaPath,

            EstadoCuentaCobro = cuenta.EstadoCuentaCobro,

            Empresa = empresa,
            CuentaBancaria = cuenta.CuentaBancariaEmpresa,

            Detalles = cuenta.Detalles.Select(d => new CuentaCobroDetalleItemVm
            {
                CodigoProducto = d.CodigoProducto,
                NombreProducto = d.NombreProducto,
                Cantidad = d.Cantidad,
                ValorUnitario = d.ValorUnitario,
                IvaValor = d.IvaValor,
                TotalLinea = d.TotalLinea
            }).ToList()
        };
    }

    public async Task<bool> CambiarEstadoAsync(int idCuentaCobro, string nuevoEstado, ClaimsPrincipal usuario)
    {
        var ctx = ObtenerContextoClaims(usuario);

        var cuenta = await _dbContext.CuentasCobro
            .FirstOrDefaultAsync(x =>
                x.IdCuentaCobro == idCuentaCobro &&
                x.IdEmpresa == ctx.EmpresaId &&
                x.SedeId == ctx.SedeId &&
                x.InfopdvId == ctx.PdvId);

        if (cuenta == null)
            return false;

        cuenta.EstadoCuentaCobro = nuevoEstado;
        await _dbContext.SaveChangesAsync();

        return true;
    }

    private async Task<string> GenerarNumeroCuentaCobroAsync()
    {
        var consecutivo = await _dbContext.CuentasCobro.CountAsync() + 1;
        return consecutivo.ToString();
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
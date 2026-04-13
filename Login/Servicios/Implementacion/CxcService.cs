using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Models.ViewModels.Cxc;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Servicios.Implementacion
{
    public class CxcService : ICxcService
    {
        private readonly BaseAdmContext _dbContext;

        public CxcService(BaseAdmContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(bool Ok, string Mensaje, int? IdCxc)> CrearDesdeVentaAsync(
            int idVenta,
            int idCliente,
            decimal totalCredito,
            DateTime? fechaVencimiento,
            string? observacion = null)
        {
            if (totalCredito <= 0)
                return (false, "El valor a crédito debe ser mayor a cero.", null);

            if (!fechaVencimiento.HasValue)
                return (false, "Debe indicar fecha de vencimiento para el crédito.", null);

            var venta = await _dbContext.Ventas
                .FirstOrDefaultAsync(v => v.IdVenta == idVenta);

            if (venta == null)
                return (false, "La venta no existe.", null);

            var existente = await _dbContext.CxcVentas
                .FirstOrDefaultAsync(x => x.IdVenta == idVenta && x.EstadoCxc != "Anulada");

            if (existente != null)
                return (false, "La venta ya tiene una cuenta por cobrar activa.", existente.IdCxc);

            var cxc = new CxcVentas
            {
                IdVenta = idVenta,
                IdCliente = idCliente,
                Total = totalCredito,
                SaldoPendiente = totalCredito,
                EstadoCxc = "Abierta",
                FechaCreacion = DateTime.Now,
                FechaVencimiento = fechaVencimiento,
                Observacion = observacion
            };

            _dbContext.CxcVentas.Add(cxc);
            await _dbContext.SaveChangesAsync();

            return (true, "Cuenta por cobrar creada correctamente.", cxc.IdCxc);
        }

        public async Task<(bool Ok, string Mensaje)> RegistrarPagoAsync(RegistrarPagoCxcViewModel model)
        {
            if (model.MontoPago <= 0)
                return (false, "El monto del pago debe ser mayor a cero.");

            var cxc = await _dbContext.CxcVentas
                .Include(x => x.Pagos)
                .FirstOrDefaultAsync(x => x.IdCxc == model.IdCxc);

            if (cxc == null)
                return (false, "La cuenta por cobrar no existe.");

            if (cxc.EstadoCxc == "Pagada")
                return (false, "La cuenta ya está pagada.");

            if (cxc.EstadoCxc == "Anulada")
                return (false, "La cuenta está anulada.");

            if (model.MontoPago > cxc.SaldoPendiente)
                return (false, $"El pago no puede superar el saldo pendiente ({cxc.SaldoPendiente:N2}).");

            var pago = new CxcPagos
            {
                IdCxc = model.IdCxc,
                IdMetodo = model.IdMetodo,
                MontoPago = model.MontoPago,
                FechaPago = model.FechaPago,
                Observacion = model.Observacion
            };

            _dbContext.CxcPagos.Add(pago);
            await _dbContext.SaveChangesAsync();

            return await RecalcularSaldoAsync(model.IdCxc);
        }

        public async Task<(bool Ok, string Mensaje)> RecalcularSaldoAsync(int idCxc)
        {
            var cxc = await _dbContext.CxcVentas
                .Include(x => x.Pagos)
                .FirstOrDefaultAsync(x => x.IdCxc == idCxc);

            if (cxc == null)
                return (false, "La cuenta por cobrar no existe.");

            var totalPagado = cxc.Pagos.Sum(x => x.MontoPago);
            var saldo = cxc.Total - totalPagado;

            if (saldo < 0)
                saldo = 0;

            cxc.SaldoPendiente = saldo;

            if (saldo == 0)
                cxc.EstadoCxc = "Pagada";
            else if (saldo < cxc.Total)
                cxc.EstadoCxc = "Parcial";
            else
                cxc.EstadoCxc = "Abierta";

            await _dbContext.SaveChangesAsync();
            return (true, "Saldo recalculado correctamente.");
        }

        public async Task<CxcVentas?> ObtenerPorVentaAsync(int idVenta)
        {
            return await _dbContext.CxcVentas
                .Include(x => x.Pagos)
                .FirstOrDefaultAsync(x => x.IdVenta == idVenta && x.EstadoCxc != "Anulada");
        }

        public async Task<CxcDetalleViewModel?> ObtenerDetalleAsync(int idCxc)
        {
            var cxc = await _dbContext.CxcVentas
                .Include(x => x.Pagos)
                    .ThenInclude(p => p.MetodoPagos)
                .FirstOrDefaultAsync(x => x.IdCxc == idCxc);

            if (cxc == null) return null;

            var cliente = await _dbContext.Clientes
                .FirstOrDefaultAsync(c => c.IdCliente == cxc.IdCliente);

            return new CxcDetalleViewModel
            {
                IdCxc = cxc.IdCxc,
                IdVenta = cxc.IdVenta,
                IdCliente = cxc.IdCliente,
                Cliente = cliente?.NombreCliente ?? "Sin nombre",
                CedulaCliente = cliente?.CedulaCliente,
                Total = cxc.Total,
                SaldoPendiente = cxc.SaldoPendiente,
                EstadoCxc = cxc.EstadoCxc,
                FechaCreacion = cxc.FechaCreacion,
                FechaVencimiento = cxc.FechaVencimiento,
                Observacion = cxc.Observacion,
                TotalPagado = cxc.Pagos.Sum(x => x.MontoPago),
                Pagos = cxc.Pagos
                    .OrderByDescending(x => x.FechaPago)
                    .Select(x => new CxcPagoItemVm
                    {
                        IdPago = x.IdPago,
                        FechaPago = x.FechaPago,
                        MontoPago = x.MontoPago,
                        Observacion = x.Observacion,
                        MetodoPago = x.MetodoPagos != null ? x.MetodoPagos.ToString() : "Método"
                    }).ToList()
            };
        }

        public async Task<List<MetodoPagos>> ObtenerMetodosPagoAsync()
        {
            return await _dbContext.MetodoPagos.ToListAsync();
        }
        public async Task<CxcIndexViewModel> ObtenerListadoAsync(string? texto, string? estado)
        {
            var query =
                from cxc in _dbContext.CxcVentas
                join cli in _dbContext.Clientes on cxc.IdCliente equals cli.IdCliente
                select new CxcListadoItemVm
                {
                    IdCxc = cxc.IdCxc,
                    IdVenta = cxc.IdVenta,
                    Cliente = cli.NombreCliente ?? "Sin nombre",
                    CedulaCliente = cli.CedulaCliente,
                    Total = cxc.Total,
                    SaldoPendiente = cxc.SaldoPendiente,
                    EstadoCxc = cxc.EstadoCxc,
                    FechaCreacion = cxc.FechaCreacion,
                    FechaVencimiento = cxc.FechaVencimiento
                };

            if (!string.IsNullOrWhiteSpace(texto))
            {
                texto = texto.Trim();

                query = query.Where(x =>
                    x.Cliente.Contains(texto) ||
                    (x.CedulaCliente.HasValue && x.CedulaCliente.Value.ToString().Contains(texto)) ||
                    x.IdVenta.ToString().Contains(texto));
            }

            if (!string.IsNullOrWhiteSpace(estado))
            {
                query = query.Where(x => x.EstadoCxc == estado);
            }

            return new CxcIndexViewModel
            {
                Texto = texto,
                Estado = estado,
                Items = await query
                    .OrderByDescending(x => x.FechaCreacion)
                    .ToListAsync()
            };
        }
    }
}
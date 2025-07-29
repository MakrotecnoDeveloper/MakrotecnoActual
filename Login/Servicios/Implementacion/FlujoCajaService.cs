using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Servicios.Implementacion
{
    public class FlujoCajaService : IFlujoCajaService
    {
        private readonly BaseAdmContext _dbContext;
        public FlujoCajaService(BaseAdmContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<FlujoCaja>> ObtenerMovimientosPorFechaAsync(DateTime fecha)
        {
            return await _dbContext.FlujoCajas
                .Where(f => f.Fecha.Date == fecha.Date)
                .OrderBy(f => f.IdFlujoCaja)
                .ToListAsync();
        }
        public async Task<(bool, string)> RegistrarCierreAsync(CierreCajaViewModel model, int cedula)
        {
            var hoy = DateTime.Today;

            var facturasHoy = await _dbContext.Factura
                .Where(f => f.FechaEmision.Date == hoy)
                .Include(f => f.Venta)
                .ToListAsync();

            var facturasDelEmpleado = facturasHoy
                .Where(f => f.Venta != null && f.Venta.Cedula == cedula)
                .ToList();

            if (!facturasDelEmpleado.Any())
                return (false, "No se encontraron facturas de este día realizadas por usted.");

            decimal totalFacturado = facturasDelEmpleado.Sum(f => f.Total);
            model.TotalFacturado = totalFacturado;

            decimal ingresos = model.Efectivo + model.Transferencia;
            decimal egresos = model.GastoEfectivo + model.GastoTransferencia;
            decimal diferencia = ingresos - egresos;

            var cierre = new FlujoCaja
            {
                Fecha = hoy,
                TipoMovimiento = "Cierre Diario",
                Monto = ingresos,
                Concepto = $"Cierre de caja - Efectivo: {model.Efectivo}, Transferencia: {model.Transferencia}, " +
                          $"Gastos Efectivo: {model.GastoEfectivo}, Gastos Transf.: {model.GastoTransferencia}, " +
                          $"Total Facturado: {totalFacturado}, Diferencia: {diferencia}",
                Cedula = cedula
            };

            _dbContext.FlujoCajas.Add(cierre);
            await _dbContext.SaveChangesAsync();

            return (true, null);
        }
        public async Task<decimal> ObtenerTotalFacturadoHoyAsync()
        {
            var hoy = DateTime.Today;
            return await _dbContext.Factura
                .Where(f => f.FechaEmision.Date == hoy)
                .SumAsync(f => (decimal?)f.Total) ?? 0m;
        }
    }
}
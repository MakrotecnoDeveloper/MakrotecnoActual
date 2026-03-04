using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using System.Text.Json;

namespace Plataforma.Servicios.Implementacion
{
    public class FlujoCajaService : IFlujoCajaService
    {
        private readonly BaseAdmContext _dbContext;
        public FlujoCajaService(BaseAdmContext dbContext)
        {
            _dbContext = dbContext;
        }
        // Nuevo servicio para la vista Index
        public async Task<IndexFlujoCajaVM> ObtenerResumenDeHoyAsync()
        {
            var fecha = DateTime.Today;

            var movimientos = await _dbContext.CierreCajas
                .Where(f => f.Fecha.Date == fecha.Date)
                .OrderBy(f => f.IdFlujoCaja)
                .ToListAsync();

            // Toma solo cierres del día
            var cierres = movimientos.Where(m => m.TipoMovimiento == "Cierre Diario").ToList();

            // Ingresos = suma del Monto de los cierres
            var ingresos = cierres.Sum(m => m.Monto);

            // Egresos = suma de la columna Gastos (null-safe)
            decimal? gastosEfectivos = cierres.Sum(m => m.GastosEfectivo);
            decimal? gastosTransferencia = cierres.Sum(m => m.GastosTransferencia);
            decimal? egresosTotales = gastosEfectivos + gastosTransferencia;

            var vm = new IndexFlujoCajaVM
            {
                Fecha = fecha,
                Movimientos = movimientos,
                TotalIngresos = ingresos,
                TotalEgresos = (decimal)egresosTotales
                // Balance se calcula en la propiedad del VM (Ingresos - Egresos)
            };

            // Deserializa conceptos por cada cierre (para el acordeón de detalle)
            foreach (var c in cierres)
            {
                var cierreVm = new CierreConConceptosVM
                {
                    IdFlujoCaja = c.IdFlujoCaja,
                    Fecha = c.Fecha,
                    Cedula = c.Cedula,
                    Concepto = c.Concepto,
                    Monto = c.Monto
                };

                if (!string.IsNullOrWhiteSpace(c.ConceptosJson))
                {
                    try
                    {
                        var conceptos = JsonSerializer.Deserialize<List<ConceptoServicioVM>>(c.ConceptosJson);
                        if (conceptos != null) cierreVm.Conceptos = conceptos;
                    }
                    catch { /* ignorar error de JSON */ }
                }

                vm.Cierres.Add(cierreVm);
            }

            return vm;
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

            // Serializar snapshot de los conceptos
            string? conceptosJson = null;
            if (model.Conceptos != null && model.Conceptos.Any())
            {
                conceptosJson = JsonSerializer.Serialize(model.Conceptos);
            }

            var cierre = new CierreCaja
            {
                Fecha = hoy,
                TipoMovimiento = "Cierre Diario",
                Monto = ingresos,
                Concepto = $"Cierre de caja - Efectivo: {model.Efectivo}, Transferencia: {model.Transferencia}, " +
                           $"Gastos Efectivo: {model.GastoEfectivo}, Gastos Transf.: {model.GastoTransferencia}, " +
                           $"Total Facturado: {totalFacturado}, Diferencia: {diferencia}",
                Cedula = cedula,
                Efectivo = model.Efectivo,
                Transferencia = model.Transferencia,
                GastosEfectivo = model.GastoEfectivo,
                GastosTransferencia = model.GastoTransferencia,
                Diferencia = diferencia,
                ConceptosJson = conceptosJson
            };

            _dbContext.CierreCajas.Add(cierre);
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
        public async Task<List<ConceptoServicioVM>> ObtenerConceptosDelDiaAsync()
        {
            var inicio = DateTime.Today;
            var fin = inicio.AddDays(1);

            // NOTA: cambia los campos según tu modelo real (Fecha, CodigoProducto/IdProducto, etc.)
            var query = from ped in _dbContext.Pedidos
                        where ped.FechaRegistro >= inicio && ped.FechaRegistro < fin
                        join prod in _dbContext.Productos on ped.Codigo equals prod.Cod_Producto // o ped.IdProducto == prod.IdProducto
                        join cat in _dbContext.CategoriaProductos on prod.IdCatepro equals cat.IdCateProducto
                        join serv in _dbContext.Servicio on cat.IdServicio equals serv.IdServicio
                        group new { ped } by new { serv.IdServicio, serv.NombreServicio } into g
                        select new ConceptoServicioVM
                        {
                            IdServicio = g.Key.IdServicio,
                            NombreServicio = g.Key.NombreServicio,
                            TotalSubTotal = (decimal)g.Sum(x => x.ped.SubTotal),
                            TotalVNeto = (decimal)g.Sum(x => x.ped.VNeto)
                        };

            return await query
                .OrderBy(x => x.NombreServicio)
                .ToListAsync();
        }
    }
}
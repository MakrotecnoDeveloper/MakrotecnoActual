using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Models.Dto.Pedido;
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
        public async Task<(bool, string)> RegistrarCierreAsync(CierreCajaViewModel model, ContextoAccesoDto ctx)
        {
            var fechaTrabajo = model.Fecha.Date;
            var inicio = fechaTrabajo;
            var fin = inicio.AddDays(1);

            // 1) Validar si ya existe un cierre previo para esa cédula en esa fecha
            var cierreExistente = await ObtenerCierreExistenteDelDiaAsync(ctx.Cedula, fechaTrabajo);

            if (cierreExistente != null)
            {
                return (false,
                    $"Ya existe un cierre de caja registrado para la cédula {ctx.Cedula} en la fecha {fechaTrabajo:dd/MM/yyyy}. " +
                    $"Si requiere un nuevo cierre, debe solicitar autorización del supervisor con cargo 'Administrador Lider'.");
            }

            // 2) Traer facturas del contexto real: empleado + empresa + sede + PDV + fecha
            var idsFacturas = await (
                from f in _dbContext.Factura
                join v in _dbContext.Ventas on f.IdVenta equals v.IdVenta
                join p in _dbContext.Pedidos on v.IdVenta equals p.IdVenta
                join pdv in _dbContext.Infopdv on p.InfopdvId equals pdv.InfopdvId
                join s in _dbContext.Sede on pdv.Id_Sede equals s.Id_sede
                where f.FechaEmision >= inicio && f.FechaEmision < fin
                      && v.Cedula == ctx.Cedula
                      && p.InfopdvId == ctx.PdvId
                      && pdv.Id_Sede == ctx.SedeId
                      && s.Id_empresa == ctx.EmpresaId
                select f.IdFactura
            ).Distinct().ToListAsync();

            if (!idsFacturas.Any())
            {
                return (false,
                    "No se encontraron facturas de esa fecha realizadas por usted en este PDV.");
            }

            decimal totalFacturado = await _dbContext.Factura
                .Where(f => idsFacturas.Contains(f.IdFactura))
                .SumAsync(f => (decimal?)f.Total) ?? 0m;

            model.TotalFacturado = totalFacturado;

            decimal ingresos = model.Efectivo + model.Transferencia;
            decimal egresos = model.GastoEfectivo + model.GastoTransferencia;
            decimal diferencia = ingresos - egresos;

            string? conceptosJson = null;
            if (model.Conceptos != null && model.Conceptos.Any())
            {
                conceptosJson = JsonSerializer.Serialize(model.Conceptos);
            }

            var cierre = new CierreCaja
            {
                Fecha = fechaTrabajo,
                TipoMovimiento = "Cierre Diario",
                Monto = ingresos,
                Concepto = $"Cierre de caja - Fecha: {fechaTrabajo:yyyy-MM-dd}, " +
                           $"PDV: {ctx.PdvId}, Efectivo: {model.Efectivo}, Transferencia: {model.Transferencia}, " +
                           $"Gastos Efectivo: {model.GastoEfectivo}, Gastos Transf.: {model.GastoTransferencia}, " +
                           $"Total Facturado: {totalFacturado}, Diferencia: {diferencia}",
                Cedula = ctx.Cedula,
                Efectivo = model.Efectivo,
                Transferencia = model.Transferencia,
                GastosEfectivo = model.GastoEfectivo,
                GastosTransferencia = model.GastoTransferencia,
                Diferencia = diferencia,
                ConceptosJson = conceptosJson,

                // contexto histórico
                IdEmpresa = ctx.EmpresaId,
                IdSede = ctx.SedeId,
                InfopdvId = ctx.PdvId,
                NombreRol = ctx.NombreRol
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
        public async Task<decimal> ObtenerTotalFacturadoDelDiaAsync(
    ContextoAccesoDto ctx,
    DateTime? fecha = null)
        {
            var baseFecha = (fecha ?? DateTime.Today).Date;
            var inicio = baseFecha;
            var fin = inicio.AddDays(1);

            var query =
                from ped in _dbContext.Pedidos
                join v in _dbContext.Ventas on ped.IdVenta equals v.IdVenta
                join pdv in _dbContext.Infopdv on ped.InfopdvId equals pdv.InfopdvId
                join s in _dbContext.Sede on pdv.Id_Sede equals s.Id_sede
                where ped.FechaRegistro >= inicio && ped.FechaRegistro < fin
                select new
                {
                    Pedido = ped,
                    Venta = v,
                    Pdv = pdv,
                    Sede = s
                };

            if (!ctx.EsAdministradorLider)
            {
                query = query.Where(x =>
                    x.Venta.Cedula == ctx.Cedula &&
                    x.Pedido.InfopdvId == ctx.PdvId &&
                    x.Pdv.Id_Sede == ctx.SedeId &&
                    x.Sede.Id_empresa == ctx.EmpresaId);
            }

            return await query.SumAsync(x => (decimal?)x.Pedido.VNeto) ?? 0m;
        }
        public async Task<List<ConceptoServicioVM>> ObtenerConceptosDelDiaAsync(
    ContextoAccesoDto ctx,
    DateTime fecha)
        {
            var inicio = fecha.Date;
            var fin = inicio.AddDays(1);

            var query =
                from ped in _dbContext.Pedidos
                join v in _dbContext.Ventas on ped.IdVenta equals v.IdVenta
                join pdv in _dbContext.Infopdv on ped.InfopdvId equals pdv.InfopdvId
                join s in _dbContext.Sede on pdv.Id_Sede equals s.Id_sede
                join prod in _dbContext.Productos on ped.Codigo equals prod.Cod_Producto
                join cat in _dbContext.CategoriaProductos on prod.IdCatepro equals cat.IdCateProducto
                join serv in _dbContext.Servicio on cat.IdServicio equals serv.IdServicio
                where ped.FechaRegistro >= inicio && ped.FechaRegistro < fin
                      && v.Cedula == ctx.Cedula
                      && ped.InfopdvId == ctx.PdvId
                      && pdv.Id_Sede == ctx.SedeId
                      && s.Id_empresa == ctx.EmpresaId
                select new
                {
                    Pedido = ped,
                    Servicio = serv
                };

            return await query
                .GroupBy(x => new { x.Servicio.IdServicio, x.Servicio.NombreServicio })
                .Select(g => new ConceptoServicioVM
                {
                    IdServicio = g.Key.IdServicio,
                    NombreServicio = g.Key.NombreServicio,
                    TotalSubTotal = g.Sum(x => x.Pedido.SubTotal),
                    TotalVNeto = g.Sum(x => x.Pedido.VUnidad ?? 0m)
                })
                .OrderBy(x => x.NombreServicio)
                .ToListAsync();
        }
        public async Task<decimal> ObtenerTotalFacturadoAsync(ContextoAccesoDto ctx, DateTime fecha)
        {
            var inicio = fecha.Date;
            var fin = inicio.AddDays(1);

            var idsFacturas = await (
                from f in _dbContext.Factura
                join v in _dbContext.Ventas on f.IdVenta equals v.IdVenta
                join p in _dbContext.Pedidos on v.IdVenta equals p.IdVenta
                join pdv in _dbContext.Infopdv on p.InfopdvId equals pdv.InfopdvId
                join s in _dbContext.Sede on pdv.Id_Sede equals s.Id_sede
                where f.FechaEmision >= inicio && f.FechaEmision < fin
                      && v.Cedula == ctx.Cedula
                      && p.InfopdvId == ctx.PdvId
                      && pdv.Id_Sede == ctx.SedeId
                      && s.Id_empresa == ctx.EmpresaId
                select f.IdFactura
            ).Distinct().ToListAsync();

            if (!idsFacturas.Any())
                return 0m;

            return await _dbContext.Factura
                .Where(f => idsFacturas.Contains(f.IdFactura))
                .SumAsync(f => (decimal?)f.Total) ?? 0m;
        }
        public async Task<CierreCaja?> ObtenerCierreExistenteDelDiaAsync(int cedula, DateTime fecha)
        {
            var inicio = fecha.Date;
            var fin = inicio.AddDays(1);

            return await _dbContext.CierreCajas
                .Where(c => c.Cedula == cedula
                         && c.Fecha >= inicio
                         && c.Fecha < fin
                         && (c.TipoMovimiento == "Cierre Diario" || c.TipoMovimiento == "Cierre Reautorizado"))
                .OrderByDescending(c => c.Fecha)
                .FirstOrDefaultAsync();
        }
    }
}
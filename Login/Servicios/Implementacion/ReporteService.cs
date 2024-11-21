using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Servicios.Implementacion
{
    public class ReporteService : IReporteService
    {
        //variable de solo lectura para referenciar la base de datos
        private readonly BaseAdmContext _dbContext;
        public ReporteService(BaseAdmContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<ReporteItem>> GenerarReporte(DateTime fechaInicio, DateTime fechaFin, string tipoReporte)
        {
            IQueryable<ReporteItem> reporte = tipoReporte switch
            {
                "ventaTotal" => (IQueryable<ReporteItem>)(from f in _dbContext.Factura
                                                          join v in _dbContext.Ventas on f.cod_factura equals v.cod_factura
                                                          where f.fechaVenta >= fechaInicio && f.fechaVenta <= fechaFin
                                                          select new ReporteItem
                                                          {
                                                              FechaFactura = f.fechaVenta,
                                                              TotalVentas = v.ventaTotal
                                                          }),
                "ventaMakrotecno" => (IQueryable<ReporteItem>)(from f in _dbContext.Factura
                                                               join v in _dbContext.Ventas on f.cod_factura equals v.cod_factura
                                                               where f.fechaVenta >= fechaInicio && f.fechaVenta <= fechaFin
                                                               select new ReporteItem
                                                               {
                                                                   FechaFactura = f.fechaVenta,
                                                                   TotalMakrotecno = v.ventaMakrotecno
                                                               }),
                "netoMakrotecno" => (IQueryable<ReporteItem>)(from f in _dbContext.Factura
                                                              join v in _dbContext.Ventas on f.cod_factura equals v.cod_factura
                                                              where f.fechaVenta >= fechaInicio && f.fechaVenta <= fechaFin
                                                              select new ReporteItem
                                                              {
                                                                  FechaFactura = f.fechaVenta,
                                                                  TotalNetoMakrotecno = v.netoMakrotecno
                                                              }),
                "ventaRecargas" => (IQueryable<ReporteItem>)(from f in _dbContext.Factura
                                                             join v in _dbContext.Ventas on f.cod_factura equals v.cod_factura
                                                             where f.fechaVenta >= fechaInicio && f.fechaVenta <= fechaFin
                                                             select new ReporteItem
                                                             {
                                                                 FechaFactura = f.fechaVenta,
                                                                 TotalRecargas = v.ventaRecargas
                                                             }),
                "ventaTienda" => (IQueryable<ReporteItem>)(from f in _dbContext.Factura
                                                           join v in _dbContext.Ventas on f.cod_factura equals v.cod_factura
                                                           where f.fechaVenta >= fechaInicio && f.fechaVenta <= fechaFin
                                                           select new ReporteItem
                                                           {
                                                               FechaFactura = f.fechaVenta,
                                                               TotalTienda = v.ventaTienda
                                                           }),
                "gananciaMakrotecno" => (IQueryable<ReporteItem>)(from f in _dbContext.Factura
                                                                  join v in _dbContext.Ventas on f.cod_factura equals v.cod_factura
                                                                  join g in _dbContext.Ganancias on v.id_venta equals g.id_venta
                                                                  where f.fechaVenta >= fechaInicio && f.fechaVenta <= fechaFin
                                                                  group new { f, v, g } by f.fechaVenta into grp
                                                                  select new ReporteItem
                                                                  {
                                                                      FechaFactura = grp.Key,
                                                                      TotalGananciaMakrotecno = grp.Sum(x => x.g.gananciaMakrotecno)
                                                                  }),
                "gananciaTotal" => (IQueryable<ReporteItem>)(from f in _dbContext.Factura
                                                             join v in _dbContext.Ventas on f.cod_factura equals v.cod_factura
                                                             join g in _dbContext.Ganancias on v.id_venta equals g.id_venta
                                                             where f.fechaVenta >= fechaInicio && f.fechaVenta <= fechaFin
                                                             group new { f, v, g } by f.fechaVenta into grp
                                                             select new ReporteItem
                                                             {
                                                                 FechaFactura = grp.Key,
                                                                 TotalGananciaTotal = grp.Sum(x => x.g.gananciaTotal)
                                                             }),
                "gananciaMaria" => (IQueryable<ReporteItem>)(from f in _dbContext.Factura
                                                             join v in _dbContext.Ventas on f.cod_factura equals v.cod_factura
                                                             join g in _dbContext.Ganancias on v.id_venta equals g.id_venta
                                                             where f.fechaVenta >= fechaInicio && f.fechaVenta <= fechaFin
                                                             group new { f, v, g } by f.fechaVenta into grp
                                                             select new ReporteItem
                                                             {
                                                                 FechaFactura = grp.Key,
                                                                 TotalGananciaMaria = grp.Sum(x => x.g.gananciaMaria)
                                                             }),
                "gananciaVictor" => (IQueryable<ReporteItem>)(from f in _dbContext.Factura
                                                              join v in _dbContext.Ventas on f.cod_factura equals v.cod_factura
                                                              join g in _dbContext.Ganancias on v.id_venta equals g.id_venta
                                                              where f.fechaVenta >= fechaInicio && f.fechaVenta <= fechaFin
                                                              group new { f, v, g } by f.fechaVenta into grp
                                                              select new ReporteItem
                                                              {
                                                                  FechaFactura = grp.Key,
                                                                  TotalGananciaVictor = grp.Sum(x => x.g.gananciaVictor)
                                                              }),
                "gananciaTeresa" => (IQueryable<ReporteItem>)(from f in _dbContext.Factura
                                                              join v in _dbContext.Ventas on f.cod_factura equals v.cod_factura
                                                              join g in _dbContext.Ganancias on v.id_venta equals g.id_venta
                                                              where f.fechaVenta >= fechaInicio && f.fechaVenta <= fechaFin
                                                              group new { f, v, g } by f.fechaVenta into grp
                                                              select new ReporteItem
                                                              {
                                                                  FechaFactura = grp.Key,
                                                                  TotalGananciaTeresa = grp.Sum(x => x.g.gananciaTeresa)
                                                              }),
                _ => throw new ArgumentException("Tipo de reporte no válido"),
            };

            // Ejecutar la consulta y obtener los resultados
            var resultados = await reporte.ToListAsync();

            return resultados;
        }
    }
}

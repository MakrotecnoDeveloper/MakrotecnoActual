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
                "ventaTotal" => (IQueryable<ReporteItem>)(from v in _dbContext.Ventas
                                                          where v.fechaVenta >= fechaInicio && v.fechaVenta <= fechaFin
                                                          select new ReporteItem
                                                          {
                                                              FechaFactura = v.fechaVenta,
                                                              TotalVentas = v.ventaTotal
                                                          }),
                "ventaMakrotecno" => (IQueryable<ReporteItem>)(from v in _dbContext.Ventas
                                                               where v.fechaVenta >= fechaInicio && v.fechaVenta <= fechaFin
                                                               select new ReporteItem
                                                               {
                                                                   FechaFactura = v.fechaVenta,
                                                                   TotalMakrotecno = v.ventaMakrotecno
                                                               }),
                "netoMakrotecno" => (IQueryable<ReporteItem>)(from v in _dbContext.Ventas
                                                              where v.fechaVenta >= fechaInicio && v.fechaVenta <= fechaFin
                                                              select new ReporteItem
                                                              {
                                                                  FechaFactura = v.fechaVenta,
                                                                  TotalNetoMakrotecno = v.netoMakrotecno
                                                              }),
                "ventaRecargas" => (IQueryable<ReporteItem>)(from v in _dbContext.Ventas
                                                             where v.fechaVenta >= fechaInicio && v.fechaVenta <= fechaFin
                                                             select new ReporteItem
                                                             {
                                                                 FechaFactura = v.fechaVenta,
                                                                 TotalRecargas = v.ventaRecargas
                                                             }),
                "ventaTienda" => (IQueryable<ReporteItem>)(from v in _dbContext.Ventas
                                                           where v.fechaVenta >= fechaInicio && v.fechaVenta <= fechaFin
                                                           select new ReporteItem
                                                           {
                                                               FechaFactura = v.fechaVenta,
                                                               TotalTienda = v.ventaTienda
                                                           }),
                "gananciaMakrotecno" => (IQueryable<ReporteItem>)(from g in _dbContext.Ganancias
                                                                  where g.fechaGanancia >= fechaInicio && g.fechaGanancia <= fechaFin
                                                                  group g by g.fechaGanancia into grouped
                                                                  select new ReporteItem
                                                                  {
                                                                      FechaFactura = grouped.Key,
                                                                      TotalGananciaMakrotecno = grouped.Sum(g => g.gananciaMakrotecno)
                                                                  }),
                "gananciaTotal" => (IQueryable<ReporteItem>)(from g in _dbContext.Ganancias
                                                             where g.fechaGanancia >= fechaInicio && g.fechaGanancia <= fechaFin
                                                             group g by g.fechaGanancia into grouped
                                                             select new ReporteItem
                                                             {
                                                                 FechaFactura = grouped.Key,
                                                                 TotalGananciaTotal = grouped.Sum(g => g.gananciaTotal)
                                                             }),
                "gananciaMaria" => (IQueryable<ReporteItem>)(from g in _dbContext.Ganancias
                                                             where g.fechaGanancia >= fechaInicio && g.fechaGanancia <= fechaFin
                                                             group g by g.fechaGanancia into grouped
                                                             select new ReporteItem
                                                             {
                                                                 FechaFactura = grouped.Key,
                                                                 TotalGananciaMaria = grouped.Sum(g => g.gananciaMaria)
                                                             }),
                "gananciaVictor" => (IQueryable<ReporteItem>)(from g in _dbContext.Ganancias
                                                              where g.fechaGanancia >= fechaInicio && g.fechaGanancia <= fechaFin
                                                              group g by g.fechaGanancia into grouped
                                                              select new ReporteItem
                                                              {
                                                                  FechaFactura = grouped.Key,
                                                                  TotalGananciaVictor = grouped.Sum(g => g.gananciaVictor)
                                                              }),
                "gananciaTeresa" => (IQueryable<ReporteItem>)(from g in _dbContext.Ganancias
                                                              where g.fechaGanancia >= fechaInicio && g.fechaGanancia <= fechaFin
                                                              group g by g.fechaGanancia into grouped
                                                              select new ReporteItem
                                                              {
                                                                  FechaFactura = grouped.Key,
                                                                  TotalGananciaTeresa = grouped.Sum(g => g.gananciaTeresa)
                                                              }),
                _ => throw new ArgumentException("Tipo de reporte no válido"),
            };

            // Ejecutar la consulta y obtener los resultados
            var resultados = await reporte.ToListAsync();

            return resultados;
        }
    }
}

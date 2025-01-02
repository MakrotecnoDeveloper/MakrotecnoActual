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
                                                          where v.FechaVenta >= fechaInicio && v.FechaVenta <= fechaFin
                                                          select new ReporteItem
                                                          {
                                                              FechaFactura = v.FechaVenta,
                                                              TotalVentas = v.VentaTotal
                                                          }),
                "ventaMakrotecno" => (IQueryable<ReporteItem>)(from v in _dbContext.Ventas
                                                               where v.FechaVenta >= fechaInicio && v.FechaVenta <= fechaFin
                                                               select new ReporteItem
                                                               {
                                                                   FechaFactura = v.FechaVenta,
                                                                   TotalMakrotecno = v.VentaMakrotecno
                                                               }),
                "netoMakrotecno" => (IQueryable<ReporteItem>)(from v in _dbContext.Ventas
                                                              where v.FechaVenta >= fechaInicio && v.FechaVenta <= fechaFin
                                                              select new ReporteItem
                                                              {
                                                                  FechaFactura = v.FechaVenta,
                                                                  TotalNetoMakrotecno = v.NetoMakrotecno
                                                              }),
                "ventaRecargas" => (IQueryable<ReporteItem>)(from v in _dbContext.Ventas
                                                             where v.FechaVenta >= fechaInicio && v.FechaVenta <= fechaFin
                                                             select new ReporteItem
                                                             {
                                                                 FechaFactura = v.FechaVenta,
                                                                 TotalRecargas = v.VentaRecargas
                                                             }),
                "ventaTienda" => (IQueryable<ReporteItem>)(from v in _dbContext.Ventas
                                                           where v.FechaVenta >= fechaInicio && v.FechaVenta <= fechaFin
                                                           select new ReporteItem
                                                           {
                                                               FechaFactura = v.FechaVenta,
                                                               TotalTienda = v.VentaTienda
                                                           }),
                "gananciaMakrotecno" => (IQueryable<ReporteItem>)(from g in _dbContext.Ganancias
                                                                  where g.FechaGanancia >= fechaInicio && g.FechaGanancia <= fechaFin
                                                                  group g by g.FechaGanancia into grouped
                                                                  select new ReporteItem
                                                                  {
                                                                      FechaFactura = grouped.Key,
                                                                      TotalGananciaMakrotecno = grouped.Sum(g => g.GananciaMakrotecno)
                                                                  }),
                "gananciaTotal" => (IQueryable<ReporteItem>)(from g in _dbContext.Ganancias
                                                             where g.FechaGanancia >= fechaInicio && g.FechaGanancia <= fechaFin
                                                             group g by g.FechaGanancia into grouped
                                                             select new ReporteItem
                                                             {
                                                                 FechaFactura = grouped.Key,
                                                                 TotalGananciaTotal = grouped.Sum(g => g.GananciaTotal)
                                                             }),
                "gananciaMaria" => (IQueryable<ReporteItem>)(from g in _dbContext.Ganancias
                                                             where g.FechaGanancia >= fechaInicio && g.FechaGanancia <= fechaFin
                                                             group g by g.FechaGanancia into grouped
                                                             select new ReporteItem
                                                             {
                                                                 FechaFactura = grouped.Key,
                                                                 TotalGananciaMaria = grouped.Sum(g => g.GananciaMaria)
                                                             }),
                "gananciaVictor" => (IQueryable<ReporteItem>)(from g in _dbContext.Ganancias
                                                              where g.FechaGanancia >= fechaInicio && g.FechaGanancia <= fechaFin
                                                              group g by g.FechaGanancia into grouped
                                                              select new ReporteItem
                                                              {
                                                                  FechaFactura = grouped.Key,
                                                                  TotalGananciaVictor = grouped.Sum(g => g.GananciaVictor)
                                                              }),
                "gananciaTeresa" => (IQueryable<ReporteItem>)(from g in _dbContext.Ganancias
                                                              where g.FechaGanancia >= fechaInicio && g.FechaGanancia <= fechaFin
                                                              group g by g.FechaGanancia into grouped
                                                              select new ReporteItem
                                                              {
                                                                  FechaFactura = grouped.Key,
                                                                  TotalGananciaTeresa = grouped.Sum(g => g.GananciaTeresa)
                                                              }),
                _ => throw new ArgumentException("Tipo de reporte no válido"),
            };

            // Ejecutar la consulta y obtener los resultados
            var resultados = await reporte.ToListAsync();

            return resultados;
        }
    }
}

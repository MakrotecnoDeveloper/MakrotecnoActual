using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Servicios.Implementacion
{
    public class ReporteService : IReporteService
    {
        private readonly BaseAdmContext _dbContext;
        public ReporteService(BaseAdmContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ReporteFinancieroViewModel> GenerarReporteDelDiaAsync(DateTime fecha)
        {
            var movimientos = await _dbContext.CierreCajas.Where(m => m.Fecha.Date == fecha.Date).ToListAsync();

            var ingresos = movimientos.Where(m => m.TipoMovimiento == "Ingreso").Sum(m => m.Monto);
            var costos = movimientos.Where(m => m.TipoMovimiento == "Egreso").Sum(m => m.Monto);
            var utilidad = ingresos - costos;

            return new ReporteFinancieroViewModel
            {
                Fecha = fecha,
                Ingresos = ingresos,
                Costos = costos,
                Utilidad = utilidad,
                BalanceCaja = ingresos - costos,
                Movimientos = movimientos
            };
        }
        public List<ConceptoServicioVM> TraerServiciosUnicos(DateTime fechaInicio, DateTime fechaFin)
        {
            var cierres = _dbContext.CierreCajas
                .Where(f => f.TipoMovimiento == "Cierre Diario"
                         && f.Fecha.Date >= fechaInicio.Date
                         && f.Fecha.Date <= fechaFin.Date)
                .ToList();

            var conceptos = new List<ConceptoServicioVM>();

            foreach (var cierre in cierres)
            {
                var lista = JsonConvert.DeserializeObject<List<ConceptoServicioVM>>(cierre.ConceptosJson) ?? new();
                conceptos.AddRange(lista);
            }

            // 🔑 Agrupamos servicios únicos
            return conceptos
                .GroupBy(c => new { c.IdServicio, c.NombreServicio })
                .Select(g => new ConceptoServicioVM
                {
                    IdServicio = g.Key.IdServicio,
                    NombreServicio = g.Key.NombreServicio
                })
                .ToList();
        }
        public (decimal totalSubTotal, decimal totalVNeto) TraerTotalesPorRangoYServicio(DateTime fechaInicio, DateTime fechaFin, int idServicio)
        {
            var cierres = _dbContext.CierreCajas
                .Where(f => f.TipoMovimiento == "Cierre Diario"
                         && f.Fecha.Date >= fechaInicio.Date
                         && f.Fecha.Date <= fechaFin.Date)
                .ToList();

            decimal totalSub = 0, totalNeto = 0;

            foreach (var cierre in cierres)
            {
                var conceptos = JsonConvert.DeserializeObject<List<ConceptoServicioVM>>(cierre.ConceptosJson) ?? new();

                var filtro = conceptos.Where(c => c.IdServicio == idServicio);

                totalSub += filtro.Sum(c => c.TotalSubTotal);
                totalNeto += filtro.Sum(c => c.TotalVNeto);
            }

            return (totalSub, totalNeto);
        }
        public List<ConceptoServicioVM> TraerUtilidadPorServicios(DateTime fechaInicio, DateTime fechaFin)
        {
            var cierres = _dbContext.CierreCajas
                .Where(f => f.TipoMovimiento == "Cierre Diario"
                         && f.Fecha >= fechaInicio
                         && f.Fecha <= fechaFin)
                .Select(f => f.ConceptosJson)
                .ToList();

            var listaServicios = new List<ConceptoServicioVM>();

            foreach (var json in cierres)
            {
                if (!string.IsNullOrEmpty(json))
                {
                    var conceptos = System.Text.Json.JsonSerializer
                        .Deserialize<List<ConceptoServicioVM>>(json);

                    if (conceptos != null)
                        listaServicios.AddRange(conceptos);
                }
            }

            // Agrupar por servicio y calcular utilidad
            var resultado = listaServicios
                .GroupBy(c => new { c.IdServicio, c.NombreServicio })
                .Select(g => new ConceptoServicioVM
                {
                    IdServicio = g.Key.IdServicio,
                    NombreServicio = g.Key.NombreServicio,
                    TotalSubTotal = g.Sum(x => x.TotalSubTotal),
                    TotalVNeto = g.Sum(x => x.TotalVNeto)
                })
                .ToList();

            return resultado;
        }


    }
}
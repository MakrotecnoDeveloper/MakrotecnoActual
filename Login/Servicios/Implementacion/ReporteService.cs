using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using Plataforma.ViewModels.Reportes;
using System.Text.Json;

namespace Plataforma.Servicios.Implementacion
{
    public class ReporteService : IReporteService
    {
        private readonly BaseAdmContext _context;
        public ReporteService(BaseAdmContext dbContext)
        {
            _context = dbContext;
        }
        public async Task<ReporteGeneralVm> ObtenerVistaGeneralAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var desde = fechaInicio.Date;
            var hasta = fechaFin.Date.AddDays(1);

            var ventasPorServicio = await ObtenerVentasPorServicioAsync(desde, hasta);
            var gananciasPorPersona = await ObtenerGananciasPorPersonaAsync(desde, hasta);

            return new ReporteGeneralVm
            {
                Filtro = new FiltroReporteVm
                {
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin
                },
                VentasPorServicio = ventasPorServicio,
                GananciasPorPersona = gananciasPorPersona
            };
        }

        public async Task<byte[]> ExportarVistaGeneralExcelAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var model = await ObtenerVistaGeneralAsync(fechaInicio, fechaFin);

            using var workbook = new XLWorkbook();

            var wsResumen = workbook.Worksheets.Add("Resumen");
            wsResumen.Cell(1, 1).Value = "Fecha inicio";
            wsResumen.Cell(1, 2).Value = model.Filtro.FechaInicio.ToString("yyyy-MM-dd");
            wsResumen.Cell(2, 1).Value = "Fecha fin";
            wsResumen.Cell(2, 2).Value = model.Filtro.FechaFin.ToString("yyyy-MM-dd");
            wsResumen.Cell(4, 1).Value = "Total VNeto";
            wsResumen.Cell(4, 2).Value = model.TotalVNeto;
            wsResumen.Cell(5, 1).Value = "Total SubTotal";
            wsResumen.Cell(5, 2).Value = model.TotalSubTotal;
            wsResumen.Cell(6, 1).Value = "Total Utilidad";
            wsResumen.Cell(6, 2).Value = model.TotalUtilidad;
            wsResumen.Cell(7, 1).Value = "Total Participaciones";
            wsResumen.Cell(7, 2).Value = model.TotalParticipaciones;

            var wsVentas = workbook.Worksheets.Add("VentasPorServicio");
            wsVentas.Cell(1, 1).Value = "Servicio";
            wsVentas.Cell(1, 2).Value = "VNeto";
            wsVentas.Cell(1, 3).Value = "SubTotal";
            wsVentas.Cell(1, 4).Value = "Utilidad";
            wsVentas.Cell(1, 5).Value = "CantidadCierres";

            var filaVentas = 2;
            foreach (var item in model.VentasPorServicio)
            {
                wsVentas.Cell(filaVentas, 1).Value = item.Servicio;
                wsVentas.Cell(filaVentas, 2).Value = item.TotalVNeto;
                wsVentas.Cell(filaVentas, 3).Value = item.TotalSubTotal;
                wsVentas.Cell(filaVentas, 4).Value = item.TotalUtilidad;
                wsVentas.Cell(filaVentas, 5).Value = item.CantidadCierres;
                filaVentas++;
            }

            var wsGanancias = workbook.Worksheets.Add("GananciasPorPersona");
            wsGanancias.Cell(1, 1).Value = "Cedula";
            wsGanancias.Cell(1, 2).Value = "Empleado";
            wsGanancias.Cell(1, 3).Value = "Servicio";
            wsGanancias.Cell(1, 4).Value = "Concepto";
            wsGanancias.Cell(1, 5).Value = "BaseUtilidad";
            wsGanancias.Cell(1, 6).Value = "PorcentajePromedio";
            wsGanancias.Cell(1, 7).Value = "ValorGanado";
            wsGanancias.Cell(1, 8).Value = "CantidadNovedades";

            var filaGanancias = 2;
            foreach (var item in model.GananciasPorPersona)
            {
                wsGanancias.Cell(filaGanancias, 1).Value = item.Cedula;
                wsGanancias.Cell(filaGanancias, 2).Value = item.Empleado;
                wsGanancias.Cell(filaGanancias, 3).Value = item.Servicio;
                wsGanancias.Cell(filaGanancias, 4).Value = item.Concepto;
                wsGanancias.Cell(filaGanancias, 5).Value = item.BaseUtilidad;
                wsGanancias.Cell(filaGanancias, 6).Value = item.PorcentajePromedio;
                wsGanancias.Cell(filaGanancias, 7).Value = item.ValorGanado;
                wsGanancias.Cell(filaGanancias, 8).Value = item.CantidadNovedades;
                filaGanancias++;
            }

            foreach (var ws in workbook.Worksheets)
            {
                ws.Columns().AdjustToContents();
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private async Task<List<ReporteVentaServicioVm>> ObtenerVentasPorServicioAsync(DateTime desde, DateTime hasta)
        {
            var cierres = await _context.CierreCajas
                .Where(x =>
                    x.Fecha >= desde &&
                    x.Fecha < hasta &&
                    !string.IsNullOrWhiteSpace(x.ConceptosJson))
                .OrderBy(x => x.Fecha)
                .ToListAsync();

            var servicios = await _context.Servicio.ToListAsync();

            var servicioPorNombre = servicios
                .Where(x => !string.IsNullOrWhiteSpace(x.NombreServicio))
                .GroupBy(x => NormalizarTexto(x.NombreServicio!))
                .ToDictionary(g => g.Key, g => g.First());

            var items = new List<ItemVentaServicioTemporal>();

            foreach (var cierre in cierres)
            {
                var detalles = ParsearConceptosJson(cierre.ConceptosJson!);
                var servicioPorId = servicios.ToDictionary(x => x.IdServicio, x => x);
                foreach (var item in detalles)
                {
                    var servicioEncontrado = ResolverServicio(item, servicioPorNombre, servicioPorId);
                    if (servicioEncontrado == null)
                        continue;

                    var vNeto = item.VNeto ?? item.TotalVNeto ?? item.MontoVNeto ?? 0m;
                    var subTotal = item.SubTotal ?? item.TotalSubTotal ?? item.MontoSubTotal ?? 0m;
                    var utilidad = item.Utilidad ?? (subTotal - vNeto);

                    items.Add(new ItemVentaServicioTemporal
                    {
                        IdServicio = servicioEncontrado.IdServicio,
                        Servicio = servicioEncontrado.NombreServicio ?? "Sin nombre",
                        VNeto = vNeto,
                        SubTotal = subTotal,
                        Utilidad = utilidad,
                        IdFlujoCaja = cierre.IdFlujoCaja
                    });
                }
            }

            return items
                .GroupBy(x => new { x.IdServicio, x.Servicio })
                .Select(g => new ReporteVentaServicioVm
                {
                    IdServicio = g.Key.IdServicio,
                    Servicio = g.Key.Servicio,
                    TotalVNeto = g.Sum(x => x.VNeto),
                    TotalSubTotal = g.Sum(x => x.SubTotal),
                    TotalUtilidad = g.Sum(x => x.Utilidad),
                    CantidadCierres = g.Select(x => x.IdFlujoCaja).Distinct().Count()
                })
                .OrderByDescending(x => x.TotalUtilidad)
                .ToList();
        }

        private async Task<List<ReporteGananciaPersonaVm>> ObtenerGananciasPorPersonaAsync(DateTime desde, DateTime hasta)
        {
            var novedades = await _context.NovedadesNomina
                .Include(x => x.ConceptoNomina)
                    .ThenInclude(c => c.Servicio)
                .Where(x =>
                    x.FechaNovedad >= desde &&
                    x.FechaNovedad < hasta &&
                    x.Origen == "CierreCaja" &&
                    x.Estado != "Anulada")
                .ToListAsync();

            var empleados = await _context.Empleado
                .ToDictionaryAsync(x => x.Cedula, x => ((x.Nombre ?? "") + " " + (x.Apellido ?? "")).Trim());

            return novedades
                .GroupBy(x => new
                {
                    x.Cedula,
                    Servicio = x.ConceptoNomina.Servicio != null ? x.ConceptoNomina.Servicio.NombreServicio : "Sin servicio",
                    x.ConceptoNomina.Nombre
                })
                .Select(g => new ReporteGananciaPersonaVm
                {
                    Cedula = g.Key.Cedula,
                    Empleado = empleados.ContainsKey(g.Key.Cedula) ? empleados[g.Key.Cedula] : g.Key.Cedula.ToString(),
                    Servicio = g.Key.Servicio ?? "Sin servicio",
                    Concepto = g.Key.Nombre,
                    BaseUtilidad = g.Sum(x => x.BaseValor ?? 0m),
                    PorcentajePromedio = g.Any() ? g.Average(x => x.PorcentajeAplicado ?? 0m) : 0m,
                    ValorGanado = g.Sum(x => x.Valor),
                    CantidadNovedades = g.Count()
                })
                .OrderByDescending(x => x.ValorGanado)
                .ToList();
        }

        private static string NormalizarTexto(string texto)
        {
            return texto.Trim().ToUpperInvariant();
        }

        private Servicio? ResolverServicio(
    CierreCajaConceptoJsonItem item,
    Dictionary<string, Servicio> servicioPorNombre,
    Dictionary<int, Servicio> servicioPorId)
        {
            if (item.IdServicio.HasValue && item.IdServicio.Value > 0)
            {
                return servicioPorId.TryGetValue(item.IdServicio.Value, out var srv) ? srv : null;
            }

            var nombreServicio = item.Servicio ?? item.NombreServicio ?? item.Concepto;
            if (string.IsNullOrWhiteSpace(nombreServicio))
                return null;

            var key = NormalizarTexto(nombreServicio);
            return servicioPorNombre.TryGetValue(key, out var servicio) ? servicio : null;
        }

        private List<CierreCajaConceptoJsonItem> ParsearConceptosJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new List<CierreCajaConceptoJsonItem>();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            try
            {
                using var doc = JsonDocument.Parse(json);

                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    return System.Text.Json.JsonSerializer.Deserialize<List<CierreCajaConceptoJsonItem>>(json, options)
                           ?? new List<CierreCajaConceptoJsonItem>();
                }

                if (doc.RootElement.ValueKind == JsonValueKind.Object)
                {
                    var root = doc.RootElement;

                    foreach (var prop in new[] { "servicios", "conceptos", "items", "detalle", "detalleServicios" })
                    {
                        if (root.TryGetProperty(prop, out var arr) && arr.ValueKind == JsonValueKind.Array)
                        {
                            return System.Text.Json.JsonSerializer.Deserialize<List<CierreCajaConceptoJsonItem>>(arr.GetRawText(), options)
                                   ?? new List<CierreCajaConceptoJsonItem>();
                        }
                    }
                }
            }
            catch
            {
            }

            return new List<CierreCajaConceptoJsonItem>();
        }

        private class ItemVentaServicioTemporal
        {
            public int IdServicio { get; set; }
            public string Servicio { get; set; } = string.Empty;
            public decimal VNeto { get; set; }
            public decimal SubTotal { get; set; }
            public decimal Utilidad { get; set; }
            public int IdFlujoCaja { get; set; }
        }

        private class CierreCajaConceptoJsonItem
        {
            public int? IdServicio { get; set; }
            public string? Servicio { get; set; }
            public string? NombreServicio { get; set; }
            public string? Concepto { get; set; }

            public decimal? VNeto { get; set; }
            public decimal? TotalVNeto { get; set; }
            public decimal? MontoVNeto { get; set; }

            public decimal? SubTotal { get; set; }
            public decimal? TotalSubTotal { get; set; }
            public decimal? MontoSubTotal { get; set; }

            public decimal? Utilidad { get; set; }
        }
    }
}
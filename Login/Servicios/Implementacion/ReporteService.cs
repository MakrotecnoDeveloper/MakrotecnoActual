using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Plataforma.Data;
using Plataforma.Models;
using Plataforma.Models.ViewModels.Reportes;
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
        public async Task<ReporteVentasViewModel> ObtenerReporteVentasAsync(
            ReporteVentasFiltroViewModel filtros,
            int pagina,
            int registrosPorPagina)
        {
            if (pagina < 1)
                pagina = 1;

            if (registrosPorPagina <= 0)
                registrosPorPagina = 20;

            var query =
                from venta in _context.Ventas.AsNoTracking()
                join factura in _context.Factura.AsNoTracking()
                    on venta.IdVenta equals factura.IdVenta into facturaJoin
                from factura in facturaJoin.DefaultIfEmpty()
                select new
                {
                    Venta = venta,
                    Factura = factura
                };

            if (filtros.FiltrarFecha)
            {
                if (filtros.FechaInicial.HasValue)
                {
                    var fechaInicial = filtros.FechaInicial.Value.Date;
                    query = query.Where(x => x.Venta.FechaVenta.Date >= fechaInicial);
                }

                if (filtros.FechaFinal.HasValue)
                {
                    var fechaFinal = filtros.FechaFinal.Value.Date;
                    query = query.Where(x => x.Venta.FechaVenta.Date <= fechaFinal);
                }
            }

            if (filtros.FiltrarVendedor && filtros.CedulaVendedor.HasValue)
            {
                query = query.Where(x => x.Venta.Cedula == filtros.CedulaVendedor.Value);
            }

            if (filtros.FiltrarMetodoPago && !string.IsNullOrWhiteSpace(filtros.MetodoPago))
            {
                query = query.Where(x => x.Venta.MetodoPago == filtros.MetodoPago);
            }

            if (filtros.FiltrarEstadoVenta && !string.IsNullOrWhiteSpace(filtros.EstadoVenta))
            {
                query = query.Where(x => x.Venta.EstadoVenta == filtros.EstadoVenta);
            }

            if (filtros.FiltrarEstadoFactura && !string.IsNullOrWhiteSpace(filtros.EstadoFactura))
            {
                query = query.Where(x =>
                    x.Factura != null &&
                    x.Factura.EstadoFactura == filtros.EstadoFactura
                );
            }

            if (filtros.FiltrarCliente && !string.IsNullOrWhiteSpace(filtros.Cliente))
            {
                var cliente = filtros.Cliente.Trim();

                query = query.Where(x =>
                    x.Venta.IdCliente.ToString().Contains(cliente) ||
                    x.Venta.CedulaCliente.ToString().Contains(cliente)
                );
            }

            if (filtros.FiltrarFactura && !string.IsNullOrWhiteSpace(filtros.NumeroFactura))
            {
                var numeroFactura = filtros.NumeroFactura.Trim();

                query = query.Where(x =>
                    (
                        x.Factura != null &&
                        x.Factura.NumeroFactura.Contains(numeroFactura)
                    )
                    ||
                    (
                        x.Venta.NumeroFactura != null &&
                        x.Venta.NumeroFactura.Contains(numeroFactura)
                    )
                );
            }

            /*
                IMPORTANTE:
                Ventas actualmente no tiene IdSede ni InfopdvId.
                Por eso, mientras no estén en Ventas, se cruza con CierreCaja
                por Cedula + Fecha.

                Esto funciona como solución temporal, pero el dato no es perfecto
                si un vendedor trabaja en más de una sede o PDV el mismo día.
            */

            if (filtros.FiltrarSede && filtros.IdSede.HasValue)
            {
                query = query.Where(x =>
                    _context.CierreCajas.Any(c =>
                        c.Cedula == x.Venta.Cedula &&
                        c.Fecha.Date == x.Venta.FechaVenta.Date &&
                        c.IdSede == filtros.IdSede.Value
                    )
                );
            }

            if (filtros.FiltrarPdv && filtros.InfopdvId.HasValue)
            {
                query = query.Where(x =>
                    _context.CierreCajas.Any(c =>
                        c.Cedula == x.Venta.Cedula &&
                        c.Fecha.Date == x.Venta.FechaVenta.Date &&
                        c.InfopdvId == filtros.InfopdvId.Value
                    )
                );
            }

            var totalRegistros = await query.CountAsync();

            var totalGeneralFiltrado = await query
                .SumAsync(x => (decimal?)x.Venta.Total) ?? 0;

            var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)registrosPorPagina);

            var datosBase = await query
                .OrderByDescending(x => x.Venta.FechaVenta)
                .ThenByDescending(x => x.Venta.IdVenta)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .Select(x => new
                {
                    x.Venta.IdVenta,
                    x.Venta.IdCliente,
                    x.Venta.CedulaCliente,
                    x.Venta.Cedula,
                    x.Venta.FechaVenta,
                    x.Venta.MetodoPago,
                    x.Venta.Total,
                    x.Venta.EstadoVenta,
                    x.Venta.TipoVenta,
                    x.Venta.Conceptos,

                    NumeroFacturaVenta = x.Venta.NumeroFactura,
                    FechaEmisionFacturaVenta = x.Venta.FechaEmisionFactura,

                    IdFactura = x.Factura != null ? (int?)x.Factura.IdFactura : null,
                    NumeroFacturaReal = x.Factura != null ? x.Factura.NumeroFactura : null,
                    FechaEmisionFacturaReal = x.Factura != null ? (DateTime?)x.Factura.FechaEmision : null,
                    TotalFactura = x.Factura != null ? (decimal?)x.Factura.Total : null,
                    EstadoFactura = x.Factura != null ? x.Factura.EstadoFactura : null
                })
                .ToListAsync();

            var cedulas = datosBase
                .Select(x => x.Cedula)
                .Distinct()
                .ToList();

            var fechas = datosBase
                .Select(x => x.FechaVenta.Date)
                .Distinct()
                .ToList();

            var empleados = await _context.Empleado
                .AsNoTracking()
                .Where(e => cedulas.Contains(e.Cedula))
                .ToListAsync();

            var cierres = await _context.CierreCajas
                .AsNoTracking()
                .Where(c =>
                    cedulas.Contains(c.Cedula) &&
                    fechas.Contains(c.Fecha.Date)
                )
                .ToListAsync();

            var resultados = datosBase.Select(item =>
            {
                var empleado = empleados.FirstOrDefault(e => e.Cedula == item.Cedula);

                var cierre = cierres
                    .Where(c =>
                        c.Cedula == item.Cedula &&
                        c.Fecha.Date == item.FechaVenta.Date
                    )
                    .OrderByDescending(c => c.Fecha)
                    .FirstOrDefault();

                return new ReporteVentasItemViewModel
                {
                    IdVenta = item.IdVenta,
                    IdFactura = item.IdFactura,

                    NumeroFactura = item.NumeroFacturaReal ?? item.NumeroFacturaVenta,

                    FechaVenta = item.FechaVenta,
                    FechaEmisionFactura = item.FechaEmisionFacturaReal ?? item.FechaEmisionFacturaVenta,

                    IdCliente = item.IdCliente,
                    CedulaCliente = item.CedulaCliente,

                    CedulaVendedor = item.Cedula,

                    /*
                        Ajusta empleado.Nombre según tu modelo real:
                        puede ser NombreCompleto, NombreEmpleado, Nombres, etc.
                    */
                    NombreVendedor = empleado != null
                        ? empleado.Nombre
                        : item.Cedula.ToString(),

                    MetodoPago = item.MetodoPago,

                    NombreSede = cierre?.NombreSede,
                    NombrePdv = cierre?.NombrePdv,

                    TotalVenta = item.Total,
                    TotalFactura = item.TotalFactura,

                    EstadoVenta = item.EstadoVenta,
                    EstadoFactura = item.EstadoFactura,

                    TipoVenta = item.TipoVenta,
                    Conceptos = item.Conceptos
                };
            }).ToList();

            return new ReporteVentasViewModel
            {
                Filtros = filtros,
                Resultados = resultados,

                PaginaActual = pagina,
                TotalPaginas = totalPaginas,
                TotalRegistros = totalRegistros,
                RegistrosPorPagina = registrosPorPagina,

                TotalPagina = resultados.Sum(x => x.TotalVenta),
                TotalGeneralFiltrado = totalGeneralFiltrado,

                Vendedores = await CargarVendedoresAsync(),
                Sedes = await CargarSedesAsync(),
                Pdvs = await CargarPdvsAsync(),
                EstadosVenta = await CargarEstadosVentaAsync(),
                EstadosFactura = await CargarEstadosFacturaAsync(),
                MetodosPago = await CargarMetodosPagoAsync()
            };
        }

        private async Task<List<SelectListItem>> CargarVendedoresAsync()
        {
            var vendedores = await _context.Ventas
                .AsNoTracking()
                .Select(v => v.Cedula)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            var empleados = await _context.Empleado
                .AsNoTracking()
                .Where(e => vendedores.Contains(e.Cedula))
                .ToListAsync();

            var items = vendedores.Select(cedula =>
            {
                var empleado = empleados.FirstOrDefault(e => e.Cedula == cedula);

                return new SelectListItem
                {
                    Value = cedula.ToString(),

                    /*
                        Ajusta empleado.Nombre según tu modelo real.
                    */
                    Text = empleado != null
                        ? $"{empleado.Nombre} - {cedula}"
                        : cedula.ToString()
                };
            }).ToList();

            return items;
        }

        private async Task<List<SelectListItem>> CargarSedesAsync()
        {
            return await _context.Sede
                .AsNoTracking()
                .OrderBy(s => s.NombreSede)
                .Select(s => new SelectListItem
                {
                    Value = s.Id_sede.ToString(),
                    Text = s.NombreSede ?? "Sin nombre"
                })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> CargarPdvsAsync()
        {
            return await _context.Infopdv
                .AsNoTracking()
                .OrderBy(p => p.NombreInfoPDV)
                .Select(p => new SelectListItem
                {
                    Value = p.InfopdvId.ToString(),
                    Text = p.NombreInfoPDV ?? "Sin nombre"
                })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> CargarMetodosPagoAsync()
        {
            return await _context.Ventas
                .AsNoTracking()
                .Where(v => v.MetodoPago != null && v.MetodoPago != "")
                .Select(v => v.MetodoPago)
                .Distinct()
                .OrderBy(m => m)
                .Select(m => new SelectListItem
                {
                    Value = m,
                    Text = m
                })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> CargarEstadosVentaAsync()
        {
            return await _context.Ventas
                .AsNoTracking()
                .Where(v => v.EstadoVenta != null && v.EstadoVenta != "")
                .Select(v => v.EstadoVenta)
                .Distinct()
                .OrderBy(e => e)
                .Select(e => new SelectListItem
                {
                    Value = e,
                    Text = e
                })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> CargarEstadosFacturaAsync()
        {
            return await _context.Factura
                .AsNoTracking()
                .Where(f => f.EstadoFactura != null && f.EstadoFactura != "")
                .Select(f => f.EstadoFactura)
                .Distinct()
                .OrderBy(e => e)
                .Select(e => new SelectListItem
                {
                    Value = e,
                    Text = e
                })
                .ToListAsync();
        }
        //Centro de Reporte
        public async Task<CentroReportesViewModel> ObtenerCentroReportesAsync(
            CentroReportesFiltroViewModel filtros,
            int pagina,
            int registrosPorPagina)
        {
            if (pagina < 1)
                pagina = 1;

            if (registrosPorPagina <= 0)
                registrosPorPagina = 20;

            var vm = new CentroReportesViewModel
            {
                Filtros = filtros,
                PaginaActual = pagina,
                RegistrosPorPagina = registrosPorPagina,
                TiposReporte = CargarTiposReporte(),
                Sedes = await CargarSedesAsync(),
                Pdvs = await CargarPdvsAsync(),
                Vendedores = await CargarVendedoresAsync(),
                MetodosPago = await CargarMetodosPagoAsync()
            };

            if (string.IsNullOrWhiteSpace(filtros.TipoReporte))
            {
                vm.Mensaje = "Seleccione un tipo de reporte para consultar.";
                return vm;
            }

            switch (filtros.TipoReporte)
            {
                case TiposReporteOperativo.DetalleVentaProducto:
                    return await ReporteDetalleVentaProductoAsync(vm, filtros, pagina, registrosPorPagina);

                case TiposReporteOperativo.VentasMetodoPago:
                    return await ReporteVentasMetodoPagoAsync(vm, filtros, pagina, registrosPorPagina);

                case TiposReporteOperativo.CierresCaja:
                    return await ReporteCierresCajaAsync(vm, filtros, pagina, registrosPorPagina);

                case TiposReporteOperativo.DiferenciasCaja:
                    return await ReporteDiferenciasCajaAsync(vm, filtros, pagina, registrosPorPagina);

                case TiposReporteOperativo.InventarioGeneral:
                    return await ReporteInventarioGeneralAsync(vm, filtros, pagina, registrosPorPagina);

                case TiposReporteOperativo.StockBajo:
                    return await ReporteStockBajoAsync(vm, filtros, pagina, registrosPorPagina);

                case TiposReporteOperativo.KardexVentas:
                    return await ReporteKardexVentasAsync(vm, filtros, pagina, registrosPorPagina);

                case TiposReporteOperativo.ProductosVendidos:
                    return await ReporteProductosVendidosAsync(vm, filtros, pagina, registrosPorPagina);

                default:
                    vm.Mensaje = "El tipo de reporte seleccionado no es válido.";
                    return vm;
            }
        }

        private async Task<CentroReportesViewModel> ReporteDetalleVentaProductoAsync(
            CentroReportesViewModel vm,
            CentroReportesFiltroViewModel filtros,
            int pagina,
            int registrosPorPagina)
        {
            vm.TituloReporte = "Detalle de venta por producto";

            var query = _context.Pedidos
                .AsNoTracking()
                .Include(p => p.Venta)
                .AsQueryable();

            if (filtros.FechaInicial.HasValue)
            {
                var fechaInicial = filtros.FechaInicial.Value.Date;
                query = query.Where(p => p.FechaRegistro.Date >= fechaInicial);
            }

            if (filtros.FechaFinal.HasValue)
            {
                var fechaFinal = filtros.FechaFinal.Value.Date;
                query = query.Where(p => p.FechaRegistro.Date <= fechaFinal);
            }

            if (!string.IsNullOrWhiteSpace(filtros.Concepto))
            {
                var concepto = filtros.Concepto.Trim();
                query = query.Where(p => p.Codigo != null && p.Codigo.Contains(concepto));
            }

            if (filtros.InfopdvId.HasValue)
            {
                query = query.Where(p => p.InfopdvId == filtros.InfopdvId.Value);
            }

            if (filtros.CedulaVendedor.HasValue)
            {
                query = query.Where(p => p.Venta.Cedula == filtros.CedulaVendedor.Value);
            }

            if (!string.IsNullOrWhiteSpace(filtros.MetodoPago))
            {
                query = query.Where(p => p.Venta.MetodoPago == filtros.MetodoPago);
            }

            var totalRegistros = await query.CountAsync();
            var totalGeneral = await query.SumAsync(p => (decimal?)p.SubTotal) ?? 0;

            var datos = await query
                .OrderByDescending(p => p.FechaRegistro)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .Select(p => new
                {
                    p.FechaRegistro,
                    p.IdVenta,
                    p.IdPedido,
                    p.Codigo,
                    p.Stock,
                    p.VNeto,
                    p.VUnidad,
                    p.VVenta,
                    p.SubTotal,
                    p.IvaPorcentaje,
                    p.IvaValor,
                    p.InfopdvId,
                    MetodoPago = p.Venta.MetodoPago,
                    CedulaVendedor = p.Venta.Cedula,
                    EstadoVenta = p.Venta.EstadoVenta
                })
                .ToListAsync();

            vm.Columnas = new List<string>
            {
                "Fecha",
                "IdVenta",
                "IdPedido",
                "Código",
                "Cantidad",
                "Costo unitario",
                "Costo total",
                "Valor venta unitario",
                "IVA %",
                "IVA valor",
                "Subtotal",
                "PDV",
                "Método pago",
                "Vendedor",
                "Estado"
            };

            vm.Filas = datos.Select(x => new CentroReportesFilaViewModel
            {
                Valores = new Dictionary<string, string>
                {
                    ["Fecha"] = x.FechaRegistro.ToString("dd/MM/yyyy HH:mm"),
                    ["IdVenta"] = x.IdVenta.ToString(),
                    ["IdPedido"] = x.IdPedido.ToString(),
                    ["Código"] = x.Codigo ?? "",
                    ["Cantidad"] = x.Stock.ToString("N2"),
                    ["Costo unitario"] = (x.VNeto ?? 0).ToString("N0"),
                    ["Costo total"] = (x.VUnidad ?? 0).ToString("N0"),
                    ["Valor venta unitario"] = x.VVenta.ToString("N0"),
                    ["IVA %"] = (x.IvaPorcentaje ?? 0).ToString("N2"),
                    ["IVA valor"] = (x.IvaValor ?? 0).ToString("N0"),
                    ["Subtotal"] = x.SubTotal.ToString("N0"),
                    ["PDV"] = x.InfopdvId.ToString(),
                    ["Método pago"] = x.MetodoPago ?? "",
                    ["Vendedor"] = x.CedulaVendedor.ToString(),
                    ["Estado"] = x.EstadoVenta ?? ""
                }
            }).ToList();

            CompletarPaginacion(vm, totalRegistros, totalGeneral, pagina, registrosPorPagina);

            return vm;
        }

        private async Task<CentroReportesViewModel> ReporteVentasMetodoPagoAsync(
            CentroReportesViewModel vm,
            CentroReportesFiltroViewModel filtros,
            int pagina,
            int registrosPorPagina)
        {
            vm.TituloReporte = "Ventas por método de pago";

            var query = _context.Ventas
                .AsNoTracking()
                .AsQueryable();

            if (filtros.FechaInicial.HasValue)
            {
                var fechaInicial = filtros.FechaInicial.Value.Date;
                query = query.Where(v => v.FechaVenta.Date >= fechaInicial);
            }

            if (filtros.FechaFinal.HasValue)
            {
                var fechaFinal = filtros.FechaFinal.Value.Date;
                query = query.Where(v => v.FechaVenta.Date <= fechaFinal);
            }

            if (filtros.CedulaVendedor.HasValue)
            {
                query = query.Where(v => v.Cedula == filtros.CedulaVendedor.Value);
            }

            if (!string.IsNullOrWhiteSpace(filtros.MetodoPago))
            {
                query = query.Where(v => v.MetodoPago == filtros.MetodoPago);
            }

            var agrupado = await query
                .GroupBy(v => v.MetodoPago)
                .Select(g => new
                {
                    MetodoPago = g.Key,
                    CantidadVentas = g.Count(),
                    Total = g.Sum(x => x.Total)
                })
                .OrderByDescending(x => x.Total)
                .ToListAsync();

            var totalRegistros = agrupado.Count;
            var totalGeneral = agrupado.Sum(x => x.Total);

            var datos = agrupado
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToList();

            vm.Columnas = new List<string>
            {
                "Método pago",
                "Cantidad ventas",
                "Total"
            };

            vm.Filas = datos.Select(x => new CentroReportesFilaViewModel
            {
                Valores = new Dictionary<string, string>
                {
                    ["Método pago"] = x.MetodoPago ?? "Sin método",
                    ["Cantidad ventas"] = x.CantidadVentas.ToString(),
                    ["Total"] = x.Total.ToString("N0")
                }
            }).ToList();

            CompletarPaginacion(vm, totalRegistros, totalGeneral, pagina, registrosPorPagina);

            return vm;
        }

        private async Task<CentroReportesViewModel> ReporteCierresCajaAsync(
            CentroReportesViewModel vm,
            CentroReportesFiltroViewModel filtros,
            int pagina,
            int registrosPorPagina)
        {
            vm.TituloReporte = "Cierres de caja";

            var query = _context.CierreCajas
                .AsNoTracking()
                .AsQueryable();

            if (filtros.FechaInicial.HasValue)
            {
                var fechaInicial = filtros.FechaInicial.Value.Date;
                query = query.Where(c => c.Fecha.Date >= fechaInicial);
            }

            if (filtros.FechaFinal.HasValue)
            {
                var fechaFinal = filtros.FechaFinal.Value.Date;
                query = query.Where(c => c.Fecha.Date <= fechaFinal);
            }

            if (filtros.IdSede.HasValue)
            {
                query = query.Where(c => c.IdSede == filtros.IdSede.Value);
            }

            if (filtros.InfopdvId.HasValue)
            {
                query = query.Where(c => c.InfopdvId == filtros.InfopdvId.Value);
            }

            if (filtros.CedulaVendedor.HasValue)
            {
                query = query.Where(c => c.Cedula == filtros.CedulaVendedor.Value);
            }

            if (!string.IsNullOrWhiteSpace(filtros.Concepto))
            {
                var concepto = filtros.Concepto.Trim();
                query = query.Where(c =>
                    (c.Concepto != null && c.Concepto.Contains(concepto)) ||
                    (c.TipoMovimiento != null && c.TipoMovimiento.Contains(concepto))
                );
            }

            var totalRegistros = await query.CountAsync();
            var totalGeneral = await query.SumAsync(c => (decimal?)c.Monto) ?? 0;

            var datos = await query
                .OrderByDescending(c => c.Fecha)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToListAsync();

            vm.Columnas = new List<string>
            {
                "Fecha",
                "Tipo movimiento",
                "Concepto",
                "Monto",
                "Efectivo",
                "Transferencia",
                "Gastos efectivo",
                "Gastos transferencia",
                "Diferencia",
                "Cédula",
                "Empresa",
                "Sede",
                "PDV",
                "Rol"
            };

            vm.Filas = datos.Select(x => new CentroReportesFilaViewModel
            {
                Valores = new Dictionary<string, string>
                {
                    ["Fecha"] = x.Fecha.ToString("dd/MM/yyyy HH:mm"),
                    ["Tipo movimiento"] = x.TipoMovimiento ?? "",
                    ["Concepto"] = x.Concepto ?? "",
                    ["Monto"] = x.Monto.ToString("N0"),
                    ["Efectivo"] = (x.Efectivo ?? 0).ToString("N0"),
                    ["Transferencia"] = (x.Transferencia ?? 0).ToString("N0"),
                    ["Gastos efectivo"] = (x.GastosEfectivo ?? 0).ToString("N0"),
                    ["Gastos transferencia"] = (x.GastosTransferencia ?? 0).ToString("N0"),
                    ["Diferencia"] = (x.Diferencia ?? 0).ToString("N0"),
                    ["Cédula"] = x.Cedula.ToString(),
                    ["Empresa"] = x.NombreEmpresa ?? "",
                    ["Sede"] = x.NombreSede ?? "",
                    ["PDV"] = x.NombrePdv ?? "",
                    ["Rol"] = x.NombreRol ?? ""
                }
            }).ToList();

            CompletarPaginacion(vm, totalRegistros, totalGeneral, pagina, registrosPorPagina);

            return vm;
        }

        private async Task<CentroReportesViewModel> ReporteDiferenciasCajaAsync(
            CentroReportesViewModel vm,
            CentroReportesFiltroViewModel filtros,
            int pagina,
            int registrosPorPagina)
        {
            vm.TituloReporte = "Diferencias de caja";

            var query = _context.CierreCajas
                .AsNoTracking()
                .Where(c => c.Diferencia != null && c.Diferencia != 0)
                .AsQueryable();

            if (filtros.FechaInicial.HasValue)
            {
                var fechaInicial = filtros.FechaInicial.Value.Date;
                query = query.Where(c => c.Fecha.Date >= fechaInicial);
            }

            if (filtros.FechaFinal.HasValue)
            {
                var fechaFinal = filtros.FechaFinal.Value.Date;
                query = query.Where(c => c.Fecha.Date <= fechaFinal);
            }

            if (filtros.IdSede.HasValue)
            {
                query = query.Where(c => c.IdSede == filtros.IdSede.Value);
            }

            if (filtros.InfopdvId.HasValue)
            {
                query = query.Where(c => c.InfopdvId == filtros.InfopdvId.Value);
            }

            if (filtros.CedulaVendedor.HasValue)
            {
                query = query.Where(c => c.Cedula == filtros.CedulaVendedor.Value);
            }

            var totalRegistros = await query.CountAsync();
            var totalGeneral = await query.SumAsync(c => (decimal?)c.Diferencia) ?? 0;

            var datos = await query
                .OrderByDescending(c => c.Fecha)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToListAsync();

            vm.Columnas = new List<string>
            {
                "Fecha",
                "Cédula",
                "Sede",
                "PDV",
                "Efectivo",
                "Transferencia",
                "Gastos efectivo",
                "Gastos transferencia",
                "Diferencia",
                "Concepto"
            };

            vm.Filas = datos.Select(x => new CentroReportesFilaViewModel
            {
                Valores = new Dictionary<string, string>
                {
                    ["Fecha"] = x.Fecha.ToString("dd/MM/yyyy HH:mm"),
                    ["Cédula"] = x.Cedula.ToString(),
                    ["Sede"] = x.NombreSede ?? "",
                    ["PDV"] = x.NombrePdv ?? "",
                    ["Efectivo"] = (x.Efectivo ?? 0).ToString("N0"),
                    ["Transferencia"] = (x.Transferencia ?? 0).ToString("N0"),
                    ["Gastos efectivo"] = (x.GastosEfectivo ?? 0).ToString("N0"),
                    ["Gastos transferencia"] = (x.GastosTransferencia ?? 0).ToString("N0"),
                    ["Diferencia"] = (x.Diferencia ?? 0).ToString("N0"),
                    ["Concepto"] = x.Concepto ?? ""
                }
            }).ToList();

            CompletarPaginacion(vm, totalRegistros, totalGeneral, pagina, registrosPorPagina);

            return vm;
        }

        private async Task<CentroReportesViewModel> ReporteInventarioGeneralAsync(
            CentroReportesViewModel vm,
            CentroReportesFiltroViewModel filtros,
            int pagina,
            int registrosPorPagina)
        {
            vm.TituloReporte = "Inventario general";

            var query = _context.Productos
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtros.Concepto))
            {
                var concepto = filtros.Concepto.Trim();

                query = query.Where(p =>
                    p.Cod_Producto.Contains(concepto) ||
                    p.NombreProducto.Contains(concepto)
                );
            }

            var totalRegistros = await query.CountAsync();

            var datos = await query
                .OrderBy(p => p.NombreProducto)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .Select(p => new
                {
                    p.Cod_Producto,
                    p.NombreProducto,
                    p.CantidadProducto,
                    p.ValorNetoProducto,
                    p.ValorUnidad,
                    p.ValorVentaProducto,
                    p.ID_Empresa,
                    p.Estado
                })
                .ToListAsync();

            var totalGeneral = datos.Sum(x => (x.ValorUnidad ?? 0) * (x.CantidadProducto));

            vm.Columnas = new List<string>
            {
                "Código",
                "Producto",
                "Stock",
                "Valor neto",
                "Valor unidad",
                "Valor venta",
                "Costo inventario",
                "Venta potencial",
                "Empresa",
                "Estado"
            };

            vm.Filas = datos.Select(x => new CentroReportesFilaViewModel
            {
                Valores = new Dictionary<string, string>
                {
                    ["Código"] = x.Cod_Producto ?? "",
                    ["Producto"] = x.NombreProducto ?? "",
                    ["Stock"] = (x.CantidadProducto).ToString("N2"),
                    ["Valor neto"] = (x.ValorNetoProducto ?? 0).ToString("N0"),
                    ["Valor unidad"] = (x.ValorUnidad ?? 0).ToString("N0"),
                    ["Valor venta"] = (x.ValorVentaProducto ?? 0).ToString("N0"),
                    ["Costo inventario"] = ((x.ValorUnidad ?? 0) * (x.CantidadProducto)).ToString("N0"),
                    ["Venta potencial"] = ((x.ValorVentaProducto ?? 0) * (x.CantidadProducto)).ToString("N0"),
                    ["Empresa"] = x.ID_Empresa ?? "",
                    ["Estado"] = x.Estado.ToString()
                }
            }).ToList();

            CompletarPaginacion(vm, totalRegistros, totalGeneral, pagina, registrosPorPagina);

            return vm;
        }

        private async Task<CentroReportesViewModel> ReporteStockBajoAsync(
            CentroReportesViewModel vm,
            CentroReportesFiltroViewModel filtros,
            int pagina,
            int registrosPorPagina)
        {
            vm.TituloReporte = "Stock bajo";

            var stockMinimo = filtros.StockMinimo ?? 5;

            var query = _context.Productos
                .AsNoTracking()
                .Where(p => p.CantidadProducto <= stockMinimo)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtros.Concepto))
            {
                var concepto = filtros.Concepto.Trim();

                query = query.Where(p =>
                    p.Cod_Producto.Contains(concepto) ||
                    p.NombreProducto.Contains(concepto)
                );
            }

            var totalRegistros = await query.CountAsync();

            var datos = await query
                .OrderBy(p => p.CantidadProducto)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .Select(p => new
                {
                    p.Cod_Producto,
                    p.NombreProducto,
                    p.CantidadProducto,
                    p.ValorUnidad,
                    p.ValorVentaProducto,
                    p.Estado
                })
                .ToListAsync();

            vm.Columnas = new List<string>
            {
                "Código",
                "Producto",
                "Stock actual",
                "Stock mínimo",
                "Valor unidad",
                "Valor venta",
                "Estado alerta",
                "Estado producto"
            };

            vm.Filas = datos.Select(x => new CentroReportesFilaViewModel
            {
                Valores = new Dictionary<string, string>
                {
                    ["Código"] = x.Cod_Producto ?? "",
                    ["Producto"] = x.NombreProducto ?? "",
                    ["Stock actual"] = (x.CantidadProducto).ToString("N2"),
                    ["Stock mínimo"] = stockMinimo.ToString("N2"),
                    ["Valor unidad"] = (x.ValorUnidad ?? 0).ToString("N0"),
                    ["Valor venta"] = (x.ValorVentaProducto ?? 0).ToString("N0"),
                    ["Estado alerta"] = (x.CantidadProducto) <= 0 ? "Agotado" : "Bajo",
                    ["Estado producto"] = x.Estado.ToString()
                }
            }).ToList();

            CompletarPaginacion(vm, totalRegistros, 0, pagina, registrosPorPagina);

            return vm;
        }

        private async Task<CentroReportesViewModel> ReporteKardexVentasAsync(
            CentroReportesViewModel vm,
            CentroReportesFiltroViewModel filtros,
            int pagina,
            int registrosPorPagina)
        {
            vm.TituloReporte = "Kardex por ventas";

            var query = _context.Pedidos
                .AsNoTracking()
                .Include(p => p.Venta)
                .AsQueryable();

            if (filtros.FechaInicial.HasValue)
            {
                var fechaInicial = filtros.FechaInicial.Value.Date;
                query = query.Where(p => p.FechaRegistro.Date >= fechaInicial);
            }

            if (filtros.FechaFinal.HasValue)
            {
                var fechaFinal = filtros.FechaFinal.Value.Date;
                query = query.Where(p => p.FechaRegistro.Date <= fechaFinal);
            }

            if (!string.IsNullOrWhiteSpace(filtros.Concepto))
            {
                var concepto = filtros.Concepto.Trim();
                query = query.Where(p => p.Codigo != null && p.Codigo.Contains(concepto));
            }

            if (filtros.InfopdvId.HasValue)
            {
                query = query.Where(p => p.InfopdvId == filtros.InfopdvId.Value);
            }

            var totalRegistros = await query.CountAsync();

            var datos = await query
                .OrderByDescending(p => p.FechaRegistro)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .Select(p => new
                {
                    p.FechaRegistro,
                    p.Codigo,
                    p.IdVenta,
                    p.IdPedido,
                    p.Stock,
                    p.VUnidad,
                    p.SubTotal,
                    p.InfopdvId,
                    CedulaVendedor = p.Venta.Cedula,
                    MetodoPago = p.Venta.MetodoPago
                })
                .ToListAsync();

            vm.Columnas = new List<string>
            {
                "Fecha",
                "Código",
                "Tipo movimiento",
                "Entrada",
                "Salida",
                "IdVenta",
                "IdPedido",
                "Costo salida",
                "Valor venta",
                "PDV",
                "Vendedor",
                "Método pago"
            };

            vm.Filas = datos.Select(x => new CentroReportesFilaViewModel
            {
                Valores = new Dictionary<string, string>
                {
                    ["Fecha"] = x.FechaRegistro.ToString("dd/MM/yyyy HH:mm"),
                    ["Código"] = x.Codigo ?? "",
                    ["Tipo movimiento"] = "Venta",
                    ["Entrada"] = "0",
                    ["Salida"] = x.Stock.ToString("N2"),
                    ["IdVenta"] = x.IdVenta.ToString(),
                    ["IdPedido"] = x.IdPedido.ToString(),
                    ["Costo salida"] = (x.VUnidad ?? 0).ToString("N0"),
                    ["Valor venta"] = x.SubTotal.ToString("N0"),
                    ["PDV"] = x.InfopdvId.ToString(),
                    ["Vendedor"] = x.CedulaVendedor.ToString(),
                    ["Método pago"] = x.MetodoPago ?? ""
                }
            }).ToList();

            CompletarPaginacion(vm, totalRegistros, 0, pagina, registrosPorPagina);

            return vm;
        }

        private async Task<CentroReportesViewModel> ReporteProductosVendidosAsync(
            CentroReportesViewModel vm,
            CentroReportesFiltroViewModel filtros,
            int pagina,
            int registrosPorPagina)
        {
            vm.TituloReporte = "Productos vendidos";

            var query = _context.Pedidos
                .AsNoTracking()
                .Include(p => p.Venta)
                .AsQueryable();

            if (filtros.FechaInicial.HasValue)
            {
                var fechaInicial = filtros.FechaInicial.Value.Date;
                query = query.Where(p => p.FechaRegistro.Date >= fechaInicial);
            }

            if (filtros.FechaFinal.HasValue)
            {
                var fechaFinal = filtros.FechaFinal.Value.Date;
                query = query.Where(p => p.FechaRegistro.Date <= fechaFinal);
            }

            if (!string.IsNullOrWhiteSpace(filtros.Concepto))
            {
                var concepto = filtros.Concepto.Trim();
                query = query.Where(p => p.Codigo != null && p.Codigo.Contains(concepto));
            }

            if (filtros.InfopdvId.HasValue)
            {
                query = query.Where(p => p.InfopdvId == filtros.InfopdvId.Value);
            }

            var agrupado = await query
                .GroupBy(p => p.Codigo)
                .Select(g => new
                {
                    Codigo = g.Key,
                    CantidadVendida = g.Sum(x => x.Stock),
                    TotalCosto = g.Sum(x => x.VUnidad ?? 0),
                    TotalVenta = g.Sum(x => x.SubTotal),
                    CantidadRegistros = g.Count()
                })
                .OrderByDescending(x => x.CantidadVendida)
                .ToListAsync();

            var totalRegistros = agrupado.Count;
            var totalGeneral = agrupado.Sum(x => x.TotalVenta);

            var datos = agrupado
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToList();

            vm.Columnas = new List<string>
            {
                "Código",
                "Cantidad vendida",
                "Cantidad movimientos",
                "Total costo",
                "Total venta",
                "Utilidad estimada"
            };

            vm.Filas = datos.Select(x => new CentroReportesFilaViewModel
            {
                Valores = new Dictionary<string, string>
                {
                    ["Código"] = x.Codigo ?? "",
                    ["Cantidad vendida"] = x.CantidadVendida.ToString("N2"),
                    ["Cantidad movimientos"] = x.CantidadRegistros.ToString(),
                    ["Total costo"] = x.TotalCosto.ToString("N0"),
                    ["Total venta"] = x.TotalVenta.ToString("N0"),
                    ["Utilidad estimada"] = (x.TotalVenta - x.TotalCosto).ToString("N0")
                }
            }).ToList();

            CompletarPaginacion(vm, totalRegistros, totalGeneral, pagina, registrosPorPagina);

            return vm;
        }

        private void CompletarPaginacion(
            CentroReportesViewModel vm,
            int totalRegistros,
            decimal totalGeneral,
            int pagina,
            int registrosPorPagina)
        {
            vm.TotalRegistros = totalRegistros;
            vm.TotalGeneral = totalGeneral;
            vm.PaginaActual = pagina;
            vm.RegistrosPorPagina = registrosPorPagina;
            vm.TotalPaginas = (int)Math.Ceiling(totalRegistros / (double)registrosPorPagina);
        }

        private List<SelectListItem> CargarTiposReporte()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = TiposReporteOperativo.DetalleVentaProducto, Text = "1.2 Detalle de venta por producto" },
                new SelectListItem { Value = TiposReporteOperativo.VentasMetodoPago, Text = "1.3 Ventas por método de pago" },
                new SelectListItem { Value = TiposReporteOperativo.CierresCaja, Text = "2. Reportes de cierre de caja" },
                new SelectListItem { Value = TiposReporteOperativo.DiferenciasCaja, Text = "2.2 Reporte de diferencias de caja" },
                new SelectListItem { Value = TiposReporteOperativo.InventarioGeneral, Text = "3.1 Inventario general" },
                new SelectListItem { Value = TiposReporteOperativo.StockBajo, Text = "3.2 Stock bajo" },
                new SelectListItem { Value = TiposReporteOperativo.KardexVentas, Text = "3.3 Kardex por ventas" },
                new SelectListItem { Value = TiposReporteOperativo.ProductosVendidos, Text = "Productos vendidos agrupados" }
            };
        }

        //Reportes Avanzados
        public async Task<CentroReportesViewModel> ObtenerCentroReportesAvanzadosAsync(
            CentroReportesFiltroViewModel filtros,
            int pagina,
            int registrosPorPagina)
        {
            if (pagina < 1)
                pagina = 1;

            if (registrosPorPagina <= 0)
                registrosPorPagina = 20;

            var vm = new CentroReportesViewModel
            {
                Filtros = filtros,
                PaginaActual = pagina,
                RegistrosPorPagina = registrosPorPagina,
                TiposReporte = CargarTiposReporteAvanzado(),
                Sedes = await CargarSedesAsync(),
                Pdvs = await CargarPdvsAsync(),
                Vendedores = await CargarVendedoresAsync(),
                MetodosPago = await CargarMetodosPagoAsync()
            };

            if (string.IsNullOrWhiteSpace(filtros.TipoReporte))
            {
                vm.Mensaje = "Seleccione un tipo de reporte avanzado para consultar.";
                return vm;
            }

            switch (filtros.TipoReporte)
            {
                // 1. Reportes adicionales de ventas
                case TiposReporteAvanzado.ProductosMasVendidos:
                    return await ReporteProductosMasVendidosAsync(vm, filtros, pagina, registrosPorPagina);

                case TiposReporteAvanzado.ProductosSinMovimiento:
                    return await ReporteProductosSinMovimientoAsync(vm, filtros, pagina, registrosPorPagina);

                case TiposReporteAvanzado.VentasAnuladas:
                    return await ReporteVentasAnuladasAsync(vm, filtros, pagina, registrosPorPagina);

                case TiposReporteAvanzado.VentasPorHora:
                    return await ReporteVentasPorHoraAsync(vm, filtros, pagina, registrosPorPagina);

                case TiposReporteAvanzado.TicketPromedio:
                    return await ReporteTicketPromedioAsync(vm, filtros, pagina, registrosPorPagina);

                // 2. Reportes adicionales de inventario
                case TiposReporteAvanzado.InventarioValorizado:
                    return await ReporteInventarioValorizadoAsync(vm, filtros, pagina, registrosPorPagina);

                case TiposReporteAvanzado.ProductosSinStock:
                    return await ReporteProductosSinStockAsync(vm, filtros, pagina, registrosPorPagina);

                case TiposReporteAvanzado.ProductosStockNegativo:
                    return await ReporteProductosStockNegativoAsync(vm, filtros, pagina, registrosPorPagina);

                case TiposReporteAvanzado.RotacionInventario:
                    return await ReporteRotacionInventarioAsync(vm, filtros, pagina, registrosPorPagina);

                // 3. Reportes de caja y control
                case TiposReporteAvanzado.ComparativoVentasCierre:
                    return await ReporteComparativoVentasCierreAsync(vm, filtros, pagina, registrosPorPagina);

                case TiposReporteAvanzado.CierresPendientes:
                    return await ReporteCierresPendientesAsync(vm, filtros, pagina, registrosPorPagina);

                case TiposReporteAvanzado.CierresPorEmpleado:
                    return await ReporteCierresPorEmpleadoAsync(vm, filtros, pagina, registrosPorPagina);

                case TiposReporteAvanzado.GastosRegistradosCierre:
                    return await ReporteGastosRegistradosCierreAsync(vm, filtros, pagina, registrosPorPagina);

                // 4. Reportes de utilidad
                case TiposReporteAvanzado.UtilidadPorProducto:
                    return await ReporteUtilidadPorProductoAsync(vm, filtros, pagina, registrosPorPagina);

                case TiposReporteAvanzado.UtilidadPorVendedor:
                    return await ReporteUtilidadPorVendedorAsync(vm, filtros, pagina, registrosPorPagina);

                case TiposReporteAvanzado.UtilidadPorPdv:
                    return await ReporteUtilidadPorPdvAsync(vm, filtros, pagina, registrosPorPagina);

                case TiposReporteAvanzado.UtilidadPorDia:
                    return await ReporteUtilidadPorDiaAsync(vm, filtros, pagina, registrosPorPagina);

                default:
                    vm.Mensaje = "El tipo de reporte avanzado seleccionado no es válido.";
                    return vm;
            }
        }
        private List<SelectListItem> CargarTiposReporteAvanzado()
        {
            return new List<SelectListItem>
            {
                // 1. Reportes adicionales de ventas
                new SelectListItem { Value = TiposReporteAvanzado.ProductosMasVendidos, Text = "1.1 Productos más vendidos" },
                new SelectListItem { Value = TiposReporteAvanzado.ProductosSinMovimiento, Text = "1.2 Productos sin movimiento" },
                new SelectListItem { Value = TiposReporteAvanzado.VentasAnuladas, Text = "1.3 Ventas anuladas" },
                new SelectListItem { Value = TiposReporteAvanzado.VentasPorHora, Text = "1.4 Ventas por hora" },
                new SelectListItem { Value = TiposReporteAvanzado.TicketPromedio, Text = "1.5 Ticket promedio" },

                // 2. Reportes adicionales de inventario
                new SelectListItem { Value = TiposReporteAvanzado.InventarioValorizado, Text = "2.1 Inventario valorizado" },
                new SelectListItem { Value = TiposReporteAvanzado.ProductosSinStock, Text = "2.2 Productos sin stock" },
                new SelectListItem { Value = TiposReporteAvanzado.ProductosStockNegativo, Text = "2.3 Productos con stock negativo" },
                new SelectListItem { Value = TiposReporteAvanzado.RotacionInventario, Text = "2.4 Rotación de inventario" },

                // 3. Reportes de caja y control
                new SelectListItem { Value = TiposReporteAvanzado.ComparativoVentasCierre, Text = "3.1 Comparativo ventas vs cierre de caja" },
                new SelectListItem { Value = TiposReporteAvanzado.CierresPendientes, Text = "3.2 Cierres pendientes" },
                new SelectListItem { Value = TiposReporteAvanzado.CierresPorEmpleado, Text = "3.3 Cierres por empleado" },
                new SelectListItem { Value = TiposReporteAvanzado.GastosRegistradosCierre, Text = "3.4 Gastos registrados en cierre" },

                // 4. Reportes de utilidad
                new SelectListItem { Value = TiposReporteAvanzado.UtilidadPorProducto, Text = "4.1 Utilidad por producto" },
                new SelectListItem { Value = TiposReporteAvanzado.UtilidadPorVendedor, Text = "4.2 Utilidad por vendedor" },
                new SelectListItem { Value = TiposReporteAvanzado.UtilidadPorPdv, Text = "4.3 Utilidad por PDV" },
                new SelectListItem { Value = TiposReporteAvanzado.UtilidadPorDia, Text = "4.4 Utilidad por día" }
            };
        }
        private async Task<CentroReportesViewModel> ReporteProductosMasVendidosAsync(
        CentroReportesViewModel vm,
        CentroReportesFiltroViewModel filtros,
        int pagina,
        int registrosPorPagina)
        {
            vm.TituloReporte = "Productos más vendidos";

            var desde = filtros.FechaInicial?.Date;
            var hasta = filtros.FechaFinal?.Date.AddDays(1);

            var query =
                from pedido in _context.Pedidos.AsNoTracking()
                join producto in _context.Productos.AsNoTracking()
                    on pedido.Codigo equals producto.Cod_Producto into productoJoin
                from producto in productoJoin.DefaultIfEmpty()
                select new
                {
                    pedido,
                    producto
                };

            if (desde.HasValue)
                query = query.Where(x => x.pedido.FechaRegistro >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(x => x.pedido.FechaRegistro < hasta.Value);

            if (filtros.InfopdvId.HasValue)
                query = query.Where(x => x.pedido.InfopdvId == filtros.InfopdvId.Value);

            if (!string.IsNullOrWhiteSpace(filtros.Concepto))
            {
                var concepto = filtros.Concepto.Trim();

                query = query.Where(x =>
                    (x.pedido.Codigo != null && x.pedido.Codigo.Contains(concepto)) ||
                    (x.producto != null && x.producto.NombreProducto.Contains(concepto))
                );
            }

            var agrupado = await query
                .GroupBy(x => new
                {
                    Codigo = x.pedido.Codigo,
                    Producto = x.producto != null ? x.producto.NombreProducto : ""
                })
                .Select(g => new
                {
                    g.Key.Codigo,
                    g.Key.Producto,
                    CantidadVendida = g.Sum(x => x.pedido.Stock),
                    TotalVendido = g.Sum(x => x.pedido.SubTotal),
                    CostoTotal = g.Sum(x => x.pedido.VUnidad ?? 0),
                    Movimientos = g.Count()
                })
                .OrderByDescending(x => x.CantidadVendida)
                .ToListAsync();

            var totalRegistros = agrupado.Count;
            var totalGeneral = agrupado.Sum(x => x.TotalVendido);

            var datos = agrupado
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToList();

            vm.Columnas = new List<string>
            {
                "Código",
                "Producto",
                "Cantidad vendida",
                "Movimientos",
                "Total vendido",
                "Costo total",
                "Utilidad estimada"
            };

            vm.Filas = datos.Select(x => new CentroReportesFilaViewModel
            {
                Valores = new Dictionary<string, string>
                {
                    ["Código"] = x.Codigo ?? "",
                    ["Producto"] = x.Producto ?? "",
                    ["Cantidad vendida"] = x.CantidadVendida.ToString("N2"),
                    ["Movimientos"] = x.Movimientos.ToString(),
                    ["Total vendido"] = x.TotalVendido.ToString("N0"),
                    ["Costo total"] = x.CostoTotal.ToString("N0"),
                    ["Utilidad estimada"] = (x.TotalVendido - x.CostoTotal).ToString("N0")
                }
            }).ToList();

            CompletarPaginacion(vm, totalRegistros, totalGeneral, pagina, registrosPorPagina);

            return vm;
        }

        private async Task<CentroReportesViewModel> ReporteProductosSinMovimientoAsync(
        CentroReportesViewModel vm,
        CentroReportesFiltroViewModel filtros,
        int pagina,
        int registrosPorPagina)
        {
            vm.TituloReporte = "Productos sin movimiento";

            var desde = filtros.FechaInicial?.Date;
            var hasta = filtros.FechaFinal?.Date.AddDays(1);

            var productos = _context.Productos.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtros.Concepto))
            {
                var concepto = filtros.Concepto.Trim();

                productos = productos.Where(p =>
                    p.Cod_Producto.Contains(concepto) ||
                    p.NombreProducto.Contains(concepto)
                );
            }

            var pedidos = _context.Pedidos.AsNoTracking().AsQueryable();

            if (desde.HasValue)
                pedidos = pedidos.Where(p => p.FechaRegistro >= desde.Value);

            if (hasta.HasValue)
                pedidos = pedidos.Where(p => p.FechaRegistro < hasta.Value);

            if (filtros.InfopdvId.HasValue)
                pedidos = pedidos.Where(p => p.InfopdvId == filtros.InfopdvId.Value);

            var codigosConMovimiento = await pedidos
                .Where(p => p.Codigo != null)
                .Select(p => p.Codigo!)
                .Distinct()
                .ToListAsync();

            var query = productos
                .Where(p => !codigosConMovimiento.Contains(p.Cod_Producto));

            var totalRegistros = await query.CountAsync();

            var datos = await query
                .OrderBy(p => p.NombreProducto)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .Select(p => new
                {
                    p.Cod_Producto,
                    p.NombreProducto,
                    p.CantidadProducto,
                    p.ValorUnidad,
                    p.ValorVentaProducto,
                    p.Estado
                })
                .ToListAsync();

            vm.Columnas = new List<string>
            {
                "Código",
                "Producto",
                "Stock actual",
                "Valor unidad",
                "Valor venta",
                "Valor congelado",
                "Estado"
            };

            vm.Filas = datos.Select(x => new CentroReportesFilaViewModel
            {
                Valores = new Dictionary<string, string>
                {
                    ["Código"] = x.Cod_Producto ?? "",
                    ["Producto"] = x.NombreProducto ?? "",
                    ["Stock actual"] = x.CantidadProducto.ToString("N2"),
                    ["Valor unidad"] = (x.ValorUnidad ?? 0).ToString("N0"),
                    ["Valor venta"] = (x.ValorVentaProducto ?? 0).ToString("N0"),
                    ["Valor congelado"] = ((x.ValorUnidad ?? 0) * x.CantidadProducto).ToString("N0"),
                    ["Estado"] = x.Estado.ToString()
                }
            }).ToList();

            CompletarPaginacion(vm, totalRegistros, 0, pagina, registrosPorPagina);

            return vm;
        }
        private async Task<CentroReportesViewModel> ReporteVentasAnuladasAsync(
    CentroReportesViewModel vm,
    CentroReportesFiltroViewModel filtros,
    int pagina,
    int registrosPorPagina)
        {
            vm.TituloReporte = "Ventas anuladas";

            var desde = filtros.FechaInicial?.Date;
            var hasta = filtros.FechaFinal?.Date.AddDays(1);

            var query = _context.Ventas
                .AsNoTracking()
                .Where(v => v.EstadoVenta.Contains("Anulada") || v.EstadoVenta.Contains("Anulado"))
                .AsQueryable();

            if (desde.HasValue)
                query = query.Where(v => v.FechaVenta >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(v => v.FechaVenta < hasta.Value);

            if (filtros.CedulaVendedor.HasValue)
                query = query.Where(v => v.Cedula == filtros.CedulaVendedor.Value);

            if (!string.IsNullOrWhiteSpace(filtros.MetodoPago))
                query = query.Where(v => v.MetodoPago == filtros.MetodoPago);

            var totalRegistros = await query.CountAsync();
            var totalGeneral = await query.SumAsync(v => (decimal?)v.Total) ?? 0;

            var datos = await query
                .OrderByDescending(v => v.FechaVenta)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .Select(v => new
                {
                    v.FechaVenta,
                    v.IdVenta,
                    v.IdCliente,
                    v.CedulaCliente,
                    v.Cedula,
                    v.MetodoPago,
                    v.Total,
                    v.EstadoVenta,
                    v.TipoVenta
                })
                .ToListAsync();

            vm.Columnas = new List<string>
    {
        "Fecha",
        "IdVenta",
        "Cliente",
        "Cédula cliente",
        "Vendedor",
        "Método pago",
        "Total",
        "Estado",
        "Tipo venta"
    };

            vm.Filas = datos.Select(x => new CentroReportesFilaViewModel
            {
                Valores = new Dictionary<string, string>
                {
                    ["Fecha"] = x.FechaVenta.ToString("dd/MM/yyyy HH:mm"),
                    ["IdVenta"] = x.IdVenta.ToString(),
                    ["Cliente"] = x.IdCliente.ToString(),
                    ["Cédula cliente"] = x.CedulaCliente.ToString(),
                    ["Vendedor"] = x.Cedula.ToString(),
                    ["Método pago"] = x.MetodoPago ?? "",
                    ["Total"] = x.Total.ToString("N0"),
                    ["Estado"] = x.EstadoVenta ?? "",
                    ["Tipo venta"] = x.TipoVenta ?? ""
                }
            }).ToList();

            CompletarPaginacion(vm, totalRegistros, totalGeneral, pagina, registrosPorPagina);

            return vm;
        }
        private async Task<CentroReportesViewModel> ReporteVentasPorHoraAsync(
    CentroReportesViewModel vm,
    CentroReportesFiltroViewModel filtros,
    int pagina,
    int registrosPorPagina)
        {
            vm.TituloReporte = "Ventas por hora";

            var desde = filtros.FechaInicial?.Date;
            var hasta = filtros.FechaFinal?.Date.AddDays(1);

            var query = _context.Ventas.AsNoTracking().AsQueryable();

            if (desde.HasValue)
                query = query.Where(v => v.FechaVenta >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(v => v.FechaVenta < hasta.Value);

            if (filtros.CedulaVendedor.HasValue)
                query = query.Where(v => v.Cedula == filtros.CedulaVendedor.Value);

            if (!string.IsNullOrWhiteSpace(filtros.MetodoPago))
                query = query.Where(v => v.MetodoPago == filtros.MetodoPago);

            var agrupado = await query
                .GroupBy(v => v.FechaVenta.Hour)
                .Select(g => new
                {
                    Hora = g.Key,
                    CantidadVentas = g.Count(),
                    TotalVendido = g.Sum(x => x.Total),
                    TicketPromedio = g.Average(x => x.Total)
                })
                .OrderBy(x => x.Hora)
                .ToListAsync();

            var totalRegistros = agrupado.Count;
            var totalGeneral = agrupado.Sum(x => x.TotalVendido);

            var datos = agrupado
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToList();

            vm.Columnas = new List<string>
    {
        "Hora",
        "Rango",
        "Cantidad ventas",
        "Total vendido",
        "Ticket promedio"
    };

            vm.Filas = datos.Select(x => new CentroReportesFilaViewModel
            {
                Valores = new Dictionary<string, string>
                {
                    ["Hora"] = x.Hora.ToString("00"),
                    ["Rango"] = $"{x.Hora:00}:00 - {x.Hora + 1:00}:00",
                    ["Cantidad ventas"] = x.CantidadVentas.ToString(),
                    ["Total vendido"] = x.TotalVendido.ToString("N0"),
                    ["Ticket promedio"] = x.TicketPromedio.ToString("N0")
                }
            }).ToList();

            CompletarPaginacion(vm, totalRegistros, totalGeneral, pagina, registrosPorPagina);

            return vm;
        }
        private async Task<CentroReportesViewModel> ReporteTicketPromedioAsync(
    CentroReportesViewModel vm,
    CentroReportesFiltroViewModel filtros,
    int pagina,
    int registrosPorPagina)
        {
            vm.TituloReporte = "Ticket promedio por día";

            var desde = filtros.FechaInicial?.Date;
            var hasta = filtros.FechaFinal?.Date.AddDays(1);

            var query = _context.Ventas.AsNoTracking().AsQueryable();

            if (desde.HasValue)
                query = query.Where(v => v.FechaVenta >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(v => v.FechaVenta < hasta.Value);

            if (filtros.CedulaVendedor.HasValue)
                query = query.Where(v => v.Cedula == filtros.CedulaVendedor.Value);

            if (!string.IsNullOrWhiteSpace(filtros.MetodoPago))
                query = query.Where(v => v.MetodoPago == filtros.MetodoPago);

            var agrupado = await query
                .GroupBy(v => v.FechaVenta.Date)
                .Select(g => new
                {
                    Fecha = g.Key,
                    CantidadVentas = g.Count(),
                    TotalVentas = g.Sum(x => x.Total),
                    TicketPromedio = g.Average(x => x.Total)
                })
                .OrderByDescending(x => x.Fecha)
                .ToListAsync();

            var totalRegistros = agrupado.Count;
            var totalGeneral = agrupado.Sum(x => x.TotalVentas);

            var datos = agrupado
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToList();

            vm.Columnas = new List<string>
    {
        "Fecha",
        "Cantidad ventas",
        "Total ventas",
        "Ticket promedio"
    };

            vm.Filas = datos.Select(x => new CentroReportesFilaViewModel
            {
                Valores = new Dictionary<string, string>
                {
                    ["Fecha"] = x.Fecha.ToString("dd/MM/yyyy"),
                    ["Cantidad ventas"] = x.CantidadVentas.ToString(),
                    ["Total ventas"] = x.TotalVentas.ToString("N0"),
                    ["Ticket promedio"] = x.TicketPromedio.ToString("N0")
                }
            }).ToList();

            CompletarPaginacion(vm, totalRegistros, totalGeneral, pagina, registrosPorPagina);

            return vm;
        }
        private async Task<CentroReportesViewModel> ReporteInventarioValorizadoAsync(
    CentroReportesViewModel vm,
    CentroReportesFiltroViewModel filtros,
    int pagina,
    int registrosPorPagina)
        {
            vm.TituloReporte = "Inventario valorizado";

            var query = _context.Productos.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtros.Concepto))
            {
                var concepto = filtros.Concepto.Trim();

                query = query.Where(p =>
                    p.Cod_Producto.Contains(concepto) ||
                    p.NombreProducto.Contains(concepto)
                );
            }

            var totalRegistros = await query.CountAsync();

            var datos = await query
                .OrderBy(p => p.NombreProducto)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .Select(p => new
                {
                    p.Cod_Producto,
                    p.NombreProducto,
                    p.CantidadProducto,
                    p.ValorUnidad,
                    p.ValorVentaProducto,
                    p.Estado
                })
                .ToListAsync();

            var totalGeneral = datos.Sum(x => (x.ValorUnidad ?? 0) * x.CantidadProducto);

            vm.Columnas = new List<string>
    {
        "Código",
        "Producto",
        "Stock",
        "Costo unitario",
        "Costo total",
        "Precio venta",
        "Venta potencial",
        "Utilidad potencial",
        "Estado"
    };

            vm.Filas = datos.Select(x =>
            {
                var costoTotal = (x.ValorUnidad ?? 0) * x.CantidadProducto;
                var ventaPotencial = (x.ValorVentaProducto ?? 0) * x.CantidadProducto;
                var utilidad = ventaPotencial - costoTotal;

                return new CentroReportesFilaViewModel
                {
                    Valores = new Dictionary<string, string>
                    {
                        ["Código"] = x.Cod_Producto ?? "",
                        ["Producto"] = x.NombreProducto ?? "",
                        ["Stock"] = x.CantidadProducto.ToString("N2"),
                        ["Costo unitario"] = (x.ValorUnidad ?? 0).ToString("N0"),
                        ["Costo total"] = costoTotal.ToString("N0"),
                        ["Precio venta"] = (x.ValorVentaProducto ?? 0).ToString("N0"),
                        ["Venta potencial"] = ventaPotencial.ToString("N0"),
                        ["Utilidad potencial"] = utilidad.ToString("N0"),
                        ["Estado"] = x.Estado.ToString()
                    }
                };
            }).ToList();

            CompletarPaginacion(vm, totalRegistros, totalGeneral, pagina, registrosPorPagina);

            return vm;
        }
        private async Task<CentroReportesViewModel> ReporteProductosSinStockAsync(
    CentroReportesViewModel vm,
    CentroReportesFiltroViewModel filtros,
    int pagina,
    int registrosPorPagina)
        {
            vm.TituloReporte = "Productos sin stock";

            var query = _context.Productos
                .AsNoTracking()
                .Where(p => p.CantidadProducto == 0)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtros.Concepto))
            {
                var concepto = filtros.Concepto.Trim();

                query = query.Where(p =>
                    p.Cod_Producto.Contains(concepto) ||
                    p.NombreProducto.Contains(concepto)
                );
            }

            var totalRegistros = await query.CountAsync();

            var datos = await query
                .OrderBy(p => p.NombreProducto)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .Select(p => new
                {
                    p.Cod_Producto,
                    p.NombreProducto,
                    p.CantidadProducto,
                    p.ValorUnidad,
                    p.ValorVentaProducto,
                    p.Estado
                })
                .ToListAsync();

            vm.Columnas = new List<string>
    {
        "Código",
        "Producto",
        "Stock",
        "Costo unitario",
        "Precio venta",
        "Estado"
    };

            vm.Filas = datos.Select(x => new CentroReportesFilaViewModel
            {
                Valores = new Dictionary<string, string>
                {
                    ["Código"] = x.Cod_Producto ?? "",
                    ["Producto"] = x.NombreProducto ?? "",
                    ["Stock"] = x.CantidadProducto.ToString("N2"),
                    ["Costo unitario"] = (x.ValorUnidad ?? 0).ToString("N0"),
                    ["Precio venta"] = (x.ValorVentaProducto ?? 0).ToString("N0"),
                    ["Estado"] = x.Estado.ToString()
                }
            }).ToList();

            CompletarPaginacion(vm, totalRegistros, 0, pagina, registrosPorPagina);

            return vm;
        }
        private async Task<CentroReportesViewModel> ReporteProductosStockNegativoAsync(
    CentroReportesViewModel vm,
    CentroReportesFiltroViewModel filtros,
    int pagina,
    int registrosPorPagina)
        {
            vm.TituloReporte = "Productos con stock negativo";

            var query = _context.Productos
                .AsNoTracking()
                .Where(p => p.CantidadProducto < 0)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtros.Concepto))
            {
                var concepto = filtros.Concepto.Trim();

                query = query.Where(p =>
                    p.Cod_Producto.Contains(concepto) ||
                    p.NombreProducto.Contains(concepto)
                );
            }

            var totalRegistros = await query.CountAsync();

            var datos = await query
                .OrderBy(p => p.CantidadProducto)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .Select(p => new
                {
                    p.Cod_Producto,
                    p.NombreProducto,
                    p.CantidadProducto,
                    p.ValorUnidad,
                    p.ValorVentaProducto,
                    p.Estado
                })
                .ToListAsync();

            vm.Columnas = new List<string>
    {
        "Código",
        "Producto",
        "Stock actual",
        "Costo unitario",
        "Precio venta",
        "Observación",
        "Estado"
    };

            vm.Filas = datos.Select(x => new CentroReportesFilaViewModel
            {
                Valores = new Dictionary<string, string>
                {
                    ["Código"] = x.Cod_Producto ?? "",
                    ["Producto"] = x.NombreProducto ?? "",
                    ["Stock actual"] = x.CantidadProducto.ToString("N2"),
                    ["Costo unitario"] = (x.ValorUnidad ?? 0).ToString("N0"),
                    ["Precio venta"] = (x.ValorVentaProducto ?? 0).ToString("N0"),
                    ["Observación"] = "Revisar ventas, ajustes o movimientos de inventario",
                    ["Estado"] = x.Estado.ToString()
                }
            }).ToList();

            CompletarPaginacion(vm, totalRegistros, 0, pagina, registrosPorPagina);

            return vm;
        }
        private async Task<CentroReportesViewModel> ReporteRotacionInventarioAsync(
    CentroReportesViewModel vm,
    CentroReportesFiltroViewModel filtros,
    int pagina,
    int registrosPorPagina)
        {
            vm.TituloReporte = "Rotación de inventario";

            var desde = filtros.FechaInicial?.Date;
            var hasta = filtros.FechaFinal?.Date.AddDays(1);

            var query =
                from producto in _context.Productos.AsNoTracking()
                join pedido in _context.Pedidos.AsNoTracking()
                    on producto.Cod_Producto equals pedido.Codigo into pedidosJoin
                from pedido in pedidosJoin.DefaultIfEmpty()
                select new
                {
                    producto,
                    pedido
                };

            if (desde.HasValue)
            {
                query = query.Where(x =>
                    x.pedido == null ||
                    x.pedido.FechaRegistro >= desde.Value
                );
            }

            if (hasta.HasValue)
            {
                query = query.Where(x =>
                    x.pedido == null ||
                    x.pedido.FechaRegistro < hasta.Value
                );
            }

            if (!string.IsNullOrWhiteSpace(filtros.Concepto))
            {
                var concepto = filtros.Concepto.Trim();

                query = query.Where(x =>
                    x.producto.Cod_Producto.Contains(concepto) ||
                    x.producto.NombreProducto.Contains(concepto)
                );
            }

            var agrupado = await query
                .GroupBy(x => new
                {
                    x.producto.Cod_Producto,
                    x.producto.NombreProducto,
                    x.producto.CantidadProducto
                })
                .Select(g => new
                {
                    Codigo = g.Key.Cod_Producto,
                    Producto = g.Key.NombreProducto,
                    StockActual = g.Key.CantidadProducto,
                    CantidadVendida = g.Sum(x => x.pedido != null ? x.pedido.Stock : 0)
                })
                .OrderByDescending(x => x.CantidadVendida)
                .ToListAsync();

            var totalRegistros = agrupado.Count;

            var datos = agrupado
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToList();

            vm.Columnas = new List<string>
    {
        "Código",
        "Producto",
        "Stock actual",
        "Cantidad vendida",
        "Rotación",
        "Estado rotación"
    };

            vm.Filas = datos.Select(x =>
            {
                var rotacion = x.StockActual > 0
                    ? x.CantidadVendida / x.StockActual
                    : 0;

                var estado = x.CantidadVendida == 0
                    ? "Sin movimiento"
                    : rotacion >= 1
                        ? "Alta rotación"
                        : rotacion >= 0.5m
                            ? "Media rotación"
                            : "Baja rotación";

                return new CentroReportesFilaViewModel
                {
                    Valores = new Dictionary<string, string>
                    {
                        ["Código"] = x.Codigo ?? "",
                        ["Producto"] = x.Producto ?? "",
                        ["Stock actual"] = x.StockActual.ToString("N2"),
                        ["Cantidad vendida"] = x.CantidadVendida.ToString("N2"),
                        ["Rotación"] = rotacion.ToString("N2"),
                        ["Estado rotación"] = estado
                    }
                };
            }).ToList();

            CompletarPaginacion(vm, totalRegistros, 0, pagina, registrosPorPagina);

            return vm;
        }
        private async Task<CentroReportesViewModel> ReporteComparativoVentasCierreAsync(
    CentroReportesViewModel vm,
    CentroReportesFiltroViewModel filtros,
    int pagina,
    int registrosPorPagina)
        {
            vm.TituloReporte = "Comparativo ventas vs cierre de caja";

            var desde = filtros.FechaInicial?.Date ?? DateTime.Today;
            var hasta = filtros.FechaFinal?.Date.AddDays(1) ?? DateTime.Today.AddDays(1);

            var ventas = await _context.Ventas
                .AsNoTracking()
                .Where(v => v.FechaVenta >= desde && v.FechaVenta < hasta)
                .GroupBy(v => new
                {
                    Fecha = v.FechaVenta.Date,
                    v.Cedula
                })
                .Select(g => new
                {
                    g.Key.Fecha,
                    g.Key.Cedula,
                    TotalVentas = g.Sum(x => x.Total),
                    CantidadVentas = g.Count()
                })
                .ToListAsync();

            var cierres = await _context.CierreCajas
                .AsNoTracking()
                .Where(c => c.Fecha >= desde && c.Fecha < hasta)
                .ToListAsync();

            if (filtros.CedulaVendedor.HasValue)
            {
                ventas = ventas.Where(x => x.Cedula == filtros.CedulaVendedor.Value).ToList();
                cierres = cierres.Where(x => x.Cedula == filtros.CedulaVendedor.Value).ToList();
            }

            if (filtros.IdSede.HasValue)
                cierres = cierres.Where(x => x.IdSede == filtros.IdSede.Value).ToList();

            if (filtros.InfopdvId.HasValue)
                cierres = cierres.Where(x => x.InfopdvId == filtros.InfopdvId.Value).ToList();

            var datosBase = ventas.Select(v =>
            {
                var cierre = cierres
                    .Where(c => c.Cedula == v.Cedula && c.Fecha.Date == v.Fecha)
                    .OrderByDescending(c => c.Fecha)
                    .FirstOrDefault();

                var totalCierre = cierre?.Monto ?? 0;
                var diferencia = totalCierre - v.TotalVentas;

                var estado = cierre == null
                    ? "Sin cierre"
                    : diferencia == 0
                        ? "Cuadra"
                        : diferencia < 0
                            ? "Faltante"
                            : "Sobrante";

                return new
                {
                    v.Fecha,
                    v.Cedula,
                    v.CantidadVentas,
                    v.TotalVentas,
                    TotalCierre = totalCierre,
                    Diferencia = diferencia,
                    Estado = estado,
                    Sede = cierre?.NombreSede ?? "",
                    Pdv = cierre?.NombrePdv ?? ""
                };
            }).OrderByDescending(x => x.Fecha).ToList();

            var totalRegistros = datosBase.Count;
            var totalGeneral = datosBase.Sum(x => x.TotalVentas);

            var datos = datosBase
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToList();

            vm.Columnas = new List<string>
    {
        "Fecha",
        "Vendedor",
        "Sede",
        "PDV",
        "Cantidad ventas",
        "Total ventas sistema",
        "Total cierre caja",
        "Diferencia",
        "Estado"
    };

            vm.Filas = datos.Select(x => new CentroReportesFilaViewModel
            {
                Valores = new Dictionary<string, string>
                {
                    ["Fecha"] = x.Fecha.ToString("dd/MM/yyyy"),
                    ["Vendedor"] = x.Cedula.ToString(),
                    ["Sede"] = x.Sede,
                    ["PDV"] = x.Pdv,
                    ["Cantidad ventas"] = x.CantidadVentas.ToString(),
                    ["Total ventas sistema"] = x.TotalVentas.ToString("N0"),
                    ["Total cierre caja"] = x.TotalCierre.ToString("N0"),
                    ["Diferencia"] = x.Diferencia.ToString("N0"),
                    ["Estado"] = x.Estado
                }
            }).ToList();

            CompletarPaginacion(vm, totalRegistros, totalGeneral, pagina, registrosPorPagina);

            return vm;
        }
        private async Task<CentroReportesViewModel> ReporteCierresPendientesAsync(
    CentroReportesViewModel vm,
    CentroReportesFiltroViewModel filtros,
    int pagina,
    int registrosPorPagina)
        {
            vm.TituloReporte = "Cierres pendientes";

            var desde = filtros.FechaInicial?.Date ?? DateTime.Today;
            var hasta = filtros.FechaFinal?.Date.AddDays(1) ?? DateTime.Today.AddDays(1);

            var ventas = await _context.Ventas
                .AsNoTracking()
                .Where(v => v.FechaVenta >= desde && v.FechaVenta < hasta)
                .GroupBy(v => new
                {
                    Fecha = v.FechaVenta.Date,
                    v.Cedula
                })
                .Select(g => new
                {
                    g.Key.Fecha,
                    g.Key.Cedula,
                    TotalVentas = g.Sum(x => x.Total),
                    CantidadVentas = g.Count()
                })
                .ToListAsync();

            var cierres = await _context.CierreCajas
                .AsNoTracking()
                .Where(c => c.Fecha >= desde && c.Fecha < hasta)
                .Select(c => new
                {
                    Fecha = c.Fecha.Date,
                    c.Cedula
                })
                .Distinct()
                .ToListAsync();

            if (filtros.CedulaVendedor.HasValue)
                ventas = ventas.Where(x => x.Cedula == filtros.CedulaVendedor.Value).ToList();

            var pendientes = ventas
                .Where(v => !cierres.Any(c => c.Cedula == v.Cedula && c.Fecha == v.Fecha))
                .OrderByDescending(v => v.Fecha)
                .ToList();

            var totalRegistros = pendientes.Count;
            var totalGeneral = pendientes.Sum(x => x.TotalVentas);

            var datos = pendientes
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToList();

            vm.Columnas = new List<string>
    {
        "Fecha",
        "Vendedor",
        "Cantidad ventas",
        "Total ventas",
        "Estado cierre"
    };

            vm.Filas = datos.Select(x => new CentroReportesFilaViewModel
            {
                Valores = new Dictionary<string, string>
                {
                    ["Fecha"] = x.Fecha.ToString("dd/MM/yyyy"),
                    ["Vendedor"] = x.Cedula.ToString(),
                    ["Cantidad ventas"] = x.CantidadVentas.ToString(),
                    ["Total ventas"] = x.TotalVentas.ToString("N0"),
                    ["Estado cierre"] = "Sin cierre registrado"
                }
            }).ToList();

            CompletarPaginacion(vm, totalRegistros, totalGeneral, pagina, registrosPorPagina);

            return vm;
        }
        private async Task<CentroReportesViewModel> ReporteCierresPorEmpleadoAsync(
    CentroReportesViewModel vm,
    CentroReportesFiltroViewModel filtros,
    int pagina,
    int registrosPorPagina)
        {
            vm.TituloReporte = "Cierres por empleado";

            var desde = filtros.FechaInicial?.Date;
            var hasta = filtros.FechaFinal?.Date.AddDays(1);

            var query = _context.CierreCajas.AsNoTracking().AsQueryable();

            if (desde.HasValue)
                query = query.Where(c => c.Fecha >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(c => c.Fecha < hasta.Value);

            if (filtros.CedulaVendedor.HasValue)
                query = query.Where(c => c.Cedula == filtros.CedulaVendedor.Value);

            if (filtros.IdSede.HasValue)
                query = query.Where(c => c.IdSede == filtros.IdSede.Value);

            if (filtros.InfopdvId.HasValue)
                query = query.Where(c => c.InfopdvId == filtros.InfopdvId.Value);

            var agrupado = await query
                .GroupBy(c => c.Cedula)
                .Select(g => new
                {
                    Cedula = g.Key,
                    CantidadCierres = g.Count(),
                    TotalMonto = g.Sum(x => x.Monto),
                    TotalEfectivo = g.Sum(x => x.Efectivo ?? 0),
                    TotalTransferencia = g.Sum(x => x.Transferencia ?? 0),
                    TotalGastos = g.Sum(x => (x.GastosEfectivo ?? 0) + (x.GastosTransferencia ?? 0)),
                    Diferencias = g.Sum(x => x.Diferencia ?? 0)
                })
                .OrderByDescending(x => x.TotalMonto)
                .ToListAsync();

            var totalRegistros = agrupado.Count;
            var totalGeneral = agrupado.Sum(x => x.TotalMonto);

            var datos = agrupado
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToList();

            vm.Columnas = new List<string>
    {
        "Empleado",
        "Cantidad cierres",
        "Total monto",
        "Total efectivo",
        "Total transferencia",
        "Total gastos",
        "Diferencias acumuladas"
    };

            vm.Filas = datos.Select(x => new CentroReportesFilaViewModel
            {
                Valores = new Dictionary<string, string>
                {
                    ["Empleado"] = x.Cedula.ToString(),
                    ["Cantidad cierres"] = x.CantidadCierres.ToString(),
                    ["Total monto"] = x.TotalMonto.ToString("N0"),
                    ["Total efectivo"] = x.TotalEfectivo.ToString("N0"),
                    ["Total transferencia"] = x.TotalTransferencia.ToString("N0"),
                    ["Total gastos"] = x.TotalGastos.ToString("N0"),
                    ["Diferencias acumuladas"] = x.Diferencias.ToString("N0")
                }
            }).ToList();

            CompletarPaginacion(vm, totalRegistros, totalGeneral, pagina, registrosPorPagina);

            return vm;
        }
        private async Task<CentroReportesViewModel> ReporteGastosRegistradosCierreAsync(
    CentroReportesViewModel vm,
    CentroReportesFiltroViewModel filtros,
    int pagina,
    int registrosPorPagina)
        {
            vm.TituloReporte = "Gastos registrados en cierre";

            var desde = filtros.FechaInicial?.Date;
            var hasta = filtros.FechaFinal?.Date.AddDays(1);

            var query = _context.CierreCajas
                .AsNoTracking()
                .Where(c =>
                    (c.GastosEfectivo != null && c.GastosEfectivo != 0) ||
                    (c.GastosTransferencia != null && c.GastosTransferencia != 0)
                )
                .AsQueryable();

            if (desde.HasValue)
                query = query.Where(c => c.Fecha >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(c => c.Fecha < hasta.Value);

            if (filtros.CedulaVendedor.HasValue)
                query = query.Where(c => c.Cedula == filtros.CedulaVendedor.Value);

            if (filtros.IdSede.HasValue)
                query = query.Where(c => c.IdSede == filtros.IdSede.Value);

            if (filtros.InfopdvId.HasValue)
                query = query.Where(c => c.InfopdvId == filtros.InfopdvId.Value);

            var totalRegistros = await query.CountAsync();
            var totalGeneral = await query.SumAsync(c =>
                (decimal?)((c.GastosEfectivo ?? 0) + (c.GastosTransferencia ?? 0))
            ) ?? 0;

            var datos = await query
                .OrderByDescending(c => c.Fecha)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .Select(c => new
                {
                    c.Fecha,
                    c.Cedula,
                    c.NombreSede,
                    c.NombrePdv,
                    c.GastosEfectivo,
                    c.GastosTransferencia,
                    c.Concepto
                })
                .ToListAsync();

            vm.Columnas = new List<string>
    {
        "Fecha",
        "Empleado",
        "Sede",
        "PDV",
        "Gasto efectivo",
        "Gasto transferencia",
        "Total gasto",
        "Concepto"
    };

            vm.Filas = datos.Select(x =>
            {
                var totalGasto = (x.GastosEfectivo ?? 0) + (x.GastosTransferencia ?? 0);

                return new CentroReportesFilaViewModel
                {
                    Valores = new Dictionary<string, string>
                    {
                        ["Fecha"] = x.Fecha.ToString("dd/MM/yyyy HH:mm"),
                        ["Empleado"] = x.Cedula.ToString(),
                        ["Sede"] = x.NombreSede ?? "",
                        ["PDV"] = x.NombrePdv ?? "",
                        ["Gasto efectivo"] = (x.GastosEfectivo ?? 0).ToString("N0"),
                        ["Gasto transferencia"] = (x.GastosTransferencia ?? 0).ToString("N0"),
                        ["Total gasto"] = totalGasto.ToString("N0"),
                        ["Concepto"] = x.Concepto ?? ""
                    }
                };
            }).ToList();

            CompletarPaginacion(vm, totalRegistros, totalGeneral, pagina, registrosPorPagina);

            return vm;
        }
        private async Task<CentroReportesViewModel> ReporteUtilidadPorProductoAsync(
    CentroReportesViewModel vm,
    CentroReportesFiltroViewModel filtros,
    int pagina,
    int registrosPorPagina)
        {
            vm.TituloReporte = "Utilidad por producto";

            var desde = filtros.FechaInicial?.Date;
            var hasta = filtros.FechaFinal?.Date.AddDays(1);

            var query =
                from pedido in _context.Pedidos.AsNoTracking()
                join producto in _context.Productos.AsNoTracking()
                    on pedido.Codigo equals producto.Cod_Producto into productoJoin
                from producto in productoJoin.DefaultIfEmpty()
                select new
                {
                    pedido,
                    producto
                };

            if (desde.HasValue)
                query = query.Where(x => x.pedido.FechaRegistro >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(x => x.pedido.FechaRegistro < hasta.Value);

            if (filtros.InfopdvId.HasValue)
                query = query.Where(x => x.pedido.InfopdvId == filtros.InfopdvId.Value);

            if (!string.IsNullOrWhiteSpace(filtros.Concepto))
            {
                var concepto = filtros.Concepto.Trim();

                query = query.Where(x =>
                    (x.pedido.Codigo != null && x.pedido.Codigo.Contains(concepto)) ||
                    (x.producto != null && x.producto.NombreProducto.Contains(concepto))
                );
            }

            var agrupado = await query
                .GroupBy(x => new
                {
                    Codigo = x.pedido.Codigo,
                    Producto = x.producto != null ? x.producto.NombreProducto : ""
                })
                .Select(g => new
                {
                    g.Key.Codigo,
                    g.Key.Producto,
                    CantidadVendida = g.Sum(x => x.pedido.Stock),
                    TotalVenta = g.Sum(x => x.pedido.SubTotal),
                    CostoTotal = g.Sum(x => x.pedido.VUnidad ?? 0)
                })
                .OrderByDescending(x => x.TotalVenta - x.CostoTotal)
                .ToListAsync();

            var totalRegistros = agrupado.Count;
            var totalGeneral = agrupado.Sum(x => x.TotalVenta - x.CostoTotal);

            var datos = agrupado
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToList();

            vm.Columnas = new List<string>
    {
        "Código",
        "Producto",
        "Cantidad vendida",
        "Total venta",
        "Costo total",
        "Utilidad",
        "Margen %"
    };

            vm.Filas = datos.Select(x =>
            {
                var utilidad = x.TotalVenta - x.CostoTotal;
                var margen = x.TotalVenta > 0 ? (utilidad / x.TotalVenta) * 100 : 0;

                return new CentroReportesFilaViewModel
                {
                    Valores = new Dictionary<string, string>
                    {
                        ["Código"] = x.Codigo ?? "",
                        ["Producto"] = x.Producto ?? "",
                        ["Cantidad vendida"] = x.CantidadVendida.ToString("N2"),
                        ["Total venta"] = x.TotalVenta.ToString("N0"),
                        ["Costo total"] = x.CostoTotal.ToString("N0"),
                        ["Utilidad"] = utilidad.ToString("N0"),
                        ["Margen %"] = margen.ToString("N2") + "%"
                    }
                };
            }).ToList();

            CompletarPaginacion(vm, totalRegistros, totalGeneral, pagina, registrosPorPagina);

            return vm;
        }
        private async Task<CentroReportesViewModel> ReporteUtilidadPorVendedorAsync(
    CentroReportesViewModel vm,
    CentroReportesFiltroViewModel filtros,
    int pagina,
    int registrosPorPagina)
        {
            vm.TituloReporte = "Utilidad por vendedor";

            var desde = filtros.FechaInicial?.Date;
            var hasta = filtros.FechaFinal?.Date.AddDays(1);

            var query = _context.Pedidos
                .AsNoTracking()
                .Include(p => p.Venta)
                .AsQueryable();

            if (desde.HasValue)
                query = query.Where(p => p.FechaRegistro >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(p => p.FechaRegistro < hasta.Value);

            if (filtros.CedulaVendedor.HasValue)
                query = query.Where(p => p.Venta.Cedula == filtros.CedulaVendedor.Value);

            if (filtros.InfopdvId.HasValue)
                query = query.Where(p => p.InfopdvId == filtros.InfopdvId.Value);

            var agrupado = await query
                .GroupBy(p => p.Venta.Cedula)
                .Select(g => new
                {
                    Vendedor = g.Key,
                    CantidadProductos = g.Sum(x => x.Stock),
                    TotalVenta = g.Sum(x => x.SubTotal),
                    CostoTotal = g.Sum(x => x.VUnidad ?? 0)
                })
                .OrderByDescending(x => x.TotalVenta - x.CostoTotal)
                .ToListAsync();

            var totalRegistros = agrupado.Count;
            var totalGeneral = agrupado.Sum(x => x.TotalVenta - x.CostoTotal);

            var datos = agrupado
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToList();

            vm.Columnas = new List<string>
    {
        "Vendedor",
        "Cantidad productos",
        "Total venta",
        "Costo total",
        "Utilidad",
        "Margen %"
    };

            vm.Filas = datos.Select(x =>
            {
                var utilidad = x.TotalVenta - x.CostoTotal;
                var margen = x.TotalVenta > 0 ? (utilidad / x.TotalVenta) * 100 : 0;

                return new CentroReportesFilaViewModel
                {
                    Valores = new Dictionary<string, string>
                    {
                        ["Vendedor"] = x.Vendedor.ToString(),
                        ["Cantidad productos"] = x.CantidadProductos.ToString("N2"),
                        ["Total venta"] = x.TotalVenta.ToString("N0"),
                        ["Costo total"] = x.CostoTotal.ToString("N0"),
                        ["Utilidad"] = utilidad.ToString("N0"),
                        ["Margen %"] = margen.ToString("N2") + "%"
                    }
                };
            }).ToList();

            CompletarPaginacion(vm, totalRegistros, totalGeneral, pagina, registrosPorPagina);

            return vm;
        }
        private async Task<CentroReportesViewModel> ReporteUtilidadPorPdvAsync(
    CentroReportesViewModel vm,
    CentroReportesFiltroViewModel filtros,
    int pagina,
    int registrosPorPagina)
        {
            vm.TituloReporte = "Utilidad por PDV";

            var desde = filtros.FechaInicial?.Date;
            var hasta = filtros.FechaFinal?.Date.AddDays(1);

            var query = _context.Pedidos
                .AsNoTracking()
                .AsQueryable();

            if (desde.HasValue)
                query = query.Where(p => p.FechaRegistro >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(p => p.FechaRegistro < hasta.Value);

            if (filtros.InfopdvId.HasValue)
                query = query.Where(p => p.InfopdvId == filtros.InfopdvId.Value);

            var agrupado = await query
                .GroupBy(p => p.InfopdvId)
                .Select(g => new
                {
                    InfopdvId = g.Key,
                    CantidadProductos = g.Sum(x => x.Stock),
                    TotalVenta = g.Sum(x => x.SubTotal),
                    CostoTotal = g.Sum(x => x.VUnidad ?? 0)
                })
                .OrderByDescending(x => x.TotalVenta - x.CostoTotal)
                .ToListAsync();

            var pdvs = await _context.Infopdv
                .AsNoTracking()
                .ToDictionaryAsync(x => x.InfopdvId, x => x.NombreInfoPDV ?? "");

            var totalRegistros = agrupado.Count;
            var totalGeneral = agrupado.Sum(x => x.TotalVenta - x.CostoTotal);

            var datos = agrupado
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToList();

            vm.Columnas = new List<string>
    {
        "PDV",
        "Nombre PDV",
        "Cantidad productos",
        "Total venta",
        "Costo total",
        "Utilidad",
        "Margen %"
    };

            vm.Filas = datos.Select(x =>
            {
                var utilidad = x.TotalVenta - x.CostoTotal;
                var margen = x.TotalVenta > 0 ? (utilidad / x.TotalVenta) * 100 : 0;

                return new CentroReportesFilaViewModel
                {
                    Valores = new Dictionary<string, string>
                    {
                        ["PDV"] = x.InfopdvId.ToString(),
                        ["Nombre PDV"] = pdvs.ContainsKey(x.InfopdvId) ? pdvs[x.InfopdvId] : "",
                        ["Cantidad productos"] = x.CantidadProductos.ToString("N2"),
                        ["Total venta"] = x.TotalVenta.ToString("N0"),
                        ["Costo total"] = x.CostoTotal.ToString("N0"),
                        ["Utilidad"] = utilidad.ToString("N0"),
                        ["Margen %"] = margen.ToString("N2") + "%"
                    }
                };
            }).ToList();

            CompletarPaginacion(vm, totalRegistros, totalGeneral, pagina, registrosPorPagina);

            return vm;
        }
        private async Task<CentroReportesViewModel> ReporteUtilidadPorDiaAsync(
    CentroReportesViewModel vm,
    CentroReportesFiltroViewModel filtros,
    int pagina,
    int registrosPorPagina)
        {
            vm.TituloReporte = "Utilidad por día";

            var desde = filtros.FechaInicial?.Date;
            var hasta = filtros.FechaFinal?.Date.AddDays(1);

            var query = _context.Pedidos
                .AsNoTracking()
                .AsQueryable();

            if (desde.HasValue)
                query = query.Where(p => p.FechaRegistro >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(p => p.FechaRegistro < hasta.Value);

            if (filtros.InfopdvId.HasValue)
                query = query.Where(p => p.InfopdvId == filtros.InfopdvId.Value);

            var agrupado = await query
                .GroupBy(p => p.FechaRegistro.Date)
                .Select(g => new
                {
                    Fecha = g.Key,
                    CantidadProductos = g.Sum(x => x.Stock),
                    TotalVenta = g.Sum(x => x.SubTotal),
                    CostoTotal = g.Sum(x => x.VUnidad ?? 0)
                })
                .OrderByDescending(x => x.Fecha)
                .ToListAsync();

            var totalRegistros = agrupado.Count;
            var totalGeneral = agrupado.Sum(x => x.TotalVenta - x.CostoTotal);

            var datos = agrupado
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToList();

            vm.Columnas = new List<string>
    {
        "Fecha",
        "Cantidad productos",
        "Total venta",
        "Costo total",
        "Utilidad",
        "Margen %"
    };

            vm.Filas = datos.Select(x =>
            {
                var utilidad = x.TotalVenta - x.CostoTotal;
                var margen = x.TotalVenta > 0 ? (utilidad / x.TotalVenta) * 100 : 0;

                return new CentroReportesFilaViewModel
                {
                    Valores = new Dictionary<string, string>
                    {
                        ["Fecha"] = x.Fecha.ToString("dd/MM/yyyy"),
                        ["Cantidad productos"] = x.CantidadProductos.ToString("N2"),
                        ["Total venta"] = x.TotalVenta.ToString("N0"),
                        ["Costo total"] = x.CostoTotal.ToString("N0"),
                        ["Utilidad"] = utilidad.ToString("N0"),
                        ["Margen %"] = margen.ToString("N2") + "%"
                    }
                };
            }).ToList();

            CompletarPaginacion(vm, totalRegistros, totalGeneral, pagina, registrosPorPagina);

            return vm;
        }
    }
}
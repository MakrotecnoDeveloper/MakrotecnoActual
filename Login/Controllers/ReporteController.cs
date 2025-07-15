using Microsoft.AspNetCore.Mvc;
using Plataforma.Servicios.Contrato;
using ClosedXML.Excel;

namespace Plataforma.Controllers
{
    public class ReporteController : Controller
    {
        private readonly IReporteService _reporteservice;

        public ReporteController(IReporteService reporteservice)
        {
            _reporteservice = reporteservice;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GenerarReporte(DateTime fechaInicio, DateTime fechaFin, string tipoReporte)
        {
            try
            {
                var resultados = await _reporteservice.GenerarReporte(fechaInicio, fechaFin, tipoReporte);
                return Json(resultados);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        public IActionResult GenerarExcel(DateTime fechaInicio, DateTime fechaFin, string tipoReporte)
        {
            var reporte = _reporteservice.GenerarReporte(fechaInicio, fechaFin, tipoReporte).Result;

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Reporte");

                // Encabezados
                worksheet.Cell(1, 1).Value = "Fecha Factura";
                worksheet.Cell(1, 2).Value = "Total Ventas";
                worksheet.Cell(1, 3).Value = "Total Ganancias";

                int currentRow = 2;

                foreach (var item in reporte)
                {
                    worksheet.Cell(currentRow, 1).Value = item.FechaFactura;
                    worksheet.Cell(currentRow, 1).Style.DateFormat.Format = "dd/MM/yyyy";

                    switch (tipoReporte)
                    {
                        case "ventaTotal":
                            worksheet.Cell(currentRow, 2).Value = item.TotalVentas;
                            break;
                        case "ventaMakrotecno":
                            worksheet.Cell(currentRow, 2).Value = item.TotalMakrotecno;
                            break;
                        case "netoMakrotecno":
                            worksheet.Cell(currentRow, 2).Value = item.TotalNetoMakrotecno;
                            break;
                        case "ventaRecargas":
                            worksheet.Cell(currentRow, 2).Value = item.TotalRecargas;
                            break;
                        case "ventaTienda":
                            worksheet.Cell(currentRow, 2).Value = item.TotalTienda;
                            break;
                        case "gananciaMakrotecno":
                            worksheet.Cell(currentRow, 3).Value = item.TotalGananciaMakrotecno;
                            break;
                        case "gananciaTotal":
                            worksheet.Cell(currentRow, 3).Value = item.TotalGananciaTotal;
                            break;
                        case "gananciaMaria":
                            worksheet.Cell(currentRow, 3).Value = item.TotalGananciaMaria;
                            break;
                        case "gananciaVictor":
                            worksheet.Cell(currentRow, 3).Value = item.TotalGananciaVictor;
                            break;
                        case "gananciaTeresa":
                            worksheet.Cell(currentRow, 3).Value = item.TotalGananciaTeresa;
                            break;
                        default:
                            break;
                    }

                    currentRow++;
                }

                // Totales
                worksheet.Cell(currentRow, 1).Value = "Total:";
                worksheet.Cell(currentRow, 2).FormulaA1 = $"SUM(B2:B{currentRow - 1})";
                worksheet.Cell(currentRow, 3).FormulaA1 = $"SUM(C2:C{currentRow - 1})";

                // Estilo total
                worksheet.Range(currentRow, 1, currentRow, 3).Style.Font.Bold = true;

                // Ajustar ancho
                worksheet.Columns().AdjustToContents();

                    var stream = new MemoryStream();
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    var fileName = "reporte.xlsx";

                    return File(stream, contentType, fileName);
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Plataforma.Servicios.Contrato;
using OfficeOpenXml;

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

            var stream = new MemoryStream();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Reporte");

                // Encabezados de las columnas
                worksheet.Cells[1, 1].Value = "Fecha Factura";
                worksheet.Cells[1, 2].Value = "Total Ventas";
                worksheet.Cells[1, 3].Value = "Total Ganancias";

                // Obtener el índice de la última fila de datos
                int lastRow = reporte.Count + 1;

                // Escribir la fórmula de suma en la primera celda de la fila siguiente a los datos
                worksheet.Cells[lastRow + 1, 1].Value = "Total:";
                worksheet.Cells[lastRow + 1, 2].Formula = $"SUM(B2:B{lastRow})";
                worksheet.Cells[lastRow + 1, 3].Formula = $"SUM(C2:C{lastRow})";

                // Establecer un estilo para la fila de total
                using (ExcelRange range = worksheet.Cells[lastRow + 1, 1, lastRow + 1, 3])
                {
                    range.Style.Font.Bold = true;
                }

                // Ajustar automáticamente el ancho de la columna para que se ajuste al contenido de la fórmula
                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();

                // Llenar el contenido del reporte
                for (int i = 0; i < reporte.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = reporte[i].FechaFactura;
                    worksheet.Cells[i + 2, 1].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Column(1).AutoFit();

                    switch (tipoReporte)
                    {
                        case "ventaTotal":
                            worksheet.Cells[i + 2, 2].Value = reporte[i].TotalVentas;
                            break;
                        case "ventaMakrotecno":
                            worksheet.Cells[i + 2, 2].Value = reporte[i].TotalMakrotecno;
                            break;
                        case "netoMakrotecno":
                            worksheet.Cells[i + 2, 2].Value = reporte[i].TotalNetoMakrotecno;
                            break;
                        case "ventaRecargas":
                            worksheet.Cells[i + 2, 2].Value = reporte[i].TotalRecargas;
                            break;
                        case "ventaTienda":
                            worksheet.Cells[i + 2, 2].Value = reporte[i].TotalTienda;
                            break;
                        case "gananciaMakrotecno":
                            worksheet.Cells[i + 2, 3].Value = reporte[i].TotalGananciaMakrotecno;
                            break;
                        case "gananciaTotal":
                            worksheet.Cells[i + 2, 3].Value = reporte[i].TotalGananciaTotal;
                            break;
                        case "gananciaMaria":
                            worksheet.Cells[i + 2, 3].Value = reporte[i].TotalGananciaMaria;
                            break;
                        case "gananciaVictor":
                            worksheet.Cells[i + 2, 3].Value = reporte[i].TotalGananciaVictor;
                            break;
                        case "gananciaTeresa":
                            worksheet.Cells[i + 2, 3].Value = reporte[i].TotalGananciaTeresa;
                            break;
                        // Agrega más casos según los tipos de reporte necesarios
                        default:
                            // Maneja el caso de tipo de reporte no válido
                            break;
                    }
                }

                package.Save();
            }

            stream.Position = 0;
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = "reporte.xlsx";

            return File(stream, contentType, fileName);
        }
    }
}
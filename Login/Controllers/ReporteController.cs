using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using Plataforma.Servicios.Implementacion;

namespace Plataforma.Controllers
{
    public class ReporteController : Controller
    {
        private readonly IReporteService _reporteService;
        private readonly BaseAdmContext _dbContext;
        public ReporteController(IReporteService reporteService, BaseAdmContext dbContext)
        {
            _reporteService = reporteService;
            _dbContext = dbContext;
        }
        [Authorize]
        public IActionResult Index(DateTime? fechaInicio, DateTime? fechaFin, int? idServicio)
        {
            // Si no viene rango, usamos hoy
            var inicio = fechaInicio ?? DateTime.Today;
            var fin = fechaFin ?? DateTime.Today;

            var servicios = _reporteService.TraerServiciosUnicos(inicio, fin);

            ViewBag.Servicios = servicios;
            ViewBag.FechaInicio = inicio;
            ViewBag.FechaFin = fin;

            if (idServicio.HasValue)
            {
                var (totalSub, totalNeto) = _reporteService.TraerTotalesPorRangoYServicio(inicio, fin, idServicio.Value);
                ViewBag.TotalSub = totalSub;
                ViewBag.TotalNeto = totalNeto;
                ViewBag.ServicioSeleccionado = idServicio.Value;
            }

            return View();
        }
        [HttpGet]
        public IActionResult GetServicios(DateTime fechaInicio, DateTime fechaFin)
        {
            var servicios = _reporteService.TraerServiciosUnicos(fechaInicio, fechaFin);

            return Json(servicios.Select(s => new
            {
                id = s.IdServicio,
                nombre = s.NombreServicio
            }));
        }
        [Authorize]
        public async Task<IActionResult> Diario()
        {
            var reporte = await _reporteService.GenerarReporteDelDiaAsync(DateTime.Today);
            return View(reporte);
        }
        public IActionResult ReporteUtilidad(DateTime fechaInicio, DateTime fechaFin)
        {
            var datos = _reporteService.TraerUtilidadPorServicios(fechaInicio, fechaFin);
            return View(datos);
        }
    }
}
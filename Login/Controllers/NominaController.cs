using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using Plataforma.Servicios.Implementacion;
using System.Diagnostics.Contracts;

namespace Plataforma.Controllers
{
    public class NominaController : Controller
    {
        private readonly INominaService _nominaService;
        private readonly BaseAdmContext _dbContext;
        public NominaController(INominaService nominaService, BaseAdmContext dbContext)
        {
            _nominaService = nominaService;
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            var servicios = _nominaService.ObtenerServicios();
            return View(servicios);
        }
        [HttpPost]
        public async Task<IActionResult> CrearContrato(int Cedula, string TipoContrato, int SalarioBase, DateTime FechaInicio, DateTime FechaFin, bool Estado)
        {
            await _nominaService.CrearContratoAsync(Cedula, TipoContrato, SalarioBase, FechaInicio, FechaFin, Estado);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CrearConceptosNomina(string Nombre, string Tipo, string ServicioAsociado, decimal Porcentaje, decimal ValorFijo)
        {
            await _nominaService.CrearConceptoNominaAsync(Nombre, Tipo, ServicioAsociado, Porcentaje, ValorFijo);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AsociarConcepto(int Cedula, int IdConcepto)
        {
            await _nominaService.AsociarConceptoAsync(Cedula, IdConcepto);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Liquidar(int cedulaEmpleado, DateTime fechaInicio, DateTime fechaFin)
        {
            var liquidacion = await _nominaService.LiquidarPeriodoAsync(cedulaEmpleado, fechaInicio, fechaFin);
            return View("Liquidacion", liquidacion);
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using Plataforma.Servicios.Implementacion;

namespace Plataforma.Controllers
{
    public class MetodoPagoController : Controller
    {
        private readonly IMetodoPagoService _metodopagoService;
        public MetodoPagoController(IMetodoPagoService metodopagoService)
        {
            _metodopagoService = metodopagoService;
        }
        [HttpPost]
        public async Task<IActionResult> CrearCxc(int idVenta, int idCliente, decimal total)
        {
            var cxc = await _metodopagoService.CrearCuentaPorCobrarAsync(idVenta, idCliente, total);
            return Json(new { ok = true, idCxc = cxc.IdCxc });
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarPago(int idCxc, decimal monto, string observacion)
        {
            var ok = await _metodopagoService.RegistrarPagoCxcAsync(idCxc, monto, observacion);
            return Json(new { ok });
        }

        [HttpGet]
        public async Task<IActionResult> DetalleCxc(int idVenta)
        {
            var cxc = await _metodopagoService.ObtenerCxcPorVentaAsync(idVenta);
            if (cxc == null) return NotFound();

            return View(cxc); // Muestra detalle de la cuenta y sus pagos
        }

    }
}
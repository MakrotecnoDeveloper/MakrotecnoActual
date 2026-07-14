using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Servicios.Contrato;
using Plataforma.ViewModels.CuentasCobro;

namespace Plataforma.Controllers;

[Authorize]
public class CuentaCobroController : Controller
{
    private readonly ICuentaCobroService _cuentaCobroService;

    public CuentaCobroController(ICuentaCobroService cuentaCobroService)
    {
        _cuentaCobroService = cuentaCobroService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var vm = await _cuentaCobroService.ObtenerCuentasCobroAsync(User);
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> CrearDesdeFactura(int idFactura)
    {
        try
        {
            var vm = await _cuentaCobroService.ConstruirDesdeFacturaAsync(idFactura, User);
            return View(vm);
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Index", "Pedido");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearDesdeFactura(CuentaCobroCrearVm vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        try
        {
            var id = await _cuentaCobroService.CrearDesdeFacturaAsync(vm, User);
            TempData["Ok"] = "Cuenta de cobro generada correctamente.";
            return RedirectToAction(nameof(Detalle), new { id });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return View(vm);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Detalle(int id)
    {
        var vm = await _cuentaCobroService.ObtenerDetalleAsync(id, User);

        if (vm == null)
        {
            TempData["Error"] = "La cuenta de cobro no fue encontrada.";
            return RedirectToAction(nameof(Index));
        }

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Imprimir(int id)
    {
        var vm = await _cuentaCobroService.ObtenerDetalleAsync(id, User);

        if (vm == null)
        {
            TempData["Error"] = "La cuenta de cobro no fue encontrada.";
            return RedirectToAction(nameof(Index));
        }

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarcarEnviada(int id)
    {
        await _cuentaCobroService.CambiarEstadoAsync(id, "Enviada", User);
        return RedirectToAction(nameof(Detalle), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarcarPagada(int id)
    {
        await _cuentaCobroService.CambiarEstadoAsync(id, "Pagada", User);
        return RedirectToAction(nameof(Detalle), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Anular(int id)
    {
        await _cuentaCobroService.CambiarEstadoAsync(id, "Anulada", User);
        return RedirectToAction(nameof(Detalle), new { id });
    }
}
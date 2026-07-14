using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Servicios.Contrato;
using Plataforma.ViewModels.Cotizacion;

namespace Plataforma.Controllers;

[Authorize]
public class CotizacionController : Controller
{
    private readonly ICotizacionService _cotizacionService;

    public CotizacionController(ICotizacionService cotizacionService)
    {
        _cotizacionService = cotizacionService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? estado, string? buscar)
    {
        var vm = await _cotizacionService.ObtenerCotizacionesAsync(User, estado, buscar);
        ViewBag.Estado = estado;
        ViewBag.Buscar = buscar;
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Crear()
    {
        var vm = await _cotizacionService.ConstruirCrearVmAsync(User);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CotizacionCrearVm vm)
    {
        if (!ModelState.IsValid)
        {
            var nuevoVm = await _cotizacionService.ConstruirCrearVmAsync(User);
            vm.Clientes = nuevoVm.Clientes;
            vm.NumeroCotizacion = nuevoVm.NumeroCotizacion;
            return View(vm);
        }

        try
        {
            var id = await _cotizacionService.CrearCotizacionAsync(vm, User);
            TempData["Ok"] = "Cotización creada correctamente.";
            return RedirectToAction(nameof(Detalle), new { id });
        }
        catch (Exception ex)
        {
            var nuevoVm = await _cotizacionService.ConstruirCrearVmAsync(User);
            vm.Clientes = nuevoVm.Clientes;
            vm.NumeroCotizacion = nuevoVm.NumeroCotizacion;

            ModelState.AddModelError("", ex.Message);
            return View(vm);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Detalle(int id)
    {
        var vm = await _cotizacionService.ObtenerDetalleAsync(id, User);

        if (vm == null)
        {
            TempData["Error"] = "La cotización no fue encontrada.";
            return RedirectToAction(nameof(Index));
        }

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistrarSeguimiento(CotizacionSeguimientoVm vm)
    {
        try
        {
            await _cotizacionService.RegistrarSeguimientoAsync(vm, User);
            TempData["Ok"] = "Seguimiento registrado correctamente.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Detalle), new { id = vm.IdCotizacion });
    }

    [HttpGet]
    public async Task<IActionResult> ConvertirVenta(int id)
    {
        try
        {
            var vm = await _cotizacionService.ObtenerParaConvertirVentaAsync(id, User);

            if (vm == null)
            {
                TempData["Error"] = "La cotización no fue encontrada.";
                return RedirectToAction(nameof(Index));
            }

            return View(vm);
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Detalle), new { id });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConvertirVenta(CotizacionConvertirVentaVm vm)
    {
        try
        {
            var idVenta = await _cotizacionService.ConvertirCotizacionAVentaAsync(vm, User);

            TempData["Ok"] = "Cotización convertida en venta correctamente.";

            return RedirectToAction("DetalleVenta", "Pedido", new { idVenta });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;

            var recargar = await _cotizacionService.ObtenerParaConvertirVentaAsync(vm.IdCotizacion, User);
            return View(recargar ?? vm);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Anular(int id, string observacion)
    {
        try
        {
            await _cotizacionService.AnularCotizacionAsync(id, observacion, User);
            TempData["Ok"] = "Cotización anulada correctamente.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Imprimir(int id)
    {
        var vm = await _cotizacionService.ObtenerDetalleAsync(id, User);

        if (vm == null)
        {
            TempData["Error"] = "La cotización no fue encontrada.";
            return RedirectToAction(nameof(Index));
        }

        return View(vm);
    }
}
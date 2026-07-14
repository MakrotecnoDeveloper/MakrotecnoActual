using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Servicios.Contrato;
using Plataforma.ViewModels.CuentasBancarias;

namespace Plataforma.Controllers;

[Authorize]
public class CuentaBancariaEmpresaController : Controller
{
    private readonly ICuentaBancariaEmpresaService _cuentaBancariaService;

    public CuentaBancariaEmpresaController(ICuentaBancariaEmpresaService cuentaBancariaService)
    {
        _cuentaBancariaService = cuentaBancariaService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var vm = await _cuentaBancariaService.ObtenerTodasAsync(User);
        return View(vm);
    }

    [HttpGet]
    public IActionResult Crear()
    {
        return View(new CuentaBancariaEmpresaVm
        {
            Estado = true
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CuentaBancariaEmpresaVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            await _cuentaBancariaService.CrearAsync(vm, User);
            TempData["Ok"] = "Cuenta bancaria registrada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(vm);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var vm = await _cuentaBancariaService.ObtenerPorIdAsync(id, User);

        if (vm == null)
        {
            TempData["Error"] = "La cuenta bancaria no fue encontrada.";
            return RedirectToAction(nameof(Index));
        }

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(CuentaBancariaEmpresaVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            var ok = await _cuentaBancariaService.ActualizarAsync(vm, User);

            if (!ok)
            {
                TempData["Error"] = "No fue posible actualizar la cuenta bancaria.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Ok"] = "Cuenta bancaria actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(vm);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id)
    {
        await _cuentaBancariaService.CambiarEstadoAsync(id, User);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarcarPrincipal(int id)
    {
        await _cuentaBancariaService.MarcarPrincipalAsync(id, User);
        return RedirectToAction(nameof(Index));
    }
}
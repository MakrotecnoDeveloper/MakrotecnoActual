using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Models.ViewModels.Deporte;
using Plataforma.Services.Interfaces;
using System.Security.Claims;

namespace Plataforma.Controllers;

[Authorize]
public class DeporteController : Controller
{
    private readonly IDeporteService _deporteService;

    public DeporteController(IDeporteService deporteService)
    {
        _deporteService = deporteService;
    }

    [HttpGet]
    public async Task<IActionResult> CronometroResultados(string? cedula, int? idPruebaDeportiva)
    {
        var vm = await _deporteService.ConstruirCronometroResultadosAsync(
            cedula,
            idPruebaDeportiva,
            ObtenerEmpresaId(),
            ObtenerSedeId());

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistrarTiempo(RegistrarTiempoDeportivoVm vm)
    {
        var usuario = ObtenerUsuarioActual();

        var resultado = await _deporteService.RegistrarTiempoDeportivoAsync(
            vm,
            ObtenerEmpresaId(),
            ObtenerSedeId(),
            usuario);

        TempData[resultado.Ok ? "Success" : "Error"] = resultado.Mensaje;

        return RedirectToAction(nameof(CronometroResultados), new
        {
            cedula = vm.Cedula,
            idPruebaDeportiva = vm.IdPruebaDeportiva
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistrarTiemposGrupo(RegistrarTiemposGrupoVm vm)
    {
        var usuario = ObtenerUsuarioActual();

        var resultado = await _deporteService.RegistrarTiemposGrupoAsync(
            vm,
            ObtenerEmpresaId(),
            ObtenerSedeId(),
            usuario);

        TempData[resultado.Ok ? "Success" : "Error"] = resultado.Mensaje;

        return RedirectToAction(nameof(CronometroResultados), new
        {
            idPruebaDeportiva = vm.IdPruebaDeportiva
        });
    }

    [HttpGet]
    public async Task<IActionResult> BuscarDeportistas(string? texto)
    {
        var clientes = await _deporteService.BuscarClientesDeportistasAsync(texto ?? "");
        return Json(clientes);
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerPruebas(int? idDeporte)
    {
        var pruebas = await _deporteService.ObtenerPruebasDeportivasAsync(idDeporte);
        return Json(pruebas);
    }

    private string? ObtenerEmpresaId()
    {
        return User.FindFirst("EmpresaId")?.Value;
    }

    private int? ObtenerSedeId()
    {
        var sedeClaim = User.FindFirst("SedeId")?.Value;
        return int.TryParse(sedeClaim, out var sedeId) ? sedeId : null;
    }
    private string ObtenerUsuarioActual()
    {
        var nombre = User.FindFirst("Nombre")?.Value;
        var apellido = User.FindFirst("Apellido")?.Value;
        var correo = User.FindFirst("Correo")?.Value;
        var cedula = User.FindFirst("Cedula")?.Value;

        var nombreCompleto = $"{nombre} {apellido}".Trim();

        if (!string.IsNullOrWhiteSpace(nombreCompleto))
            return nombreCompleto;

        if (!string.IsNullOrWhiteSpace(correo))
            return correo;

        if (!string.IsNullOrWhiteSpace(cedula))
            return cedula;

        return "sistema";
    }
    [HttpGet]
    public async Task<IActionResult> EstilosDeportivos(
    int? idDeporte,
    string? buscarDeporte,
    string? buscarEstilo,
    int paginaDeportes = 1,
    int paginaEstilos = 1)
    {
        var vm = await _deporteService.ConstruirEstilosDeportivosAdminAsync(
            idDeporte,
            buscarDeporte,
            buscarEstilo,
            paginaDeportes,
            paginaEstilos);

        return View(vm);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearEstiloDeportivo(EstiloDeportivoFormVm vm)
    {
        var resultado = await _deporteService.CrearEstiloDeportivoAsync(vm);

        TempData[resultado.Ok ? "Success" : "Error"] = resultado.Mensaje;

        return RedirectToAction(nameof(EstilosDeportivos), new
        {
            idDeporte = vm.IdDeporte
        });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActualizarEstiloDeportivo(EstiloDeportivoFormVm vm)
    {
        var resultado = await _deporteService.ActualizarEstiloDeportivoAsync(vm);

        TempData[resultado.Ok ? "Success" : "Error"] = resultado.Mensaje;

        return RedirectToAction(nameof(EstilosDeportivos), new
        {
            idDeporte = vm.IdDeporte
        });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstadoEstiloDeportivo(int idPruebaDeportiva, int idDeporte)
    {
        var resultado = await _deporteService.CambiarEstadoEstiloDeportivoAsync(idPruebaDeportiva);

        TempData[resultado.Ok ? "Success" : "Error"] = resultado.Mensaje;

        return RedirectToAction(nameof(EstilosDeportivos), new
        {
            idDeporte
        });
    }
}
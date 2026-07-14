using Microsoft.AspNetCore.Mvc.Rendering;
using Plataforma.Models;
using Plataforma.Models.ViewModels.Deporte;
using System.Threading.Tasks;

namespace Plataforma.Services.Interfaces;

public interface IDeporteService
{
    Task<CronometroResultadosVm> ConstruirCronometroResultadosAsync(
        string? cedula,
        int? idPruebaDeportiva,
        string? idEmpresa,
        int? idSede);

    Task<(bool Ok, string Mensaje)> RegistrarTiempoDeportivoAsync(
        RegistrarTiempoDeportivoVm vm,
        string? idEmpresa,
        int? idSede,
        string usuario);

    Task<(bool Ok, string Mensaje)> RegistrarTiemposGrupoAsync(
        RegistrarTiemposGrupoVm vm,
        string? idEmpresa,
        int? idSede,
        string usuario);

    Task<List<ClienteBusquedaDeportivaVm>> BuscarClientesDeportistasAsync(string texto);

    Task<List<SelectListItem>> ObtenerDeportesAsync();

    Task<List<SelectListItem>> ObtenerPruebasDeportivasAsync(int? idDeporte = null);
    Task<EstilosDeportivosAdminVm> ConstruirEstilosDeportivosAdminAsync(
    int? idDeporte,
    string? buscarDeporte,
    string? buscarEstilo,
    int paginaDeportes,
    int paginaEstilos);

    Task<(bool Ok, string Mensaje)> CrearEstiloDeportivoAsync(EstiloDeportivoFormVm vm);

    Task<(bool Ok, string Mensaje)> ActualizarEstiloDeportivoAsync(EstiloDeportivoFormVm vm);

    Task<(bool Ok, string Mensaje)> CambiarEstadoEstiloDeportivoAsync(int idPruebaDeportiva);
}


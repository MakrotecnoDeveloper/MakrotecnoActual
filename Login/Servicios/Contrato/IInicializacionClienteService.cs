using MakroTecno.Models.ViewModels.InicializacionClientes;

namespace MakroTecno.Services.InicializacionClientes
{
    public interface IInicializacionClienteService
    {
        Task<InicializacionClienteViewModel> ObtenerPanelAsync(string? nit);
        Task<(bool Ok, string Mensaje)> CrearClienteInicialAsync(CrearClienteInicialViewModel model);
        Task<(bool Ok, string Mensaje)> CambiarPlanAsync(string idEmpresa, int idPlan);
        Task<(bool Ok, string Mensaje)> ActivarModuloAsync(string idEmpresa, int idModulo);
        Task<(bool Ok, string Mensaje)> DesactivarModuloAsync(string idEmpresa, int idModulo);
        Task<(bool Ok, string Mensaje)> CrearModuloAsync(CrearModuloViewModel model);
    }
}
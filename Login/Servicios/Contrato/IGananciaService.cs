using Plataforma.Models;

namespace Plataforma.Servicios.Contrato
{
    public interface IGananciaService
    {
        Task<List<Ganancias>> ListarGananciasAsync();
        Task RegistrarGananciaAsync(Ganancias ganancia);
        Task<GananciaViewModel> ObtenerGananciaDelDiaAsync(int cedulaUsuario);
        Task GuardarGananciaDelDiaAsync(GananciaViewModel model);
    }
}

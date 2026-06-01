using MakroTecno.ViewModels.Integraciones;
using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IIntegracionService
    {
        // Tiendas Rappi
        Task<List<RappiTiendaViewModel>> ObtenerTiendasRappiAsync();
        Task<RappiTiendaViewModel> PrepararTiendaRappiAsync(int? id = null);
        Task GuardarTiendaRappiAsync(RappiTiendaViewModel vm);

        // Categorías Rappi
        Task<List<RappiCategoriaMapeoViewModel>> ObtenerCategoriasMapeoAsync();
        Task<RappiCategoriaMapeoViewModel> PrepararCategoriaMapeoAsync(int? id = null);
        Task GuardarCategoriaMapeoAsync(RappiCategoriaMapeoViewModel vm);

        // Productos Rappi
        Task<List<RappiProductoConfigViewModel>> ObtenerProductosConfigRappiAsync();
        Task<RappiProductoConfigViewModel> PrepararProductoConfigAsync(int? id = null);
        Task GuardarProductoConfigAsync(RappiProductoConfigViewModel vm);

        // Exportación
        Task<RappiExportResultadoViewModel> ValidarProductosPersonalizadosRappiAsync(
            int rappiTiendaId,
            bool soloConStock
        );

        Task<RappiExportResultadoViewModel> GenerarExcelProductosPersonalizadosRappiAsync(
            int rappiTiendaId,
            bool soloConStock,
            string usuarioId
        );
        Task<RappiActualizacionResultadoViewModel> ActualizarProductosRappiDesdeExcelAsync(
            int rappiTiendaId,
            IFormFile archivo,
            string usuarioId
        );
        Task<RappiActualizacionResultadoViewModel> PrepararActualizacionProductosRappiAsync();
    }
}
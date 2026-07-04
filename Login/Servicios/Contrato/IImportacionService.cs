// Services/IImportacionService.cs

using Plataforma.Models;

namespace Plataforma.Services
{
    public interface IImportacionService
    {
        Task<ResultadoImportacionDto> ProcesarTextoAsync(
            ImportacionProductosViewModel model,
            string usuario
        );

        Task<ResultadoImportacionDto> ProcesarExcelAsync(
            ImportacionProductosViewModel model,
            string usuario
        );

        Task<ResultadoImportacionDto> ConfirmarImportacionAsync(
            int idImportacion,
            string tipoOperacion,
            List<string> camposSeleccionados
        );

        string LimpiarTexto(string texto);

        string GenerarCodigoProducto(string nombreProducto);

        decimal ConvertirPrecio(string precioTexto);
    }
}
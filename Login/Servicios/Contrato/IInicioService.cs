using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IInicioService
    {
        Task<IReadOnlyList<SeriePuntoDTO>> VentasUltimosDiasAsync(int dias, int? idPdv, CancellationToken ct = default);
        Task<IReadOnlyList<CategoriaValorDTO>> VentasPorServicioAsync(DateTime desde, DateTime hasta, int? idPdv, CancellationToken ct = default);
        Task<IReadOnlyList<OrdenServicioDTO>> OSAbiertasTopAsync(int take, CancellationToken ct = default);
        Task<IReadOnlyList<StockBajoDTO>> StockBajoAsync(int take, int minimo, CancellationToken ct = default);
        Task<IReadOnlyList<CompraDTO>> ComprasRecientesAsync(int take, CancellationToken ct = default);
    }
}

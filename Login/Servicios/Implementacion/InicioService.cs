using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using System.Runtime.InteropServices;
namespace Plataforma.Servicios.Implementacion
{
    public class InicioService : IInicioService
    {
        private readonly BaseAdmContext _dbContext;
        public InicioService(BaseAdmContext dbContext)
        {
            _dbContext = dbContext;
        }
        // Rango local (Bogotá): [00:00 hace N días, 00:00 de mañana)
        private static (DateTime desde, DateTime hasta) RangoUltimosNDiasLocal(int dias)
        {
            var tzId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "SA Pacific Standard Time" : "America/Bogota";
            var tz = TimeZoneInfo.FindSystemTimeZoneById(tzId);
            var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
            var hasta = nowLocal.Date.AddDays(1);
            var desde = hasta.AddDays(-dias);
            return (desde, hasta);
        }

        /// ----------------------------------------------------------------------
        /// 1) Ventas últimos N días (usa FACTURAS emitidas; evita duplicar por join)
        ///    PDV se filtra por existencia de renglones en PEDIDOS con ese IdInfoPDV
        /// ----------------------------------------------------------------------
        public async Task<IReadOnlyList<SeriePuntoDTO>> VentasUltimosDiasAsync(int dias, int? idPdv, CancellationToken ct = default)
        {
            var (desde, hasta) = RangoUltimosNDiasLocal(dias);

            var facturas = _dbContext.Factura
                .AsNoTracking()
                .Where(f => f.FechaEmision >= desde && f.FechaEmision < hasta);

            // filtrar por PDV a través de los PEDIDOS de esa venta
            if (idPdv.HasValue)
            {
                int pdv = idPdv.Value;
                facturas = facturas.Where(f =>
                    _dbContext.Pedidos.Any(p => p.IdVenta == f.IdVenta && p.InfopdvId == pdv));
            }

            var data = await facturas
                .GroupBy(f => f.FechaEmision.Date)
                .Select(g => new SeriePuntoDTO
                {
                    Fecha = g.Key,
                    Total = g.Sum(x => (decimal?)x.Total) ?? 0m
                })
                .OrderBy(x => x.Fecha)
                .ToListAsync(ct);

            return data;
        }

        /// ----------------------------------------------------------------------
        /// 2) Ventas por SERVICIO en un rango:
        ///    Suma por renglón de PEDIDOS (SubTotal o VNeto) SOLO de ventas facturadas
        ///    Joins: Factura(IdVenta) -> Pedidos(IdVenta) -> Producto -> Categoria -> Servicio
        ///    PDV: filtra por Pedidos.IdInfoPDV
        /// ----------------------------------------------------------------------
        public async Task<IReadOnlyList<CategoriaValorDTO>> VentasPorServicioAsync(DateTime desde, DateTime hasta, int? idPdv, CancellationToken ct = default)
        {
            // base: solo líneas de ventas que tienen FACTURA en el rango
            var q = from f in _dbContext.Factura.AsNoTracking()
                    where f.FechaEmision >= desde && f.FechaEmision < hasta
                    join p in _dbContext.Pedidos.AsNoTracking() on f.IdVenta equals p.IdVenta
                    join prod in _dbContext.Productos.AsNoTracking() on p.Codigo equals prod.Cod_Producto
                    join c in _dbContext.CategoriaProductos.AsNoTracking() on prod.IdCatepro equals c.IdCateProducto
                    join s in _dbContext.Servicio.AsNoTracking() on c.IdServicio equals s.IdServicio
                    select new
                    {
                        s.NombreServicio,
                        p.SubTotal,  // o p.VNeto si prefieres neto del renglón
                        p.InfopdvId
                    };

            if (idPdv.HasValue)
            {
                int pdv = idPdv.Value;
                q = q.Where(x => x.InfopdvId == pdv);
            }

            var data = await q
                .GroupBy(x => x.NombreServicio)
                .Select(g => new CategoriaValorDTO
                {
                    Etiqueta = g.Key,
                    Valor = g.Sum(z => (decimal?)z.SubTotal) ?? 0m
                    // Si prefieres neto: Valor = g.Sum(z => (decimal?)z.VNeto) ?? 0m
                })
                .OrderByDescending(x => x.Valor)
                .ToListAsync(ct);

            return data;
        }

        // --------- lo demás queda igual (te los incluyo para completar) ---------

        public async Task<IReadOnlyList<OrdenServicioDTO>> OSAbiertasTopAsync(int take, CancellationToken ct = default)
        {
            var data = await _dbContext.OrdenServicios
                .AsNoTracking()
                .Where(o => o.Estado != "Cerrada" && o.Estado != "Finalizada")
                .OrderByDescending(o => o.FechaIngreso)
                .Take(take)
                .Select(o => new OrdenServicioDTO
                {
                    Id = o.IdOrden,
                    Cliente = (o.Dispositivo.Cliente.NombreCliente) ?? "Sin cliente",
                    Tecnico = o.Cedula,
                    Estado = o.Estado,
                    Fecha = o.FechaIngreso
                })
                .ToListAsync(ct);
            return data;
        }

        public async Task<IReadOnlyList<StockBajoDTO>> StockBajoAsync(int take, int minimo, CancellationToken ct = default)
        {
            var data = await _dbContext.Productos
                .AsNoTracking()
                .Where(p => p.Estado == 1 && p.CantidadProducto <= minimo)
                .OrderBy(p => p.CantidadProducto).ThenBy(p => p.NombreProducto)
                .Take(take)
                .Select(p => new StockBajoDTO
                {
                    Codigo = p.Cod_Producto,
                    Nombre = p.NombreProducto,
                    Stock = p.CantidadProducto,
                    Minimo = minimo
                })
                .ToListAsync(ct);
            return data;
        }

        public async Task<IReadOnlyList<CompraDTO>> ComprasRecientesAsync(int take, CancellationToken ct = default)
        {
            var data = await (
            from c in _dbContext.Compras.AsNoTracking()
            join p in _dbContext.Proveedores.AsNoTracking()
                on c.IdProveedor equals p.IdProveedor into lj
            from p in lj.DefaultIfEmpty()
            orderby c.FechaCompra descending
            select new CompraDTO
            {
                IdCompra = c.IdCompra,
                Proveedor = p != null ? p.RazonSocial : "(Sin proveedor)",
                Fecha = c.FechaCompra,
                Total = c.ValorTotal
            })
            .Take(take)
            .ToListAsync(ct);

                    return data;
         }
    }
}

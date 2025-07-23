using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Servicios.Implementacion
{
    public class ComprasService : IComprasService
    {
        private readonly BaseAdmContext _dbContext;
        public ComprasService(BaseAdmContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Proveedores>> ObtenerProveedoresAsync()
        {
            return await _dbContext.Set<Proveedores>().ToListAsync();
        }

        public async Task<Proveedores> BuscarProveedorPorIdAsync(int idProveedor)
        {
            return await _dbContext.Set<Proveedores>().FindAsync(idProveedor);
        }

        public async Task<List<Producto>> BuscarProductosPorCodigoAsync(string term)
        {
            return await _dbContext.Set<Producto>()
                                 .Where(p => p.Cod_Producto.Contains(term))
                                 .ToListAsync();
        }

        public async Task InsertarCompraAsync(Compras compra, List<DetalleCompra> detalles)
        {
            _dbContext.Set<Compras>().Add(compra);
            await _dbContext.SaveChangesAsync(); // Para obtener IdCompra generado

            foreach (var detalle in detalles)
            {
                detalle.IdCompra = compra.IdCompras;
                _dbContext.Set<DetalleCompra>().Add(detalle);

                var producto = await _dbContext.Set<Producto>().FindAsync(detalle.CodProducto);
                if (producto != null)
                {
                    producto.CantidadProducto += detalle.Cantidad;
                }
            }

            compra.ValorTotal = detalles.Sum(d => d.ValorTotal);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Compras>> ObtenerComprasAsync()
        {
            return await _dbContext.Set<Compras>()
                                 .Include(c => c.Detalles)
                                 .ToListAsync();
        }
    }
}

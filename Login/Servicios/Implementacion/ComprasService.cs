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

        public async Task<List<Producto>> BuscarProductosPorCodigoAsync(string codigo)
        {
            return await _dbContext.Set<Producto>()
                                 .Where(p => p.Cod_Producto.Contains(codigo))
                                 .ToListAsync();
        }

        public async Task<bool> InsertarCompraAsync(CompraViewModel model)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                //Si hay descuento..
                var total = model.Productos.Sum(p => p.VTotal);
                if(model.Iva > 0)
                {
                    total = ((total * model.Iva) / 100) + total;
                }
                // 1. Crear la compra
                var compra = new Compras
                {
                    IdProveedor = model.IdProveedor,
                    ValorTotal = total,
                    FechaCompra = DateTime.Now,
                    Estado = 1,
                    Iva = model.Iva,
                    DescuentoFactura = total - model.DescuentoFactura,
                    CodFacturaExterno = model.CodFacturaExterno
                };

                _dbContext.Compras.Add(compra);
                await _dbContext.SaveChangesAsync(); // Guarda y obtiene IdCompra

                // 2. Crear los detalles
                foreach (var producto in model.Productos)
                {

                    var productoExistente = _dbContext.Productos.FirstOrDefault(p => p.Cod_Producto == producto.Codigo);
                    if(productoExistente != null)
                    {
                        productoExistente.CantidadProducto += producto.Stock;
                        _dbContext.Productos.Update(productoExistente);
                    }

                    var detalle = new DetalleCompra
                    {
                        IdCompra = compra.IdCompra,
                        Codigo = producto.Codigo,
                        Stock = producto.Stock,
                        VNeto = producto.VNeto,
                        VVenta = producto.VVenta,
                        VTotal = producto.VTotal
                    };
                    _dbContext.DetalleCompras.Add(detalle);
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Log ex (implementa tu logger o consola)
                return false;
            }
        }

        public async Task<List<Compras>> ObtenerComprasAsync()
        {
            return await _dbContext.Set<Compras>()
                                 .Include(c => c.Detalles)
                                 .ToListAsync();
        }
    }
}

using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Servicios.Implementacion
{
    public class PedidoService : IPedidoService
    {
        private readonly BaseAdmContext _dbContext;
        public PedidoService(BaseAdmContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> CrearVentaAsync(Ventas venta)
        {
            try
            {
                _dbContext.Ventas.Add(venta);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<List<Ventas>> ObtenerTodasLasVentasAsync()
        {
            return await _dbContext.Ventas
                .OrderByDescending(v => v.FechaVenta)
                .ToListAsync();
        }
        public async Task<bool> AgregarPedidoAVentaAsync(Pedidos pedido)
        {
            try
            {
                _dbContext.Pedidos.Add(pedido);

                // Actualizar el total en la venta
                var venta = await _dbContext.Ventas.FirstOrDefaultAsync(v => v.IdVenta == pedido.IdVenta);
                if (venta != null)
                {
                    venta.Total += (int)pedido.SubTotal;
                }

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task GuardarPedidoAsync(Pedidos pedido)
        {
            _dbContext.Pedidos.Add(pedido);
            await _dbContext.SaveChangesAsync();
        }
        public void ActualizarEstadoFacturas()
        {
            var facturasVencidas = _dbContext.Factura
                .Where(f => f.FechaEmision.AddDays(7) <= DateTime.Now && f.EstadoFactura == "Emitida")
                .ToList();

            foreach (var factura in facturasVencidas)
            {
                factura.EstadoFactura = "Cerrada";
            }

            _dbContext.SaveChanges();
        }
        public List<Factura> ObtenerFacturasFechaDescendente()
        {
            return _dbContext.Factura
                .Include(f => f.Venta)
                .OrderByDescending(f => f.FechaEmision)
                .ToList();
        }
        public async Task<Ventas> ObtenerVentaConPedidos(int idVenta)
        {
            return await _dbContext.Ventas
                .Include(v => v.Pedidos)
                .FirstOrDefaultAsync(v => v.IdVenta == idVenta);
        }
        public async Task<int> GenerarConsecutivoFactura()
        {
            var random = new Random();
            int numeroFactura;

            do
            {
                numeroFactura = random.Next(10000000, 99999999); // 8 dígitos
            } while (await _dbContext.Factura.AnyAsync(f => f.NumeroFactura == numeroFactura));

            return numeroFactura;
        }
        public async Task GuardarFacturaAsync(Factura factura)
        {
            _dbContext.Factura.Add(factura);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("⚠️ Error al guardar: " + ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine("🔍 Inner: " + ex.InnerException.Message);
                throw;
            }
        }
        public async Task<List<Factura>> ObtenerFacturasConVentaCliente()
        {
            return await _dbContext.Factura
                .Include(f => f.Venta)
                .ThenInclude(v => v.Pedidos)
                .OrderByDescending(f => f.FechaEmision)
                .ToListAsync();
        }
        public async Task<Factura> ObtenerFacturaConDetalle(int idFactura)
        {
            return await _dbContext.Factura
                .Include(f => f.Venta)
                .ThenInclude(v => v.Pedidos)
                .FirstOrDefaultAsync(f => f.IdFactura == idFactura);
        }
        public async Task<bool> AnularFacturaAsync(int idFactura)
        {
            var factura = await _dbContext.Factura.FindAsync(idFactura);
            if (factura == null || factura.EstadoFactura == "Anulada")
                return false;

            factura.EstadoFactura = "Anulada";
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> EliminarFacturaAsync(int idFactura)
        {
            var factura = await _dbContext.Factura.FindAsync(idFactura);
            if (factura == null)
                return false;

            _dbContext.Factura.Remove(factura);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<Ventas> ObtenerVentaPorIdAsync(int id)
        {
            return await _dbContext.Ventas.FindAsync(id);
        }

        public async Task ActualizarVentaAsync(Ventas venta)
        {
            _dbContext.Ventas.Update(venta);
            await _dbContext.SaveChangesAsync();
        }

    }
}

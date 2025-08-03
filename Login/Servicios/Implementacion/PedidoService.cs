using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Vml;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using System.Security.Claims;

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
        public async Task<(float valorVenta, float valorNeto)?> BuscarProductoPorCodigoAsync(string codigo)
        {
            Console.WriteLine(codigo);
            var producto = await _dbContext.Productos
                .Where(p => p.Cod_Producto == codigo)
                .Select(p => new
                {
                    p.ValorVentaProducto,
                    p.ValorNetoProducto
                })
                .FirstOrDefaultAsync();

            if (producto == null)
                return null;

            return (producto.ValorVentaProducto, producto.ValorNetoProducto);
        }
        public async Task<decimal> ObtenerCantidadProductoActual(string codigo)
        {
            try
            {
                var cantidad = await _dbContext.Productos.Where(x => x.Cod_Producto == codigo).FirstOrDefaultAsync();
                return cantidad.CantidadProducto;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task GuardarPedidosAsync(List<Pedidos> pedidos, int idVenta, ClaimsPrincipal usuario)
        {
            // 1. Obtener cédula desde el claim
            var cedulaStr = usuario.FindFirst("Cedula")?.Value;
            if (!int.TryParse(cedulaStr, out int cedula))
                throw new Exception("No se pudo obtener la cédula del usuario autenticado.");

            // 2. Consultar IdSede
            var idSede = await _dbContext.Sedeempleado
                .Where(se => se.Cedula == cedula)
                .Select(se => se.Id_sede)
                .FirstOrDefaultAsync();

            if (idSede == 0)
                throw new Exception("No se encontró una sede asociada al usuario.");

            // 3. Consultar InfoPdvId
            var infoPdvId = await _dbContext.Infopdv
                .Where(p => p.Id_Sede == idSede)
                .Select(p => p.InfopdvId)
                .FirstOrDefaultAsync();

            if (infoPdvId == 0)
                throw new Exception("No se encontró un PDV válido para la sede.");

            decimal totalVenta = 0;

            // 4. Completar y guardar los pedidos
            foreach (var pedido in pedidos)
            {

                var productoExistente = _dbContext.Productos.FirstOrDefault(p => p.Cod_Producto == pedido.Codigo);
                if (productoExistente != null)
                {
                    productoExistente.CantidadProducto -= pedido.Stock;
                    _dbContext.Productos.Update(productoExistente);
                }
                //Calcular subtotal de este pedido
                pedido.IdVenta = idVenta;
                pedido.InfopdvId = infoPdvId;
                pedido.FechaRegistro = DateTime.Now;
                var subtotal = pedido.Stock * pedido.VVenta;
                pedido.SubTotal = pedido.Stock * pedido.VVenta;
                _dbContext.Pedidos.Add(pedido);
            }
            

            await _dbContext.SaveChangesAsync();
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
        public async Task GuardarVentaActualizada(Ventas venta, decimal total)
        {
            if (venta == null)
                throw new ArgumentNullException(nameof(venta), "La venta no puede ser nula.");

            if (venta.Pedidos == null || !venta.Pedidos.Any())
                throw new Exception("La venta no tiene productos asociados.");


            // 2. Actualizar el campo Total en la venta
            venta.Total = total;

            // 3. Guardar cambios en la base de datos
            _dbContext.Ventas.Update(venta);
            await _dbContext.SaveChangesAsync();
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
        public async Task ActualizarEstadoVentaAsync(int idVenta, string nuevoEstado)
        {
            var venta = await _dbContext.Ventas.FirstOrDefaultAsync(v => v.IdVenta == idVenta);
            if (venta != null)
            {
                venta.EstadoVenta = nuevoEstado;
                _dbContext.Ventas.Update(venta);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception($"No se encontró la venta con Id {idVenta} para actualizar su estado.");
            }
        }
        public async Task<Factura> ObtenerFacturaConAdicionesAsync(int IdFactura)
        {
            return await _dbContext.Factura
                .Include(f => f.Venta)
                .Include(f => f.Adiciones)
                .FirstOrDefaultAsync(f => f.IdFactura == IdFactura);
        }

        public async Task<bool> AgregarAdicionFacturaAsync(int IdFactura, decimal valor, string descripcion, int cedulaEmpleado, string EstadoAdicion)
        {
            var factura = await _dbContext.Factura.FindAsync(IdFactura);
            if (factura == null)
                return false;

            var adicion = new AdicionFactura
            {
                IdFactura = IdFactura,
                Valor = valor,
                Descripcion = descripcion,
                Fecha = DateTime.Now,
                Cedula = cedulaEmpleado
            };

            if(EstadoAdicion == "Descuento")
            {
                factura.Total -= valor;
            }
            else if(EstadoAdicion == "Aumento")
                {
                factura.Total += valor;
            }
                

            _dbContext.AdicionFacturas.Add(adicion);
            _dbContext.Factura.Update(factura);
            await _dbContext.SaveChangesAsync();

            return true;
        }
        public List<AdicionFactura> ObtenerConceptosCompletos()
        {
            return _dbContext.AdicionFacturas.ToList();
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Plataforma.Servicios.Implementacion
{
    public class PedidoService : IPedidoService
    {
        private readonly BaseAdmContext _dbContext;
        public PedidoService(BaseAdmContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<Factura> ObtenerFacturas()
        {
            return _dbContext.Factura.ToList();
        }
        public void ActualizarEstadoFacturas()
        {
            var facturasCompletadas = _dbContext.Factura
                .Where(f => f.estado == "Pagado" && f.fechaVenta.AddDays(7) <= DateTime.Now)
                .ToList();

            foreach (var factura in facturasCompletadas)
            {
                factura.estado = "Cerrado";
            }

            _dbContext.SaveChanges();
        }
        public IEnumerable<Factura> CrearFactura(int cedula_cliente, int cedula_empleado, DateTime fechaVenta, string estado)
        {
            // Crear una nueva instancia de Empleado
            var nuevaFactura = new Factura
            {
                cedula_cliente = cedula_cliente,
                cedula = cedula_empleado,
                fechaVenta = fechaVenta,
                estado = estado
            };

            // Agregar el nuevo empleado al contexto de la base de datos
            _dbContext.Factura.Add(nuevaFactura);

            // Guardar los cambios en la base de datos
            _dbContext.SaveChanges();

            // Retornar todos los empleados después de agregar el nuevo empleado
            return _dbContext.Factura.ToList();
        }
        public List<string> ObtenerCodigosProductosAutocompletado(string codigo)
        {
            return _dbContext.Productos
                .Where(p => p.Cod_Producto.StartsWith(codigo))
                .Select(p => p.Cod_Producto)
                .ToList();
        }
        public async Task<Producto> ObtenerInfoProductoAsync(string codigoProducto)
        {
            var producto = await _dbContext.Productos.FirstOrDefaultAsync(p => p.Cod_Producto == codigoProducto);
            return producto;
        }
        public async Task<List<Factura>> ObtenerFacturasAsync(int page, int pageSize)
        {
            // Lógica para obtener facturas desde tu base de datos, teniendo en cuenta la paginación
            // Por ejemplo, puedes usar LINQ para aplicar la paginación
            var facturas = await _dbContext.Factura
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return facturas;
        }

        public async Task<List<Factura>> BuscarFacturaPorNumeroAsync(int numeroFactura)
        {
            return await _dbContext.Factura
                .Where(f => f.cod_factura == numeroFactura)
                .ToListAsync();
        }
        public async Task<int> ObtenerCantidadTotalFacturasAsync()
        {
            int totalFacturas = await _dbContext.Factura.CountAsync();
            return totalFacturas;
        }
        public Factura BuscarFacturaPorId(int id)
        {
            // Implementa la lógica para buscar la factura en la base de datos
            return _dbContext.Factura.FirstOrDefault(f => f.cod_factura == id);
        }
        public void InsertarPedido(int codfact, string cod_producto, int stock, int vneto, int vventa, string estado)
        {
            var pedido = new Pedidos
            {
                cod_factura = codfact,
                cod_producto = cod_producto,
                cantidad = stock,
                valorNeto = vneto,
                valorVenta = vventa,
                estado = estado
            };

            _dbContext.Pedidos.Add(pedido);
            _dbContext.SaveChanges();
        }
        public async Task<List<Factura>> VisualizarPedido(string estado)
        {
            return await _dbContext.Factura
                .Where(f => f.estado == estado)
                .ToListAsync();
        }
        public async Task<List<Pedidos>> VisualizarPedidoPorId(int id)
        {
            List<Pedidos> pedidos = await _dbContext.Pedidos.Where(p => p.cod_factura == id).ToListAsync();
            if (pedidos != null && pedidos.Count > 0)
            {
                return pedidos;
            }
            else
            {
                throw new Exception("No se encontró ningún pedido con el ID especificado");
            }
        }
    }
}

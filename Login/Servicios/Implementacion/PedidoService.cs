using Login.Models;
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
        public List<Producto> GetProdutos(string cod_producto)
        {
            // Verifica si el código de producto existe en la base de datos
            var productoExiste = _dbContext.Productos.Any(p => p.Cod_Producto == cod_producto);

            if (!productoExiste)
            {
                return null;
            }

            // Si el código de producto es válido, retorna la lista de productos
            return _dbContext.Productos.Where(p => p.Cod_Producto == cod_producto).ToList();
        }
        public List<Factura> ObtenerFacturasFechaDescendente()
        {
            var facturasOrdenadas = _dbContext.Factura.OrderByDescending(f => f.fechaVenta).ToList();
            return facturasOrdenadas;
        }
        public List<Factura> ObtenerFacturas()
        {
            return _dbContext.Factura.ToList();
        }
        public void ActualizarEstadoFacturas()
        {
            var facturasCompletadas = _dbContext.Factura
                .Where(f => f.estado == "Proceso" && f.fechaVenta.AddDays(7) <= DateTime.Now)
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
        public Sedeempleado BuscarPdvPorCedula(int cedula)
        {
            return _dbContext.Sedeempleado.FirstOrDefault(s => s.cedula == cedula);
        }
        public void InsertarPedido(int codfact, string cod_producto, int stock, int vneto, int vventa, string estado)
        {
            var producto = _dbContext.Productos.FirstOrDefault(p => p.Cod_Producto == cod_producto);
            if (producto != null)
            {
                if (producto.CantidadProducto >= stock)
                {
                    int cantidadRestante = producto.CantidadProducto - stock;
                    vventa = stock * vventa;
                    vneto = stock * vneto;
                    producto.CantidadProducto = cantidadRestante;
                    _dbContext.SaveChanges();
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
                else
                {
                    Console.WriteLine("No hay suficiente stock disponible para este producto.");
                }
            }

        }
        public async Task<List<Factura>> VisualizarPedido(string estado)
        {
            return await _dbContext.Factura
                .Where(f => f.estado == estado)
                .OrderByDescending(f => f.fechaVenta)
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
        public async Task<List<Pedidos>> traerValorProductos(int id)
        {
            var pedidos = await _dbContext.Pedidos
                       .Where(p => p.cod_factura == id)
                       .ToListAsync();
            var vnetoTotal = pedidos.Sum(p => p.valorNeto);
            var vventaTotal = pedidos.Sum(p => p.valorVenta);
            return pedidos;
        }
        public async Task<int> VentaInsertada(int cod_factura, int ventaTotal, int ventaMakrotecno, int netoMakrotecno, int ventaRecarga, int ventaTienda, int ventapasivos)
        {
            var ventas = new Ventas
            {
                cod_factura = cod_factura,
                ventaTotal = ventaTotal,
                ventaMakrotecno = ventaMakrotecno,
                netoMakrotecno = netoMakrotecno,
                ventaRecargas = ventaRecarga,
                ventaTienda = ventaTienda,
                ventaPasivos = ventapasivos
            };

            _dbContext.Ventas.Add(ventas);
            _dbContext.SaveChanges();
            return ventas.id_venta;
        }
        public async Task GananciaInsertada(int id_venta, int gananciaMakrotecno, int gananciaMaria, int gananciaVictor, int gananciaTeresa, int gananciaRecargas, int gananciaTotal)
        {
            var ganancia = new Ganancias
            {
                id_venta = id_venta,
                gananciaMakrotecno = gananciaMakrotecno,
                gananciaTotal = gananciaTotal,
                gananciaMaria = gananciaMaria,
                gananciaVictor = gananciaVictor,
                gananciaTeresa = gananciaTeresa
            };
            _dbContext.Ganancias.Add(ganancia);
            _dbContext.SaveChanges();
        }
        public List<Ganancias> TraerGanancias()
        {
            return _dbContext.Ganancias.ToList();
        }
        public void EliminarPedido(int id)
        {
            var pedidoVerificado = _dbContext.Pedidos.FirstOrDefault(p => p.cod_pedido == id);
            if (pedidoVerificado != null)
            {
                try
                {
                    // 3. Eliminar el producto.
                    _dbContext.Pedidos.Remove(pedidoVerificado);

                    // 4. Guardar los cambios en la base de datos.
                    _dbContext.SaveChanges();

                    Console.WriteLine("Producto eliminado exitosamente.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al eliminar el producto: " + ex.Message);
                    // Puedes agregar un código adicional aquí para manejar el error, como registrar el error en un archivo de registro, notificar al usuario, etc.
                }
            }
            else
            {
                Console.WriteLine("El producto no existe.");
                // Puedes agregar un código adicional aquí si necesitas manejar el caso en que el producto no exista
            }
        }
        public List<Ventas> TraerVentas()
        {
            return _dbContext.Ventas.ToList();
        }
    }
}

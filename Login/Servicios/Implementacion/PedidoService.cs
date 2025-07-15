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
            var facturasOrdenadas = _dbContext.Factura.OrderByDescending(f => f.FechaVenta).ToList();
            return facturasOrdenadas;
        }
        public List<Factura> ObtenerFacturas()
        {
            return _dbContext.Factura.ToList();
        }
        public void ActualizarEstadoFacturas()
        {
            var facturasCompletadas = _dbContext.Factura
                .Where(f => f.Estado == "Proceso" && f.FechaVenta.AddDays(7) <= DateTime.Now)
                .ToList();

            foreach (var factura in facturasCompletadas)
            {
                factura.Estado = "Cerrado";
            }

            _dbContext.SaveChanges();
        }
        public IEnumerable<Factura> CrearFactura(int cedula_cliente, int cedula_empleado, DateTime fechaVenta, string estado, string tpfactura)
        {
            // Crear una nueva instancia de Empleado
            var nuevaFactura = new Factura
            {
                Cedula_cliente = cedula_cliente,
                Cedula = cedula_empleado,
                FechaVenta = fechaVenta,
                Estado = estado,
                TipoFactura = tpfactura
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
                .Where(f => f.Cod_factura == numeroFactura)
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
            return _dbContext.Factura.FirstOrDefault(f => f.Cod_factura == id);
        }
        public int? BuscarIdSedePorCedula(int cedula)
        {
            // Busca el primer registro que coincida con la cédula y devuelve el id_sede
            return _dbContext.Sedeempleado
                .Where(s => s.Cedula == cedula)
                .Select(s => s.Id_sede)
                .FirstOrDefault();
        }
        public int? BuscarIdPDVPorIdSede(int? buscarIdSede)
        {
            return _dbContext.Infopdv
                .Where(s => s.Id_Sede == buscarIdSede)
                .Select(s => s.InfopdvId)
                .FirstOrDefault();
        }
        public void InsertarPedido(int codfact, string cod_producto, decimal stock, int vneto, int vventa, DateTime fechaIngreso, string tpventa, int idpdv)
        {
            var producto = _dbContext.Productos.FirstOrDefault(p => p.Cod_Producto == cod_producto);
            if (producto != null)
            {
                if (producto.CantidadProducto >= stock)
                {
                    decimal cantidadRestante = producto.CantidadProducto - stock;
                    vventa = (int)(stock * vventa);
                    vneto = (int)(stock * vneto);
                    producto.CantidadProducto = cantidadRestante;
                    _dbContext.SaveChanges();
                    var pedido = new Pedidos
                    {
                        Cod_factura = codfact,
                        Cod_producto = cod_producto,
                        Cantidad = stock,
                        ValorNeto = vneto,
                        ValorVenta = vventa,
                        Estado = tpventa,
                        InfopdvId = idpdv,
                        FechaIngreso = fechaIngreso
                    };

                    _dbContext.Pedidos.Add(pedido);
                    try
                    {
                        _dbContext.SaveChanges();
                    }
                    catch (DbUpdateException ex)
                    {
                        Console.WriteLine(ex.InnerException?.Message);
                        throw; // O maneja el error según lo necesites
                    }

                    //Inserccion en la tabla GananciaPedido
                    int codPedidoGenerado = pedido.Cod_pedido;
                    int ganancia = vventa - vneto;
                    var gananciaPedido = new GananciaPedido
                    {
                        Cod_pedido = codPedidoGenerado,
                        Ganancia = ganancia
                    };
                    _dbContext.GananciaPedido.Add(gananciaPedido);
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
                .Where(f => f.Estado == estado)
                .OrderByDescending(f => f.FechaVenta)
                .ToListAsync();
        }
        public async Task<List<Pedidos>> VisualizarPedidoPorId(int id)
        {
            List<Pedidos> pedidos = await _dbContext.Pedidos.Where(p => p.Cod_factura == id).ToListAsync();
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
                       .Where(p => p.Cod_factura == id)
                       .ToListAsync();
            var vnetoTotal = pedidos.Sum(p => p.ValorNeto);
            var vventaTotal = pedidos.Sum(p => p.ValorVenta);
            return pedidos;
        }
        public async Task<int> VentaInsertada(int ventaEfectivo, int ventaMakrotecno, int netoMakrotecno, int ventaRecarga, int ventaTienda, int ventapasivos)
        {
            DateTime fechaActual = DateTime.Now;
            var ventas = new Ventas
            {
                VentaTotal = ventaEfectivo,
                VentaMakrotecno = ventaMakrotecno,
                NetoMakrotecno = netoMakrotecno,
                VentaRecargas = ventaRecarga,
                VentaTienda = ventaTienda,
                VentaPasivos = ventapasivos,
                FechaVenta = fechaActual
            };

            _dbContext.Ventas.Add(ventas);
            _dbContext.SaveChanges();
            return ventas.Id_venta;
        }
        public async Task GananciaInsertada(int gananciaMakrotecno, int gananciaMaria, int gananciaVictor, int gananciaTeresa, int gananciaRecargas, int gananciaTotal)
        {
            DateTime fechaActual = DateTime.Now;
            var ganancia = new Ganancias
            {
                GananciaMakrotecno = gananciaMakrotecno,
                GananciaTotal = gananciaTotal,
                GananciaMaria = gananciaMaria,
                GananciaVictor = gananciaVictor,
                GananciaTeresa = gananciaTeresa,
                FechaGanancia = fechaActual
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
            var pedidoVerificado = _dbContext.Pedidos.FirstOrDefault(p => p.Cod_pedido == id);
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

        public decimal SumarNetoDelDia(DateTime fecha)
        {
            var sumaValorNeto = (from pedido in _dbContext.Pedidos
                                 join factura in _dbContext.Factura
                                 on pedido.Cod_factura equals factura.Cod_factura
                                 where factura.FechaVenta.Date == fecha.Date
                                 select pedido.ValorNeto)
                         .Sum();

            return sumaValorNeto;
        }

        public decimal SumarVVentaDelDia(DateTime fecha)
        {
            var sumaValorVenta = (from pedido in _dbContext.Pedidos
                                 join factura in _dbContext.Factura
                                 on pedido.Cod_factura equals factura.Cod_factura
                                 where factura.FechaVenta.Date == fecha.Date
                                 select pedido.ValorVenta)
                         .Sum();

            return sumaValorVenta;
        }
        public decimal SumarCompraTotal(DateTime fecha)
        {
            //var sumaCompraTotal = _dbContext.HistoricoCompras.Sum(hc => hc.ValorTotal);
            var sumaCompraTotal = (from HistoricoCompras in _dbContext.HistoricoCompras
                                   where HistoricoCompras.FechaRegistro.Date == fecha.Date
                                   select HistoricoCompras.ValorTotal)
                            .Sum();
            return sumaCompraTotal;
        }
        public int? ValidarEstadoPDV(int? traerIdPDVLogsLogin)
        {
            var today = DateTime.Today;
             return _dbContext.Syncpdv
            .Where(s => s.InfopdvId == traerIdPDVLogsLogin && s.FechaEstado.Date == today)
            .OrderByDescending(s => s.Idsync)
            .Select(s => s.Estado)
            .FirstOrDefault();
        }
        public int TraerUltimoIDPdv(int cedulaEmpleado)
        {
            return _dbContext.LogsLogin
            .Where(ce => ce.Cedula == cedulaEmpleado)
            .OrderByDescending(ce => ce.Id_log)
            .Select(ce => ce.InfopdvId)
            .FirstOrDefault();
        }
        public int? ValidarExistenteIdPDV(int idPDV, int cedula)
        {
            return _dbContext.Syncpdv
                .Where(s => s.InfopdvId == idPDV && s.Cedula == cedula)
                .OrderByDescending(s => s.Idsync)
                .Select(s => s.Estado)
                .FirstOrDefault();
        }
        public string? ObtenerNombreCliente(int cedulaCliente)
        {
            return _dbContext.Cliente
                .Where(nc => nc.CedulaCliente == cedulaCliente)
                .Select(nc => nc.NombreCliente)
                .FirstOrDefault();
        }
    }
}

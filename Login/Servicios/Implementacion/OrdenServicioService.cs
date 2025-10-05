using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using System.Security.Claims;

namespace Plataforma.Servicios.Implementacion
{
    public class OrdenServicioService : IOrdenServicioService
    {
        private readonly BaseAdmContext _dbContext;
        public OrdenServicioService(BaseAdmContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AgregarDispositivoAsync(Dispositivos dispositivo)
        {
            _dbContext.Dispositivos.Add(dispositivo);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<List<OrdenServicios>> ObtenerTodasAsync()
        {
            return await _dbContext.OrdenServicios
                .Include(o => o.Dispositivo)
                .ThenInclude(d => d.Cliente)
                .ToListAsync();
        }

        public async Task<OrdenServicios?> ObtenerPorIdAsync(int id)
        {
            return await _dbContext.OrdenServicios
                .Include(o => o.Dispositivo)
                .ThenInclude(d => d.Cliente)
                .FirstOrDefaultAsync(o => o.IdOrden == id);
        }

        public async Task<OrdenServicios> CrearAsync(OrdenServicios orden, string cedulaClaim)
        {
            orden.Cedula = int.Parse(cedulaClaim);
            orden.FechaIngreso = DateTime.Now;

            _dbContext.OrdenServicios.Add(orden);
            await _dbContext.SaveChangesAsync();

            return orden;
        }

        public async Task ActualizarAsync(OrdenServicios orden, int cedulaEmpleado)
        {
                var ordenExistente = await _dbContext.OrdenServicios
            .Include(o => o.Dispositivo)
            .ThenInclude(d => d.Cliente)
            .FirstOrDefaultAsync(o => o.IdOrden == orden.IdOrden);

                // Verifica si cambia a "Entregado"
                bool seEntrego = orden.Estado == "Entregado" && ordenExistente.Estado != "Entregado";

                ordenExistente.ProblemaReportado = orden.ProblemaReportado;
                ordenExistente.Estado = orden.Estado;
                ordenExistente.FechaIngreso = DateTime.Now;

                _dbContext.OrdenServicios.Update(ordenExistente);
                if (seEntrego)
                {
                    var venta = new Ventas
                    {
                        EstadoVenta = "Pendiente",
                        FechaVenta = DateTime.Now,
                        IdCliente = ordenExistente.Dispositivo.IdCliente,
                        Total = 0, // lo puedes ajustar según reglas
                        MetodoPago = 1,
                        Cedula = cedulaEmpleado
                    };

                    _dbContext.Ventas.Add(venta);
                }

                await _dbContext.SaveChangesAsync();
        }
        public async Task<List<HistOrdSer>> ObtenerOrdenPorIdAsync(int idOrden)
        {
                return await _dbContext.HistOrdServ
                    .Where(o => o.IdOrden == idOrden)
                    .ToListAsync();
        }
        public async Task<(OrdenServicios Orden, bool MostrarAgregarProductos)> ObtenerOrdenYPermisosAsync(int idOrden)
        {
            var orden = await _dbContext.OrdenServicios
                .FirstOrDefaultAsync(o => o.IdOrden == idOrden);

            if (orden == null)
                throw new Exception("No se encontró la orden.");

            bool mostrarAgregarProductos =
                orden.Estado?.Equals("Ejecucion", StringComparison.OrdinalIgnoreCase) == true;

            return (orden, mostrarAgregarProductos);
        }
        public async Task CrearHistOrdenAsync(
            HistOrdSer baseHistorial,
            string[]? Cod_Producto,
            int[] Stock,
            decimal[]? ValorRepuesto,
            string[]? Condicion,
            string[]? Tipo,
            int[] Proveedor,
            int cedula)
        {
            baseHistorial.FechaRegistro = DateTime.Now;
            baseHistorial.Cedula = cedula;

            var codigos = (Cod_Producto ?? Array.Empty<string>()).ToList();
            var stocks = (Stock ?? Array.Empty<int>()).ToList();
            var valorRepuestos = (ValorRepuesto ?? Array.Empty<decimal>()).ToList();
            var condiciones = (Condicion ?? Array.Empty<string>()).ToList();
            var tipos = (Tipo ?? Array.Empty<string>()).ToList();
            var proveedores = (Proveedor ?? Array.Empty<int>()).ToList();

            await using var tx = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if (codigos.Count > 0 && stocks.Count > 0)
                {
                    var registros = new List<HistOrdSer>();

                    for (int i = 0; i < codigos.Count; i++)
                    {
                        if (string.IsNullOrWhiteSpace(codigos[i])) continue;

                        decimal valorUnitario = valorRepuestos.ElementAtOrDefault(i);
                        int cantidad = stocks.ElementAtOrDefault(i);
                        decimal valorTotal = valorUnitario * cantidad;

                        registros.Add(new HistOrdSer
                        {
                            IdOrden = baseHistorial.IdOrden,
                            ReparacionDet = baseHistorial.ReparacionDet,
                            FechaRegistro = baseHistorial.FechaRegistro,
                            Cedula = baseHistorial.Cedula,
                            Cod_Producto = codigos[i].Trim(),
                            ValorNetoProducto = valorRepuestos.ElementAtOrDefault(i),
                            ValorVentaProducto = valorTotal,
                            CondicionProducto = condiciones.ElementAtOrDefault(i),
                            AutenticidadProducto = tipos.ElementAtOrDefault(i),
                            IdProveedor = proveedores.ElementAtOrDefault(i),
                            Stock = cantidad
                        });
                    }

                    _dbContext.HistOrdServ.AddRange(registros);

                    // Actualizar stock en tabla Productos
                    var porCodigo = registros
                        .GroupBy(x => x.Cod_Producto)
                        .ToDictionary(g => g.Key, g => g.Sum(r => r.Stock));

                    var productos = await _dbContext.Productos
                        .Where(p => porCodigo.Keys.Contains(p.Cod_Producto))
                        .ToListAsync();

                    foreach (var prod in productos)
                    {
                        var desc = porCodigo[prod.Cod_Producto];
                        prod.CantidadProducto = Math.Max(prod.CantidadProducto - desc, 0);
                    }

                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    // Caso sin productos
                    baseHistorial.IdProveedor = 24;
                    baseHistorial.Cod_Producto = null;
                    baseHistorial.Stock = 0;
                    _dbContext.HistOrdServ.Add(baseHistorial);
                    await _dbContext.SaveChangesAsync();
                }

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
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
        public async Task<bool> ActualizarOrdenAsync(OrdenServicios orden, ClaimsPrincipal usuario)
        {
            //1. Se consulta si la orden que viene del formulario existe o no
            var ordenExistente = await _dbContext.OrdenServicios
                .Include(o => o.Dispositivo)
                .FirstOrDefaultAsync(o => o.IdOrden == orden.IdOrden);

            if (ordenExistente == null)
                throw new Exception("La orden no existe.");

            //2. Se valida si el campo Estado que viene del formulario tiene la palabra Finalizada y el campo Estado que viene de la
            //consulta de la ordenExistente es diferente a Finalizada.
            //Por ultimo se actualiza los campos en el siguiente script.
            bool seFinaliza = orden.Estado == "Finalizada" && ordenExistente.Estado != "Finalizada";

            ordenExistente.Estado = orden.Estado;
            ordenExistente.Observaciones = orden.Observaciones;
            ordenExistente.IdDispositivo = orden.IdDispositivo;
            ordenExistente.ProblemaReportado = orden.ProblemaReportado;
            ordenExistente.ValorPago = orden.ValorPago;
            ordenExistente.FechaIngreso = DateTime.Now;
            _dbContext.OrdenServicios.Update(ordenExistente);

            //3. Validar si seFinaliza es true osea el estado es Finalizada, Ejecucion, Pendiente...
            //Si no, el estado debe ser Rechazada.
            if (seFinaliza)
            {
                // Obtener cédula desde el claim
                var cedulaStr = usuario.FindFirst("Cedula")?.Value;
                if (!int.TryParse(cedulaStr, out int cedula))
                    throw new Exception("No se pudo obtener la cédula del usuario autenticado.");

                // Traer todos los items de HistOrdServ para esta orden
                var itemsHist = await _dbContext.HistOrdServ
                    .Where(h => h.IdOrden == orden.IdOrden && h.Cod_Producto != null)
                    .ToListAsync();
                decimal? SubTotalPedidos = 0;
               
                // Consultar IdSede e InfoPdvId una sola vez aquí, porque los necesitas si hay pedidos
                var idSede = await _dbContext.Sedeempleado
                    .Where(se => se.Cedula == cedula)
                    .Select(se => se.Id_sede)
                    .FirstOrDefaultAsync();

                if (idSede == 0)
                    throw new Exception("No se encontró una sede asociada al usuario.");

                var infoPdvId = await _dbContext.Infopdv
                    .Where(p => p.Id_Sede == idSede)
                    .Select(p => p.InfopdvId)
                    .FirstOrDefaultAsync();

                if (infoPdvId == 0)
                    throw new Exception("No se encontró un PDV válido para la sede.");
                    // Crear venta (aunque no haya productos, subtotal es 0)
                    var venta = new Ventas
                    {
                        EstadoVenta = "Aprobada",
                        FechaVenta = DateTime.Now,
                        IdCliente = ordenExistente.Dispositivo.IdCliente,
                        Total = (decimal)orden.ValorPago,
                        MetodoPago = 1,
                        Cedula = cedula,
                        CedulaCliente = (int)ordenExistente.Dispositivo.CedulaCliente,
                        Conceptos = $"Venta generada desde la orden #{orden.IdOrden}"
                    };

                _dbContext.Ventas.Add(venta);
                await _dbContext.SaveChangesAsync(); // Genera IdVenta

                decimal? totalCosto = 0;
                int totalStock = 0;
                decimal? valorPagos = 0;
                var pedidosNuevos = new List<Pedidos>();
                foreach (var item in itemsHist)
                {
                    /*var prod = await _dbContext.Productos
                        .Where(p => p.Cod_Producto == "1")
                        .Select(p => new
                        {
                            p.Cod_Producto,
                            p.ValorVentaProducto,
                            p.ValorNetoProducto
                        })
                        .FirstOrDefaultAsync();*/
                        
                        pedidosNuevos.Add(new Pedidos
                        {
                            IdVenta = venta.IdVenta,
                            Codigo = "1",
                            Stock = item.Stock,
                            VNeto = item.ValorNetoProducto * item.Stock,
                            VVenta = 0,
                            InfopdvId = infoPdvId,
                            FechaRegistro = DateTime.Now,
                            SubTotal = 0
                        });
                    // Acumular para el registro final del servicio
                    valorPagos = orden.ValorPago;
                }
                // Registro extra: el servicio vendido (con el valor final al cliente)
                pedidosNuevos.Add(new Pedidos
                {
                    IdVenta = venta.IdVenta,
                    Codigo = "SERVICIO_TECNICO",   // puedes usar un código especial
                    Stock = 1,    // o 1 si prefieres que sea un servicio único
                    VNeto = 0,    // suma de los costos
                    VVenta = 0,       // aquí va el valor final que cobras
                    InfopdvId = infoPdvId,
                    FechaRegistro = DateTime.Now,
                    SubTotal = valorPagos      // valor de venta final
                });
                _dbContext.Pedidos.AddRange(pedidosNuevos);
                await _dbContext.SaveChangesAsync();
                decimal iva = 0;
                decimal totalIva = venta.Total * iva;
                decimal total = venta.Total + iva;
                Factura factura = new Factura
                {
                    NumeroFactura = await GenerarConsecutivoFactura(),
                    FechaEmision = DateTime.Now,
                    IdVenta = venta.IdVenta,
                    SubTotal = venta.Total,
                    IVA = totalIva,
                    Total = total,
                    EstadoFactura = "Emitida"
                };
                _dbContext.Factura.Add(factura);
                await _dbContext.SaveChangesAsync();
            }
            if(orden.Estado == "Rechazada")
            {
                await _dbContext.SaveChangesAsync();
                return false;
            }
                await _dbContext.SaveChangesAsync();
                return true;
        }
        public async Task<List<Empleados>> ObtenerEmpleadosAsync()
        {
            return await _dbContext.Empleado.ToListAsync();
        }
        public async Task<OrdenServicios?> ActualizarOrdenTecnico(OrdenServicios orden)
        {
            var ordenExistente = await _dbContext.OrdenServicios
                .FirstOrDefaultAsync(o => o.IdOrden == orden.IdOrden);

            if (ordenExistente == null)
                return null;

            // 🔹 Actualizas solo lo que venga de la vista
            ordenExistente.Cedula = orden.Cedula;

            // No cambias los demás campos (quedan igualitos en BD)
            await _dbContext.SaveChangesAsync();

            return ordenExistente;
        }
        public async Task<OrdenServicioRowDTO> GetOrdenRowAsync(int idOrden)
        {
            return await _dbContext.OrdenServicios
                .Where(o => o.IdOrden == idOrden)
                .Select(o => new OrdenServicioRowDTO
                {
                    IdOrden = o.IdOrden,
                    FechaIngreso = o.FechaIngreso,
                    Cliente = o.Dispositivo.Cliente.NombreCliente,
                    Telefono = o.Dispositivo.Cliente.TelefonoCliente,
                    Password = o.Dispositivo.Clave,
                    Marca = o.Dispositivo.Marca,
                    Modelo = o.Dispositivo.Modelo,
                    Descripcion = o.ProblemaReportado,
                    Observacion = o.Observaciones,
                    Estado = o.Estado,
                    Cedula = o.Cedula
                })
                .FirstAsync();
        }
        public async Task<List<Sedeempleado>> ObtenerEmpleadoSedeAsync()
        {
            return await _dbContext.Sedeempleado.ToListAsync();
        }
    }
}

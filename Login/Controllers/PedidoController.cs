using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Domain.Exceptions;
using Plataforma.Models;
using Plataforma.Models.Dto.Pedido;
using Plataforma.Servicios.Contrato;
using Plataforma.ViewModels.Pedido;
using System.Security.Claims;

namespace Plataforma.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IPedidoService _pedidoServicio;
        private readonly IProductoService _productoservice;
        private readonly ITercerosService _tercerosService;
        private readonly ILogger<HomeController> _logger;
        public PedidoController(IPedidoService pedidoServicio, IProductoService productoservice, ILogger<HomeController> logger, ITercerosService tercerService)
        {
            _pedidoServicio = pedidoServicio;
            _productoservice = productoservice;
            _logger = logger;
            _tercerosService = tercerService;
        }
        private ContextoAccesoDto ObtenerContextoAcceso()
        {
            return new ContextoAccesoDto
            {
                Cedula = int.Parse(User.FindFirstValue("Cedula") ?? "0"),
                EmpresaId = User.FindFirstValue("EmpresaId") ?? "",
                SedeId = int.Parse(User.FindFirstValue("SedeId") ?? "0"),
                PdvId = int.Parse(User.FindFirstValue("PdvId") ?? "0"),
                NombreRol = User.FindFirstValue("NombreRol") ?? ""
            };
        }
        public IActionResult Index()
        {
            _pedidoServicio.ActualizarEstadoFacturas();
            var facturas = _pedidoServicio.ObtenerFacturasFechaDescendente();
            return View(facturas);
        }
        [HttpGet]
        public async Task<IActionResult> CrearVenta()
        {
            var vm = await _pedidoServicio.ConstruirCrearVentaViewModelAsync(User);
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearVenta(CrearVentaViewModel vm)
        {
            if (vm.IdCliente <= 0)
            {
                TempData["ErrorMessage"] = "Debe seleccionar un cliente.";
                return RedirectToAction(nameof(CrearVenta));
            }

            if (string.IsNullOrWhiteSpace(vm.Conceptos))
            {
                TempData["ErrorMessage"] = "Debe ingresar un concepto para la venta.";
                return RedirectToAction(nameof(CrearVenta));
            }

            var creada = await _pedidoServicio.CrearVentaAsync(vm);

            if (!creada)
            {
                TempData["ErrorMessage"] = "No se pudo crear la venta.";
                return RedirectToAction(nameof(CrearVenta));
            }

            return RedirectToAction("AgregarProductoAVenta", new { idVenta = vm.IdVentaCreada });
        }
        [Authorize]
        public async Task<IActionResult> ListaVentas()
        {
            var ctx = ObtenerContextoAcceso();
            var ventas = await _pedidoServicio.ObtenerVentasFiltradasAsync(ctx);
            return View(ventas);
        }
        [HttpGet]
        public async Task<IActionResult> DetalleVenta(int idVenta)
        {
            var model = await _pedidoServicio.ObtenerVentaConPedidos(idVenta);

            if (model == null)
            {
                TempData["ErrorMessage"] = "La venta no existe.";
                return RedirectToAction("ListaVentas");
            }

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> RegistrarPagos(int idVenta)
        {
            var venta = await _pedidoServicio.ObtenerVentaConDetalleAsync(idVenta);

            if (venta == null)
            {
                TempData["ErrorMessage"] = "La venta no existe.";
                return RedirectToAction("ListaVentas");
            }

            if (venta.EstadoVenta != "Confirmada")
            {
                TempData["ErrorMessage"] = "La venta debe estar confirmada para registrar pagos.";
                return RedirectToAction("DetalleVenta", new { idVenta });
            }

            var viewModel = new RegistrarPagosViewModel
            {
                IdVenta = venta.IdVenta,
                IdCliente = venta.IdCliente,
                TotalVenta = venta.Total,

                OrigenModulo = venta.OrigenModulo,
                TipoOperacion = venta.TipoOperacion,
                CodigoReferenciaOrigen = venta.CodigoReferenciaOrigen,
                ObservacionVenta = venta.ObservacionVenta
            };

            return PartialView("_RegistrarPagos", viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> RegistrarPagos(RegistrarPagosViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // ✅ Construir método final
            var metodos = new List<string>();
            if (model.MontoEfectivo > 0) metodos.Add("Efectivo");
            if (model.MontoTransferencia > 0) metodos.Add("Transferencia");
            if (model.MontoCredito > 0) metodos.Add("Crédito");
            var metodoPagoFinal = string.Join(" + ", metodos);

            await _pedidoServicio.FacturarConPagosAsync(
                model.IdVenta,
                model.IdCliente,
                metodoPagoFinal,                 // ✅ 3er parámetro: string
                model.MontoEfectivo,             // ✅ 4to: decimal
                model.MontoTransferencia,        // ✅ 5to: decimal
                model.MontoCredito,              // ✅ 6to: decimal
                model.FechaVencimientoCredito    // ✅ 7mo: DateTime?
            );

            TempData["SuccessMessage"] = "Factura emitida correctamente.";
            return RedirectToAction("DetalleVenta", new { idVenta = model.IdVenta });
        }
        [HttpGet]
        public async Task<IActionResult> BuscarProductoPorCodigo(string codigo)
        {
            var resultado = await _pedidoServicio.BuscarProductoPorCodigoAsync(codigo);

            if (resultado == null)
                return NotFound();

            return Json(new
            {
                valorVenta = resultado.Value.valorVenta,
                valorNeto = resultado.Value.valorNeto
            });
        }
        [HttpGet]
        public async Task<IActionResult> BuscarProductoPorNombreVenta(string texto)
        {
            
                var result = await _pedidoServicio.BuscarProductosPorNombreVentaAsync(texto, User);
                return Json(result);
            
        }


        [HttpGet]
        public IActionResult AgregarProductoAVenta(int idVenta)
        {
            var viewModel = new PedidosViewModel
            {
                IdVenta = idVenta,
                Productos = new List<Pedidos>
        {
            new Pedidos() // Inicializa con una fila vacía
        }
            };

            return View(viewModel); // ✅ Ahora el modelo coincide con la vista
        }

        [HttpPost]
        public async Task<IActionResult> AgregarMultiplesProductosAVenta(PedidosViewModel model)
        {
            try
            {
                await _pedidoServicio.GuardarPedidosAsync(model.Productos, model.IdVenta, User);
                return RedirectToAction("DetalleVenta", new { idVenta = model.IdVenta });
            }
            catch (PedidoException ex)
            {
                TempData["ErrorMessage"] = ex.Message; // Pone el mensaje de error en TempData
                return RedirectToAction("Error", "Errores"); // Redirige a ErroresController
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction("Error", "Errores"); // Redirige a ErroresController
            }
        }
        [Authorize]
        public IActionResult CrearFactura()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CrearFactura(int idVenta, int Iva)
        {
            var venta = await _pedidoServicio.ObtenerVentaConPedidos(idVenta);
            if (venta.Venta == null || venta.Venta.Pedidos.Count == 0)
            {
                TempData["ErrorMessage"] = "No se encontró la venta o no tiene productos.";
                return RedirectToAction("AgregarFactura");
            }
            //decimal iva = subtotal * 0.19M;
            decimal total = (decimal)venta.Venta.Pedidos.Sum(p => p.SubTotal);
            decimal totalIva = ((total * Iva) / 100);
            if (totalIva > 0)
            {
                total = totalIva + total;
            }
            await _pedidoServicio.GuardarVentaActualizada(venta.Venta, total);

            Factura factura = new Factura
            {
                NumeroFactura = "1",
                FechaEmision = DateTime.Now,
                IdVenta = idVenta,
                SubTotal = total,
                IVA = totalIva,
                Total = total,
                EstadoFactura = "Emitida"
            };

            await _pedidoServicio.GuardarFacturaAsync(factura);

            await _pedidoServicio.ActualizarEstadoVentaAsync(idVenta, "Finalizado");

            return RedirectToAction("ListaFacturas");
        }
        [Authorize]
        public async Task<IActionResult> ListaFacturas()
        {
            var ctx = ObtenerContextoAcceso();
            var facturas = await _pedidoServicio.ObtenerFacturasFiltradasAsync(ctx);
            return View(facturas);
        }
        public async Task<IActionResult> DetalleFactura(int idFactura)
        {
            var empresaId = User.FindFirst("EmpresaId")?.Value;

            if (string.IsNullOrEmpty(empresaId))
                return Unauthorized();

            var factura = await _pedidoServicio.ObtenerFacturaConDetalle(idFactura, empresaId);

            if (factura == null)
                return NotFound();

            return View(factura);
        }
        [HttpPost]
        public async Task<IActionResult> AnularFactura(int idFactura)
        {
            bool result = await _pedidoServicio.AnularFacturaAsync(idFactura);
            if (!result)
                TempData["ErrorMessage"] = "No se pudo anular la factura.";

            return RedirectToAction("ListaFacturas");
        }
        [HttpPost]
        public async Task<IActionResult> EliminarFactura(int idFactura)
        {
            bool eliminado = await _pedidoServicio.EliminarFacturaAsync(idFactura);
            if (!eliminado)
                TempData["ErrorMessage"] = "No se pudo eliminar la factura.";

            return RedirectToAction("ListaFacturas");
        }
        [HttpPost]
        public async Task<IActionResult> CambiarEstadoVenta(int idVenta, string nuevoEstado)
        {
            try
            {
                await _pedidoServicio.CambiarEstadoVentaAsync(idVenta, nuevoEstado);
                return RedirectToAction("DetalleVenta", new { idVenta });
            }
            catch (PedidoException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Errores");
            }
        }

        [HttpGet]
        public async Task<IActionResult> BuscarFacturaPorId(int IdFactura)
        {
            var factura = await _pedidoServicio.ObtenerFacturaConAdicionesAsync(IdFactura);
            if (factura == null)
                return PartialView("_FacturaNoEncontrada");

            var rol = User.IsInRole("Administrador") ? "Admin" : "User";
            ViewBag.Rol = rol;
            return PartialView("_FacturaParcial", factura);
        }

        [HttpGet]
        public IActionResult FormularioAdicion(int IdFactura)
        {
            return PartialView("_FormularioAdicion", IdFactura);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarAdicion(int IdFactura, decimal Valor, string Descripcion, string EstadoAdicion)
        {
            var cedulaClaim = User.FindFirst("Cedula")?.Value;
            if (int.TryParse(cedulaClaim, out int cedulaEmpleado))
            {
                var resultado = await _pedidoServicio.AgregarAdicionFacturaAsync(IdFactura, Valor, Descripcion, cedulaEmpleado, EstadoAdicion);
                return Json(new { success = resultado });
            }
            return Json(new { success = false });
        }
        public IActionResult VerConceptos()
        {
            //var facturas = _pedidoServicio.ObtenerFacturasFechaDescendente();
            var traerConceptos = _pedidoServicio.ObtenerConceptosCompletos();
            return View(traerConceptos);
        }
        [HttpGet]
        public async Task<IActionResult> BuscarProductoPorCodigoVenta(string codigo)
        {
            //Console.WriteLine("Me oprimiste aca" + codigo);
            var productos = await _pedidoServicio.BuscarProductosPorCodigo(codigo, User);
            var resultados = productos.Select(p => new {
                label = $"{p.ProductoId} - {p.Producto.NombreProducto}",
                value = p.ProductoId,
                valorUnidad = p.VUnidad,
                valorVenta = p.PrecioUnitario
            });

            return Json(resultados);
        }
        [HttpGet]
        public async Task<IActionResult> ImprimirFactura(int idFactura)
        {
            var empresaId = User.FindFirst("EmpresaId")?.Value;

            if (string.IsNullOrEmpty(empresaId))
                return Unauthorized();

            var factura = await _pedidoServicio.ObtenerFacturaConDetalle(idFactura, empresaId);
            if (factura == null) return NotFound();

            return View(factura); // Vista: ImprimirFactura.cshtml
        }
    }
}

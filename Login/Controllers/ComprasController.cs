using MakroTecno.ViewModels.Compras;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Controllers
{
    public class ComprasController : Controller
    {
        private readonly IComprasService _comprasService;
        public ComprasController(IComprasService comprasService)
        {
            _comprasService = comprasService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> InsertarCompra()
        {
            ViewBag.Proveedores = await _comprasService.ObtenerProveedoresAsync();
            return View(new CompraViewModel());
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> InsertarCompraCreate([FromForm] CompraViewModel model)
        {
            if (ModelState.IsValid)
            {
                var resultado = await _comprasService.InsertarCompraAsync(model);
                if (resultado)
                {
                    return RedirectToAction("InsertarCompra"); // O a donde requieras
                }
            }
            else
            {
                // Aquí inspeccionas los errores del ModelState
                foreach (var entry in ModelState)
                {
                    var key = entry.Key; // Nombre del campo
                    var errors = entry.Value.Errors;
                    foreach (var error in errors)
                    {
                        // Puedes registrar, mostrar o hacer algo con los errores
                        Console.WriteLine($"Error en campo '{key}': {error.ErrorMessage}");
                    }
                }
            }

            // Si falla la validación o inserción, recarga la vista con el modelo
            ViewBag.Proveedores = await _comprasService.ObtenerProveedoresAsync();
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var compras = await _comprasService.ObtenerComprasAsync();
            return View(compras);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> BuscarProductoPorCodigo(string codigo)
        {
            //Console.WriteLine("Me oprimiste aca" + codigo);
            var productos = await _comprasService.BuscarProductosPorCodigoAsync(codigo);
            var resultados = productos.Select(p => new {
                label = $"{p.Cod_Producto} - {p.NombreProducto}",
                value = p.Cod_Producto,
                valorNeto = p.ValorNetoProducto,
                valorVenta = p.ValorVentaProducto
            });

            return Json(resultados);
        }
        [HttpGet]
        public async Task<IActionResult> FacturasCompra()
        {
            var facturas = await _comprasService.ObtenerFacturasCompraAsync();
            return View(facturas);
        }

        [HttpGet]
        public async Task<IActionResult> CrearFacturaCompra()
        {
            var model = new FacturaCompraViewModel
            {
                FechaCompra = DateTime.Now,
                Proveedores = await _comprasService.ObtenerProveedoresSelectAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearFacturaCompra(FacturaCompraViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.Proveedores = await _comprasService.ObtenerProveedoresSelectAsync();
                    return View(model);
                }

                await _comprasService.CrearFacturaCompraAsync(model);

                TempData["Success"] = "Factura de compra guardada correctamente.";
                return RedirectToAction(nameof(FacturasCompra));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                model.Proveedores = await _comprasService.ObtenerProveedoresSelectAsync();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditarFacturaCompra(int id)
        {
            var factura = await _comprasService.ObtenerFacturaCompraPorIdAsync(id);

            if (factura == null)
                return NotFound();

            var model = new FacturaCompraViewModel
            {
                IdFacturaCompra = factura.IdFacturaCompra,
                IdProveedor = factura.IdProveedor,
                NumeroFactura = factura.NumeroFactura,
                PrefijoFactura = factura.PrefijoFactura,
                FechaCompra = factura.FechaCompra,
                Observacion = factura.Observacion,
                RutaArchivoActual = factura.RutaArchivo,
                NombreArchivoOriginal = factura.NombreArchivoOriginal,
                Proveedores = await _comprasService.ObtenerProveedoresSelectAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarFacturaCompra(FacturaCompraViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.Proveedores = await _comprasService.ObtenerProveedoresSelectAsync();
                    return View(model);
                }

                await _comprasService.ActualizarFacturaCompraAsync(model);

                TempData["Success"] = "Factura de compra actualizada correctamente.";
                return RedirectToAction(nameof(FacturasCompra));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                model.Proveedores = await _comprasService.ObtenerProveedoresSelectAsync();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> VerFacturaCompra(int id)
        {
            var factura = await _comprasService.ObtenerFacturaCompraPorIdAsync(id);

            if (factura == null)
                return NotFound();

            return View(factura);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InactivarFacturaCompra(int id, string? motivoInactivacion)
        {
            try
            {
                string? usuario = User.Identity?.Name;

                await _comprasService.InactivarFacturaCompraAsync(id, motivoInactivacion, usuario);

                TempData["Success"] = "Factura inactivada correctamente. El archivo fue conservado para trazabilidad documental.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(FacturasCompra));
        }
    }
}
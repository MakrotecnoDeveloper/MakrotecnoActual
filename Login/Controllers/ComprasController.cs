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
        [HttpGet]
        public async Task<IActionResult> InsertarCompra()
        {
            ViewBag.Proveedores = await _comprasService.ObtenerProveedoresAsync();
            return View(new CompraViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> InsertarCompra(CompraViewModel model)
        {
            if (ModelState.IsValid)
            {
                var compra = new Compras
                {
                    IdProveedor = model.IdProveedor,
                    CodFacturaExterno = model.CodFacturaExterno,
                    FechaCompra = DateTime.Now,
                    Estado = 1
                };

                var detalles = model.Detalles.Select(d => new DetalleCompra
                {
                    CodProducto = d.CodProducto,
                    Cantidad = d.Cantidad,
                    ValorU = d.ValorU,
                    ValorTotal = d.Cantidad * d.ValorU
                }).ToList();

                await _comprasService.InsertarCompraAsync(compra, detalles);

                return RedirectToAction("Index");
            }

            ViewBag.Proveedores = await _comprasService.ObtenerProveedoresAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var compras = await _comprasService.ObtenerComprasAsync();
            return View(compras);
        }

        [HttpGet]
        public async Task<IActionResult> BuscarProductoPorCodigo(string term)
        {
            var productos = await _comprasService.BuscarProductosPorCodigoAsync(term);
            var resultados = productos.Select(p => new {
                label = $"{p.Cod_Producto} - {p.NombreProducto}",
                value = p.Cod_Producto,
                valorNeto = p.ValorNetoProducto
            });

            return Json(resultados);
        }
    }
}
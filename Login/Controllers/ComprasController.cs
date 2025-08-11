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


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var compras = await _comprasService.ObtenerComprasAsync();
            return View(compras);
        }

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
    }
}
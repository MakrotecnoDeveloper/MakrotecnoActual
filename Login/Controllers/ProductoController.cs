using Microsoft.AspNetCore.Mvc;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using Plataforma.Servicios.Implementacion;

namespace Plataforma.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IProductoService _productoservice;
        public ProductoController(IProductoService productoservice)
        {
            _productoservice = productoservice;
        }
        public IActionResult Index()
        {
            var productos = _productoservice.ObtenerProductos();
            return View(productos);
        }
        public IActionResult Insertar() 
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Insertar(string id_empresa, string codigo, string descripcion, int valorNeto, int valorVenta, int stock, string categoria)
        {
            Console.WriteLine(valorNeto);

            if (ModelState.IsValid)
            {
                // Lógica para agregar el producto usando _productoService
                var resultado = await _productoservice.AgregarProductoAsync(id_empresa, codigo, descripcion, valorNeto, valorVenta, stock, categoria);

                if (resultado)
                {
                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }
    }
}

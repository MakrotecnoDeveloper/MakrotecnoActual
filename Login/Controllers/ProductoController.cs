using Login.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using Plataforma.Servicios.Implementacion;
using System.Security.Claims;

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
        public async Task<IActionResult> Insertar(string id_empresa, string codigo, string descripcion, float valorNeto, float valorVenta, int stock, string categoria)
        {

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
        [HttpPost]
        public IActionResult Buscar(string searchTerm, string categoriaTerm)
        {
            var productosEncontrados = _productoservice.BuscarProductos(searchTerm, categoriaTerm);
            //Console.WriteLine(productosEncontrados);
            if (productosEncontrados.Any())
            {
                // Oculta la tabla de productos y muestra la tabla temporal
                return PartialView("_TablaProductos", productosEncontrados);
            }
            else
            {
                // Producto no encontrado, maneja la lógica adecuada
                ViewBag.Mensaje = "Producto no encontrado";
                return PartialView("_Mensaje");
            }
        }
        public IActionResult Editar(string id)
        {
            string categoriaTerm = "";
            var editarProducto = _productoservice.BuscarProductos(id, categoriaTerm);
            if (editarProducto.Any())
            {
                // Oculta la tabla de productos y muestra la tabla temporal
                return View("", editarProducto);
            }
            else
            {
                // Producto no encontrado, maneja la lógica adecuada
                ViewBag.Mensaje = "Producto no encontrado";
                return View("_Mensaje");
            }
        }
        public IActionResult Stock(string id, int cantidad, int opcion)
        {
            var productosActualizados = _productoservice.EditarStock(id, cantidad, opcion);
            if (productosActualizados.Any())
            {
                return View("Index", productosActualizados);
            }
            else
            {
                ViewBag.Mensaje = "Producto no encontrado";
                return View("_Mensaje");
            }
        }
    }
}

using Login.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        [AuthorizeRole(1)]
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
        [Authorize]
        [HttpGet]
        public IActionResult Buscar(string searchTerm, string categoriaTerm)
        {
            if (string.IsNullOrEmpty(searchTerm)) {
                searchTerm = "";
            }else
            {
                categoriaTerm = "";
            }
            var productosEncontrados = _productoservice.BuscarProductos(searchTerm, categoriaTerm);
            return PartialView("_TablaProductos", productosEncontrados);
        }
        [HttpGet]
        public IActionResult BuscarSinStock(string searchTerm, string categoriaTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = "";
            }
            else
            {
                categoriaTerm = "";
            }
            var productosSinStock = _productoservice.SinStock(searchTerm, categoriaTerm);
            return PartialView("_TablaProductos", productosSinStock);
        }
        [HttpGet]
        public IActionResult BuscarProximosSinStock(string searchTerm, string categoriaTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = "";
            }
            else
            {
                categoriaTerm = "";
            }
            var productosEncontrados = _productoservice.BuscarProSinStock(searchTerm, categoriaTerm);
            return PartialView("_TablaProductos", productosEncontrados);
        }
        public IActionResult Editar(string id)
        {
            string categoriaTerm = "";
            var editarProducto = _productoservice.BuscarProductos(id, categoriaTerm);
            foreach (var producto in editarProducto)
            {
                // Realiza acciones con cada producto, por ejemplo:
                Console.WriteLine($"ID: {producto.Cod_Producto}, Nombre: {producto.NombreProducto}");
            }
            if (editarProducto.Any())
            {
                // Oculta la tabla de productos y muestra la tabla temporal
                return View(editarProducto);
            }
            else
            {
                // Producto no encontrado, maneja la lógica adecuada
                Console.WriteLine("No hay productos con ese codigo referenciado");
                return View("Index");
            }
        }
        [HttpPost]
        public IActionResult EditarProducto(string codigo, string nombreProducto, float valorNeto, float valorVenta, int cantidad, string categoria, string idEmpresa)
        {
            Console.WriteLine(codigo + nombreProducto + valorNeto + valorVenta + cantidad + categoria + idEmpresa);
            // Llama al método EditarProducto del servicio de productos
            _productoservice.EditarProducto(codigo, nombreProducto, valorNeto, valorVenta, cantidad, categoria, idEmpresa);

            // Redirige a la acción que deseas después de editar el producto
            return RedirectToAction("Index"); // Por ejemplo, redirigir a la página de inicio del controlador de productos
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
        public IActionResult Eliminar(string id)
        {
            _productoservice.EliminarProducto(id);
            return RedirectToAction("Index");
        }
        public IActionResult VisualizarProducto(string id)
        {
            string categoriaTerm = "";
            string searchTerm = id;
            var traerProductos = _productoservice.BuscarProductos(searchTerm, categoriaTerm);
            return View(traerProductos);
        }
        /*Visualizacion de  Recargas de Plataformas */
        public IActionResult formPlataforma()
        {
            return View();
        }
        [HttpPost]
        public IActionResult insertPlataforma(string plataforma, string descripcion, int valorventa, int valorneto, DateTime fechaInipago, DateTime fechaFinpago, int cantidad, string correo, string contrasena, int cedula, int estado) 
        { 
            if(string.IsNullOrEmpty(plataforma) || string.IsNullOrEmpty(descripcion) || valorventa <= 0 || valorneto <= 0 || fechaInipago == DateTime.MinValue || fechaFinpago == DateTime.MinValue || cantidad <= 0 || string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena) || cedula <= 0 || estado <= 0)
            {
                var mensaje = "Error: Hay campos sin informacion digitada, revisar todo lo llenado.";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }else
            {
                _productoservice.inserPlataformaService(plataforma, descripcion, valorventa, valorneto, fechaInipago, fechaFinpago, cantidad, correo, contrasena, cedula, estado);
                return View("formPlataforma");
            }
        }
        public async Task<IActionResult> formInserClienPlatf()
        {
            var traerPlataformas = await _productoservice.traerPlataformasExistentes();
            foreach(var plataformas in traerPlataformas)
            {
                int idPlataforma = plataformas.idPlataforma;
                if(idPlataforma > 0)
                {
                    TempData["idplataform"] = idPlataforma;
                }else
                {
                    idPlataforma = 1;
                    TempData["idplataform"] = idPlataforma;
                }
            }
            return View(traerPlataformas);
        }
        public IActionResult insertVentClientPltf(string nombrecliente, string celularcliente, string correo, string contrasena, int idplataforma, int cantidad, string ppm, DateTime feciniplat, DateTime fecfinplat, int valorventa, int valorneto, int cedula, int estado)
        {
            _productoservice.servicioInsertarVentClientPlataforma(nombrecliente, celularcliente, correo, contrasena, idplataforma, cantidad, ppm, feciniplat, fecfinplat, valorventa, valorneto, cedula, estado);
            return View("formInserClienPlatf");
        }
        public IActionResult insertInfoCuentaClientPlatf(int idCliPltf, string perfil, string clave)
        {
            _productoservice.servicioInsertarInfoCuentaClientPlatf(idCliPltf, perfil, clave);
            return View("formInserClienPlatf");
        }
    }
}

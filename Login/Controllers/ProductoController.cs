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
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> VisualizarProducto(int id, string nombreProducto)
        {
            string cod_producto = "";
            string descripcion = nombreProducto;
            int cantidadProducto = 0;
            float valorNetoProducto = 0;
            float valorVentaProducto = 0;
            string id_empresa = "";
            string categoria = "";
            var perfil = _usuarioService.ObtenerPerfilPorId(id);
            var resultados = await _usuarioService.BuscarUsuarios(nombre, municipio, lider, area, celula);
            var comentarios = await _usuarioService.ObtenerComentariosPorIdDestinatario(id);
            if (perfil == null)
            {
                return RedirectToAction("Error", "Errores", new { mensaje = "Error, no se encontro toda la informacion solicitada." });
            }
            var modelo = new VisualizarUsuarioViewModel
            {
                Perfil = perfil,
                Comentarios = comentarios,
                ResultadosBusqueda = resultados,
                Celula = idCelula
            };
            return View(modelo);
        }
    }
}

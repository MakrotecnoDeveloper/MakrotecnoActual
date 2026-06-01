using MakroTecno.ViewModels.Integraciones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using System.Security.Claims;

namespace Plataforma.Controllers
{
    public class IntegracionController : Controller
    {
        private readonly IIntegracionService _integracionService;
        public IntegracionController(IIntegracionService integracionService)
        {
            _integracionService = integracionService;
        }
        // =====================================================
        // DASHBOARD
        // =====================================================

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // =====================================================
        // TIENDAS RAPPI
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> RappiTiendas()
        {
            var tiendas = await _integracionService.ObtenerTiendasRappiAsync();
            return View(tiendas);
        }

        [HttpGet]
        public async Task<IActionResult> CrearEditarRappiTienda(int? id)
        {
            var vm = await _integracionService.PrepararTiendaRappiAsync(id);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearEditarRappiTienda(RappiTiendaViewModel vm)
        {
            try
            {
                await _integracionService.GuardarTiendaRappiAsync(vm);
                TempData["Success"] = "Tienda Rappi guardada correctamente.";
                return RedirectToAction(nameof(RappiTiendas));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                var nuevoVm = await _integracionService.PrepararTiendaRappiAsync(vm.Id);
                nuevoVm.IdTiendaRappi = vm.IdTiendaRappi;
                nuevoVm.NombreTiendaRappi = vm.NombreTiendaRappi;
                nuevoVm.SedeId = vm.SedeId;
                nuevoVm.Activa = vm.Activa;
                return View(nuevoVm);
            }
        }

        // =====================================================
        // CATEGORÍAS RAPPI
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> RappiCategorias()
        {
            var categorias = await _integracionService.ObtenerCategoriasMapeoAsync();
            return View(categorias);
        }

        [HttpGet]
        public async Task<IActionResult> CrearEditarRappiCategoria(int? id)
        {
            var vm = await _integracionService.PrepararCategoriaMapeoAsync(id);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearEditarRappiCategoria(RappiCategoriaMapeoViewModel vm)
        {
            try
            {
                await _integracionService.GuardarCategoriaMapeoAsync(vm);
                TempData["Success"] = "Categoría Rappi guardada correctamente.";
                return RedirectToAction(nameof(RappiCategorias));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                var nuevoVm = await _integracionService.PrepararCategoriaMapeoAsync(vm.Id);
                nuevoVm.IdCateProducto = vm.IdCateProducto;
                nuevoVm.CategoriaRappi = vm.CategoriaRappi;
                nuevoVm.Activo = vm.Activo;
                return View(nuevoVm);
            }
        }

        // =====================================================
        // PRODUCTOS RAPPI
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> RappiProductosConfig()
        {
            var productos = await _integracionService.ObtenerProductosConfigRappiAsync();
            return View(productos);
        }

        [HttpGet]
        public async Task<IActionResult> CrearEditarRappiProductoConfig(int? id)
        {
            var vm = await _integracionService.PrepararProductoConfigAsync(id);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearEditarRappiProductoConfig(RappiProductoConfigViewModel vm)
        {
            try
            {
                await _integracionService.GuardarProductoConfigAsync(vm);
                TempData["Success"] = "Configuración del producto guardada correctamente.";
                return RedirectToAction(nameof(RappiProductosConfig));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                var nuevoVm = await _integracionService.PrepararProductoConfigAsync(vm.Id);

                nuevoVm.ProductoId = vm.ProductoId;
                nuevoVm.SkuRappi = vm.SkuRappi;
                nuevoVm.NombreRappi = vm.NombreRappi;
                nuevoVm.DescripcionRappi = vm.DescripcionRappi;
                nuevoVm.MarcaRappi = vm.MarcaRappi;
                nuevoVm.Ean = vm.Ean;
                nuevoVm.EsPesable = vm.EsPesable;
                nuevoVm.EsPreempaquetado = vm.EsPreempaquetado;
                nuevoVm.CantidadPresentacion = vm.CantidadPresentacion;
                nuevoVm.UnidadMedidaRappi = vm.UnidadMedidaRappi;
                nuevoVm.PublicarEnRappi = vm.PublicarEnRappi;
                nuevoVm.Activo = vm.Activo;

                return View(nuevoVm);
            }
        }

        // =====================================================
        // EXPORTACIÓN RAPPI
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> RappiProductosPersonalizados(int rappiTiendaId, bool soloConStock = true)
        {
            var resultado = await _integracionService.ValidarProductosPersonalizadosRappiAsync(
                rappiTiendaId,
                soloConStock
            );

            return View(resultado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerarRappiProductosPersonalizados(int rappiTiendaId, bool soloConStock = true)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.Identity?.Name ?? "Sistema";

            var resultado = await _integracionService.GenerarExcelProductosPersonalizadosRappiAsync(
                rappiTiendaId,
                soloConStock,
                usuarioId
            );

            if (resultado.Exitoso)
                TempData["Success"] = resultado.Mensaje;
            else
                TempData["Error"] = resultado.Mensaje;

            return View("RappiProductosPersonalizados", resultado);
        }
        [HttpGet]
        public async Task<IActionResult> RappiActualizarProductos()
        {
            var vm = await _integracionService.PrepararActualizacionProductosRappiAsync();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RappiActualizarProductos(int rappiTiendaId, IFormFile archivo)
        {
            try
            {
                var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.Identity?.Name
                    ?? "Sistema";

                var resultado = await _integracionService.ActualizarProductosRappiDesdeExcelAsync(
                    rappiTiendaId,
                    archivo,
                    usuarioId
                );

                if (resultado.Exitoso)
                    TempData["Success"] = resultado.Mensaje;
                else
                    TempData["Error"] = resultado.Mensaje;

                return View(resultado);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                var vm = await _integracionService.PrepararActualizacionProductosRappiAsync();
                vm.Mensaje = ex.Message;
                vm.Exitoso = false;

                return View(vm);
            }
        }
    }
}
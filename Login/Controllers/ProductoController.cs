using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using Plataforma.Servicios.Implementacion;

namespace Plataforma.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IProductoService _productoservice;
        private readonly BaseAdmContext _dbContext;
        public ProductoController(IProductoService productoservice, BaseAdmContext dbContext)
        {
            _productoservice = productoservice;
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            var model = new ProductosCategoriaViewModel
            {
                Productos = _productoservice.ObtenerProductos(),
                CategoriaProductos = _productoservice.ObtenerCategorias()
            };

            return View(model);
        }

        // Para la búsqueda asincrónica
        public IActionResult ProductosPorCategoria(int idCategoria)
        {
            var productos = _productoservice.ObtenerProductosPorCategoria(idCategoria);
            return PartialView("_TablaProductos", productos);
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerCategoriaProductos(int idServicio)
        {
            var categorias = await _productoservice.ObtenerCategoriasPorServicio(idServicio);

            return Json(categorias.Select(c => new {
                idCateProducto = c.IdCateProducto,
                descripcion = c.Descripcion
            }));
        }
        [HttpGet]
        public async Task<IActionResult> Insertar()
        {
            var model = new ProductoInsertarViewModel
            {
                Servicios = await _productoservice.ObtenerServicios(),
                Categorias = new List<CategoriaProductos>(),
                Proveedores = await _productoservice.ObtenerProveedores()
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Insertar(string id_empresa, string codigo, string descripcion, float valor_neto, float valor_unitario, decimal stock, int categorias, int id_proveedor)
        {

            if (ModelState.IsValid)
            {
                // Lógica para agregar el producto usando _productoService
                var resultado = await _productoservice.AgregarProductoAsync(id_empresa, codigo, descripcion, valor_neto, valor_unitario, stock, categorias, id_proveedor);

                if (resultado)
                {
                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }
        [Authorize]
        [HttpGet]
        public IActionResult Buscar(string searchTerm, int categoriaTerm)
        {
            if (string.IsNullOrEmpty(searchTerm)) {
                searchTerm = "";
            } else
            {
                categoriaTerm = 0;
            }
            var productosEncontrados = _productoservice.BuscarProductos(searchTerm, categoriaTerm);
            return PartialView("_TablaProductos", productosEncontrados);
        }
        [HttpGet]
        public IActionResult BuscarSinStock(string searchTerm, int categoriaTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = "";
            }
            else
            {
                categoriaTerm = 0;
            }
            var productosSinStock = _productoservice.SinStock(searchTerm, categoriaTerm);
            return PartialView("_TablaProductos", productosSinStock);
        }
        [HttpGet]
        public IActionResult BuscarProximosSinStock(string searchTerm, int categoriaTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = "";
            }
            else
            {
                categoriaTerm = 0;
            }
            var productosEncontrados = _productoservice.BuscarProSinStock(searchTerm, categoriaTerm);
            return PartialView("_TablaProductos", productosEncontrados);
        }
        [HttpGet]
        public async Task<IActionResult> Editar(string id)
        {
            int categoriaTerm = 0;
            var editarProducto = _productoservice.BuscarProductos(id, categoriaTerm);
            if (!editarProducto.Any())
            {
                Console.WriteLine("No hay productos con ese codigo referenciado");
                return View("Index");
            }

            // Catálogos para la UI (no se postean)
            ViewBag.Servicios = await _productoservice.ObtenerServiciosAsync();

            ViewBag.Proveedores = await _productoservice.ObtenerProveedores();

            // Servicio preseleccionado a partir de la categoría del producto
            var p = editarProducto.First();
            ViewBag.SelectedServicioId = await _productoservice.SeleccionarServicio(p);

            // (Opcional) Preselección de proveedor si tu entidad Producto tiene IdProveedor
            ViewBag.SelectedProveedorId = p.idProveedor;

            return View(editarProducto); // @model List<Producto>
        }
        [HttpPost]
        public IActionResult EditarProducto(string codigo, float valorNeto, float valorVenta, int valorUnidad, int cantidad)
        {
            // Llama al método EditarProducto del servicio de productos
            _productoservice.EditarProducto(codigo, valorNeto, valorVenta, valorUnidad, cantidad);

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
            int categoriaTerm = 0;
            string searchTerm = id;
            var traerProductos = _productoservice.BuscarProductos(searchTerm, categoriaTerm);
            return View(traerProductos);
        }
        /*Visualizacion de  Recargas de Plataformas */
        public IActionResult FormPlataforma()
        {
            var traerPlataformasExistentes = _productoservice.TraerPlataformasExistentes();
            return View(traerPlataformasExistentes);
        }
        [HttpPost]
        public IActionResult InsertPlataforma(int idPlataforma, string plataformas, string descripcion, int valorventa, int valorneto, DateTime fechaInipago, DateTime fechaFinpago, int cantidad, string correo, string contrasena, int cedula, int estado)
        {
            if (string.IsNullOrEmpty(descripcion) || valorventa <= 0 || valorneto <= 0 || fechaInipago == DateTime.MinValue || fechaFinpago == DateTime.MinValue || cantidad <= 0 || string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena) || cedula <= 0 || estado <= 0)
            {
                var mensaje = "Error: Hay campos sin informacion digitada, revisar todo lo llenado.";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            } else
            {
                _productoservice.InserPlataformaService(idPlataforma, descripcion, valorventa, valorneto, fechaInipago, fechaFinpago, cantidad, correo, contrasena, cedula, estado);
                return View("FormPlataforma");
            }
        }
        public IActionResult FormInserClienPlatf()
        {
            var traerPlataformas = _productoservice.SuscripcionesActivas();
            return View(traerPlataformas);
        }
        public IActionResult InsertVentClientPltf(string nombrecliente, string celularcliente, string correo, string contrasena, int idPltfSuscripcion, int cantidad, string ppm, DateTime feciniplat, DateTime fecfinplat, int valorventa, int valorneto, int cedula, int estado, string clave)
        {
            _productoservice.ServicioInsertarVentClientPlataforma(nombrecliente, celularcliente, correo, contrasena, idPltfSuscripcion, cantidad, ppm, feciniplat, fecfinplat, valorventa, valorneto, cedula, estado, clave);
            return RedirectToAction("formInserClienPlatf", "Producto");
        }
        public IActionResult FormVisuPlatf()
        {
            var searchPlataform = _productoservice.TraerPlataformasExistentes();
            return View(searchPlataform);
        }
        public IActionResult FormVisuCta()
        {
            var searchPlataform = _productoservice.TraerPlataformasExistentes();
            return View(searchPlataform);
        }
        [HttpGet]
        public async Task<IActionResult> GetSuscripcionesActivas(int plataformaId)
        {
            var suscripciones = await _productoservice.ObtenerSuscripcionesActivas(plataformaId);
            return Json(suscripciones);
        }

        // Obtener datos de clientes relacionados con una suscripción
        [HttpGet]
        public async Task<IActionResult> GetDatosSuscripcion(int suscripcionId)
        {
            var datos = await _productoservice.ObtenerDatosSuscripcion(suscripcionId);
            return Json(datos);
        }
        [HttpGet]
        public async Task<IActionResult> GetDatosPlataforma(int suscripcionId)
        {
            var datos = await _productoservice.ObtenerDatosPlataforma(suscripcionId);
            return Json(datos);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var resultado = await _productoservice.EliminarClienteAsync(id);
            if (resultado)
            {
                return Ok(new { message = "Cliente eliminado con éxito." });
            }
            return BadRequest(new { message = "Error al eliminar el cliente o cliente no encontrado." });
        }
        [HttpPost]
        public async Task<IActionResult> EditarEstadoCta(int id, int estado, int idCliente)
        {
            //Console.WriteLine("IDCLIENTEPLATAFORMA: " + id + " ESTADO: " + estado + " IDCLIENTE " + idCliente);
            await _productoservice.ActualizarCliente(id, estado, idCliente);
            return Json(new { success = true });
        }
        //fin plataformas de streaming
        //Visualizar productos existentes para vender en la pagina inicial
        [HttpGet]
        public IActionResult ProductosExistentesVenta(int IdServicio)
        {
            var traerCategoriasExistentes = _productoservice.ObtenerCategoriaProductos(IdServicio);
            return View("../Home/productosExistentesVenta", traerCategoriasExistentes);
        }
        public IActionResult TraerProductoXCategoria(int categoria)
        {
            var productosTraidos = _productoservice.TraerProductosXCategoria(categoria);
            return PartialView("../Home/_ProductosParciales", productosTraidos);
        }
        [HttpGet]
        public async Task<IActionResult> CategoriaProductos()
        {
            var servicios = await _productoservice.ObtenerServiciosAsync();

            var viewModel = new CategoriaProductosViewModel
            {
                Servicios = servicios
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCate(CategoriaProductosViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Servicios = await _productoservice.ObtenerServiciosAsync();
                return View(model);
            }

            var categoria = new CategoriaProductos
            {
                Descripcion = model.Descripcion,
                IdServicio = model.IdServicio
            };

            var exito = await _productoservice.CrearCategoriaAsync(categoria);

            if (exito)
            {
                TempData["Mensaje"] = "Categoría creada correctamente.";
                return RedirectToAction("CategoriaProductos"); // o una vista de categorías
            }

            ModelState.AddModelError("", "No se pudo crear la categoría.");
            model.Servicios = await _productoservice.ObtenerServiciosAsync();
            return View(model);
        }
        public async Task<IActionResult> CreateService()
        {
            var servicios = await _productoservice.ObtenerServicios();
            return View(servicios);
        }

        [HttpGet]
        public IActionResult FormCrearServicio()
        {
            return PartialView("_FormCrearServicio", new Servicio());
        }

        [HttpPost]
        public async Task<IActionResult> CrearServicio(Servicio servicio)
        {
            if (ModelState.IsValid)
            {
                var nuevoServicio = await _productoservice.CrearServicio(servicio);
                return PartialView("_ServicioRow", nuevoServicio);
            }

            return BadRequest("Error al crear servicio");
        }
        // Vista principal con filtro por sede
        public async Task<IActionResult> ProductosLista(int? sedeId, string? q)
        {
                var sedes = await _dbContext.Sede.Where(s => s.Estado == 1)
                    .OrderBy(s => s.NombreSede)
                    .Select(s => new SelectListItem { Value = s.Id_sede.ToString(), Text = s.NombreSede })
                    .ToListAsync();
                
                var productos = _dbContext.Productos.ToList();
                var sedesLista = _dbContext.Sede.ToList(); 

                sedes.Insert(0, new SelectListItem { Value = "", Text = "Todas las sedes" });

                var vm = new ProductosIndexVm
                {
                    FiltroSedeId = sedeId,
                    Sedes = sedes,
                    Q = q,
                    Productos = productos,
                    Sede = sedesLista
                };

                return View(vm);
         }

        // Endpoint para AJAX (JSON)
        [HttpGet]
        public async Task<IActionResult> StockAsignacionProductoSede(
        int? sedeId, string? q, int page = 1, int pageSize = 20,
        string? sortBy = null, bool desc = false)
        {
            var pageData = await _productoservice.ObtenerStockAsync(sedeId, q, page, pageSize, sortBy, desc);
            var resumen = await _productoservice.ResumenAsync(sedeId, q);

            return Json(new
            {
                ok = true,
                page = pageData.Page,
                pageSize = pageData.PageSize,
                totalRows = pageData.TotalRows,
                totalPages = pageData.TotalPages,
                totalUnidades = resumen.totalUnidades,
                skusConStock = resumen.skusConStock,
                data = pageData.Rows
            });
        }
        // Exportar a Excel (con los mismos filtros q/sedeId, sin paginación)
        [HttpGet]
        public async Task<IActionResult> ExportarExcel(int? sedeId, string? q, string? sortBy = null, bool desc = false)
        {
            // Trae TODO lo filtrado para exportar (pageSize muy grande o segundo método Get-all)
            var all = await _productoservice.ObtenerStockAsync(sedeId, q, page: 1, pageSize: int.MaxValue, sortBy, desc);

            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("Inventario");

            int col = 1, row = 1;
            ws.Cell(row, col++).Value = "SKU";
            ws.Cell(row, col++).Value = "Producto";
            if (sedeId != null) ws.Cell(row, col++).Value = "Sede";
            ws.Cell(row, col++).Value = "Cantidad";
            ws.Row(row).Style.Font.Bold = true;

            foreach (var r in all.Rows)
            {
                row++; col = 1;
                ws.Cell(row, col++).Value = r.Nombre;
                if (sedeId != null) ws.Cell(row, col++).Value = r.SedeNombre ?? "";
                ws.Cell(row, col++).Value = r.Cantidad;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"Inventario_{(sedeId?.ToString() ?? "Todas")}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> InsertarLote([FromBody] List<ProductoInsertDto> lote)
        {
            if (lote == null || lote.Count == 0) return Json(new { ok = false, msg = "Lote vacío" });

            var (insertados, omitidos) = await _productoservice.InsertarLoteAsync(lote);
            return Json(new { ok = true, insertados, omitidos });
        }

        // Catálogos para selects
        [HttpGet]
        public async Task<IActionResult> GetServicios()
        {
            var data = await _productoservice.GetServiciosAsync();
            return Json(data.Select(x => new { id = x.IdServicio, nombre = x.DescripcionServicio }));
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoriasPorServicio(int servicioId)
        {
            var data = await _productoservice.GetCategoriasPorServicioAsync(servicioId);
            return Json(data.Select(x => new { id = x.IdCateProducto, nombre = x.Descripcion }));
        }

        [HttpGet]
        public async Task<IActionResult> GetProveedores()
        {
            var data = await _productoservice.GetProveedoresAsync();
            return Json(data.Select(x => new { id = x.IdProveedor, nombre = x.RazonSocial }));
        }
        [HttpPost]
        public IActionResult AsignacionSedeProducto(string producto, int sede, int cantidad, int valorUnitario)
        {
            var cedulaClaim = User.FindFirst("Cedula")?.Value;
            var validarSede = _productoservice.ValidarSedeAsignacionProducto(sede);
            if(validarSede)
            {
                var validarProducto = _productoservice.ValidarProductoAsignacion(producto);
                if(validarProducto)
                {
                    var validarCantidadProducto = _productoservice.ValidarCantidadProducto(producto, cantidad);
                    if (validarCantidadProducto)
                    {
                        var resultado = _productoservice.AsignarProductoSede(producto, sede, cantidad, valorUnitario, cedulaClaim);
                        return RedirectToAction("ProductosLista");
                    }else
                    {
                        var mensaje = "Error: La cantidad que desea asignar es mayor a la que tiene el producto en bodega.";
                        TempData["ErrorMessage"] = mensaje;
                        return RedirectToAction("Error", "Errores");
                    }
                    
                }
                else
                {
                    var mensaje = "Error: El producto no existe.";
                    TempData["ErrorMessage"] = mensaje;
                    return RedirectToAction("Error", "Errores");
                }
                    
            }
            else
            {
                var mensaje = "Error: La sede no existe.";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }
        }
        [HttpGet]
        public IActionResult HabilitarProducto()
        {
            var productosInactivos = _productoservice.TraerProductosInactivos();
            return View(productosInactivos);
        }
        [HttpPost]
        public IActionResult CambiarEstado(string codigo)
        {
            var producto = _dbContext.Productos.FirstOrDefault(p => p.Cod_Producto == codigo);
            if (producto != null)
            {
                producto.Estado = producto.Estado == 0 ? 1 : 0; // alternar
                _dbContext.SaveChanges();
            }

            return RedirectToAction("HabilitarProducto");
        }
    }
}

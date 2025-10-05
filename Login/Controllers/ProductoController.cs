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
                Proveedores = await _productoservice.ObtenerProveedores(),
                Empresas = await _productoservice.TraerEmpresas()
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Insertar(string id_empresa, string codigo, string descripcion, decimal? valor_neto, decimal? valor_unitario, decimal stock, int categorias, int id_proveedor, IFormFile imagen, string? autenticidadProducto, string? condicionProducto)
        {

                // 1️⃣ Buscar la categoría por id
                var categoria = await _dbContext.CategoriaProductos.FindAsync(categorias);
                if (categoria == null)
                    return Json(new { success = false, message = "Categoría no encontrada." });

                // 2️⃣ Buscar el servicio usando el idServicio que está en la categoría
                var servicio = await _dbContext.Servicio.FindAsync(categoria.IdServicio);
                if (servicio == null)
                    return Json(new { success = false, message = "Servicio no encontrado." });

                string? rutaImagen = null;
                if (imagen != null && imagen.Length > 0)
                {
                    var extension = Path.GetExtension(imagen.FileName).ToLower();
                    if (extension != ".jpg" && extension != ".png")
                        return Json(new { success = false, message = "Solo se permiten imágenes .jpg o .png" });

                    string servicioFolder = servicio.NombreServicio.Replace(" ", "_");
                    string categoriaFolder = categoria.Descripcion.Replace(" ", "_");

                    // Crear carpeta dinámica
                    var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(),
                                                   "wwwroot", "img", "Productos",
                                                   servicioFolder, categoriaFolder);

                    if (!Directory.Exists(uploadsPath))
                        Directory.CreateDirectory(uploadsPath);

                    var fileName = $"{codigo}{extension}";
                    var filePath = Path.Combine(uploadsPath, fileName);

                    // 3.1️⃣ Si existe una imagen anterior, eliminarla
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    // 3.2️⃣ Guardar la nueva imagen
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imagen.CopyToAsync(stream);
                    }

                    rutaImagen = $"/img/Productos/{servicioFolder}/{categoriaFolder}/{fileName}";
                }else
                {
                    rutaImagen = $"/img/Productos/nodisponible.png";
                }

                    // ✅ Llamar al servicio y guardar el producto
                    var resultado = await _productoservice.AgregarProductoAsync(
                        id_empresa, codigo, descripcion, valor_neto, valor_unitario,
                        stock, categorias, id_proveedor, rutaImagen, autenticidadProducto, condicionProducto
                    );

                return Json(new { success = resultado });
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
        public async Task<IActionResult> Editar(string searchTerm)
        {
            int categoriaTerm = 0;
            var editarProducto = _productoservice.BuscarProductos(searchTerm, categoriaTerm);
            if (!editarProducto.Any())
            {
                Console.WriteLine("No hay productos con ese código referenciado");
                return View("Index");
            }

            var p = editarProducto.First();

            // 1️⃣ Servicios: mover el servicio actual a la primera posición
            var servicios = await _productoservice.ObtenerServiciosAsync();
            var selectedServicioId = await _productoservice.SeleccionarServicio(p);
            if (selectedServicioId.HasValue)
            {
                servicios = servicios
                    .OrderByDescending(s => s.IdServicio == selectedServicioId.Value) // el actual va primero
                    .ThenBy(s => s.NombreServicio)
                    .ToList();
            }

            // 2️⃣ Proveedores: mover el proveedor actual a la primera posición
            var proveedores = await _productoservice.ObtenerProveedores();
            if (p.idProveedor != 0)
            {
                proveedores = proveedores
                    .OrderByDescending(pr => pr.IdProveedor == p.idProveedor)
                    .ThenBy(pr => pr.RazonSocial)
                    .ToList();
            }

            // Cargar en ViewBag
            ViewBag.Servicios = servicios;
            ViewBag.Proveedores = proveedores;
            ViewBag.SelectedServicioId = selectedServicioId;
            ViewBag.SelectedProveedorId = p.idProveedor;

            return View(editarProducto); // @model List<Producto>
        }
        [HttpPost]
        public async Task<IActionResult> EditarProducto(string codigo, decimal? valorNeto, decimal? valorVenta, int valorUnidad, int cantidad, int categorias, int id_proveedor, IFormFile imagen)
        {
            // 1️⃣ Buscar la categoría por id
            var categoria = await _dbContext.CategoriaProductos.FindAsync(categorias);
            if (categoria == null)
                return Json(new { success = false, message = "Categoría no encontrada." });

            // 2️⃣ Buscar el servicio usando el idServicio que está en la categoría
            var servicio = await _dbContext.Servicio.FindAsync(categoria.IdServicio);
            if (servicio == null)
                return Json(new { success = false, message = "Servicio no encontrado." });

            string? rutaImagen = null;
            if (imagen != null && imagen.Length > 0)
            {
                var extension = Path.GetExtension(imagen.FileName).ToLower();
                if (extension != ".jpg" && extension != ".png")
                    return Json(new { success = false, message = "Solo se permiten imágenes .jpg o .png" });

                string servicioFolder = servicio.NombreServicio.Replace(" ", "_");
                string categoriaFolder = categoria.Descripcion.Replace(" ", "_");

                // Crear carpeta dinámica
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(),
                                               "wwwroot", "img", "Productos",
                                               servicioFolder, categoriaFolder);

                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                var fileName = $"{codigo}{extension}";
                var filePath = Path.Combine(uploadsPath, fileName);

                // 3.1️⃣ Si existe una imagen anterior, eliminarla
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // 3.2️⃣ Guardar la nueva imagen
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imagen.CopyToAsync(stream);
                }

                rutaImagen = $"/img/Productos/{servicioFolder}/{categoriaFolder}/{fileName}";
            }
            else
            {
                rutaImagen = $"/img/Productos/nodisponible.png";
            }
            // Llama al método EditarProducto del servicio de productos
            _productoservice.EditarProducto(codigo, valorNeto, valorVenta, valorUnidad, cantidad, categorias, id_proveedor, rutaImagen);

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

            // Llama al nuevo método que devuelve un solo producto con su categoría y servicio
            var viewModel = _productoservice.BuscarProductoXImagen(searchTerm, categoriaTerm);

            return View(viewModel);
        }
        /*Visualizacion de  Recargas de Plataformas */
        [Authorize]
        [HttpGet]
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
        [Authorize]
        [HttpGet]
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
        [Authorize]
        [HttpGet]
        public IActionResult FormVisuPlatf()
        {
            var searchPlataform = _productoservice.TraerPlataformasExistentes();
            return View(searchPlataform);
        }
        [Authorize]
        [HttpGet]
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
        public IActionResult AsignacionSedeProducto(string producto, int sede, int cantidad, int valorUnitario, int valorNeto)
        {
            var cedulaClaim = User.FindFirst("Cedula")?.Value;
            var validarSede = _productoservice.ValidarSedeAsignacionProducto(sede);
            if(validarSede)
            {
                var validarProducto = _productoservice.ValidarProductoAsignacion(producto);
                if(validarProducto)
                {
                    var validarProductoSede = _productoservice.ValidarProductoSede(producto, sede);
                    if (!validarProductoSede) 
                    {

                        var validarCantidadProducto = _productoservice.ValidarCantidadProducto(producto, cantidad);
                        if (validarCantidadProducto)
                        {
                            var resultado = _productoservice.AsignarProductoSede(producto, sede, cantidad, valorUnitario, cedulaClaim, valorNeto);
                            return RedirectToAction("ProductosLista");
                        }
                        else
                        {
                            var mensaje = "Error: La cantidad que desea asignar es mayor a la que tiene el producto en bodega.";
                            TempData["ErrorMessage"] = mensaje;
                            return RedirectToAction("Error", "Errores");
                        }

                    }
                    else
                    {
                        var mensaje = "Error: Ya existe una relacion entre la sede y el producto creados, editar la informacion internamente.";
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
        [HttpGet]
        public async Task<IActionResult> BuscarProductoPorCodigoVenta(string codigo)
        {
            var productos = await _productoservice.BuscarProductosPorCodigo(codigo);
            var resultados = productos.Select(p => new {
                label = $"{p.Cod_Producto} - {p.NombreProducto}",
                value = p.Cod_Producto,
                valorNeto = p.ValorNetoProducto,
                valorVenta = p.ValorVentaProducto
            });

            return Json(resultados);
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerProductoPorId(string id)
        {
            var producto = await _dbContext.InventarioSedes
                .Include(i => i.Producto)
                .FirstOrDefaultAsync(i => i.ProductoId == id);

            if (producto == null) return NotFound();

            return Json(new
            {
                productoId = producto.ProductoId,
                nombre = producto.Producto.NombreProducto,
                valorNeto = producto.ValorNeto,
                valorVenta = producto.PrecioUnitario,
                cantidad = producto.Cantidad
            });
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarProducto([FromForm] ProductoUpdateDto model)
        {
            var productoInventarioSede = await _dbContext.InventarioSedes
                .FirstOrDefaultAsync(i => i.ProductoId == model.ProductoId);

            var producto = await _dbContext.Productos
                .FirstOrDefaultAsync(i => i.Cod_Producto == model.ProductoId);
            // 5 >= 2
            if (producto.CantidadProducto >= model.Cantidad)
            {
                if (producto == null) return Json(new { ok = false });

                productoInventarioSede.ValorNeto = (int?)model.ValorNeto;
                productoInventarioSede.PrecioUnitario = (int?)model.ValorVenta;
                //10 = 10+5 = 15
                productoInventarioSede.Cantidad = productoInventarioSede.Cantidad + model.Cantidad;
                //5 = 5-5 = 0
                producto.CantidadProducto = producto.CantidadProducto - model.Cantidad;
                await _dbContext.SaveChangesAsync();

                return Json(new { ok = true });
            }else
            {
                var mensaje = "Error: La cantidad que desea asignar es mayor a la que tiene el producto en bodega.";
                TempData["ErrorMessage"] = mensaje;
                return RedirectToAction("Error", "Errores");
            }
        }
    }
}

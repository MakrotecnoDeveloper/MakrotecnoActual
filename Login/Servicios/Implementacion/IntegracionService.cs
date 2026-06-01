using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using MakroTecno.ViewModels.Integraciones;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using System.Runtime.InteropServices;
namespace Plataforma.Servicios.Implementacion
{
    public class IntegracionService : IIntegracionService
    {
        private readonly BaseAdmContext _dbContext;
        private readonly IWebHostEnvironment _env;
        public IntegracionService(BaseAdmContext dbContext, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _env = env;
        }
        // =====================================================
        // TIENDAS RAPPI
        // =====================================================

        public async Task<List<RappiTiendaViewModel>> ObtenerTiendasRappiAsync()
        {
            return await _dbContext.RappiTienda
                .Include(x => x.Sede)
                .OrderBy(x => x.NombreTiendaRappi)
                .Select(x => new RappiTiendaViewModel
                {
                    Id = x.Id,
                    IdTiendaRappi = x.IdTiendaRappi,
                    NombreTiendaRappi = x.NombreTiendaRappi,
                    SedeId = x.SedeId,
                    NombreSede = x.Sede.NombreSede,
                    Activa = x.Activa
                })
                .ToListAsync();
        }

        public async Task<RappiTiendaViewModel> PrepararTiendaRappiAsync(int? id = null)
        {
            var vm = new RappiTiendaViewModel();

            if (id.HasValue)
            {
                var tienda = await _dbContext.RappiTienda
                    .FirstOrDefaultAsync(x => x.Id == id.Value);

                if (tienda != null)
                {
                    vm.Id = tienda.Id;
                    vm.IdTiendaRappi = tienda.IdTiendaRappi;
                    vm.NombreTiendaRappi = tienda.NombreTiendaRappi;
                    vm.SedeId = tienda.SedeId;
                    vm.Activa = tienda.Activa;
                }
            }

            vm.Sedes = await _dbContext.Sede
                .OrderBy(x => x.NombreSede)
                .Select(x => new SelectListItem
                {
                    Value = x.Id_sede.ToString(),
                    Text = x.NombreSede
                })
                .ToListAsync();

            return vm;
        }

        public async Task GuardarTiendaRappiAsync(RappiTiendaViewModel vm)
        {
            if (vm.Id == 0)
            {
                var tienda = new RappiTienda
                {
                    IdTiendaRappi = vm.IdTiendaRappi,
                    NombreTiendaRappi = vm.NombreTiendaRappi,
                    SedeId = vm.SedeId,
                    Activa = vm.Activa
                };

                _dbContext.RappiTienda.Add(tienda);
            }
            else
            {
                var tienda = await _dbContext.RappiTienda
                    .FirstOrDefaultAsync(x => x.Id == vm.Id);

                if (tienda == null)
                    throw new Exception("No se encontró la tienda Rappi.");

                tienda.IdTiendaRappi = vm.IdTiendaRappi;
                tienda.NombreTiendaRappi = vm.NombreTiendaRappi;
                tienda.SedeId = vm.SedeId;
                tienda.Activa = vm.Activa;
            }

            await _dbContext.SaveChangesAsync();
        }

        // =====================================================
        // CATEGORÍAS RAPPI
        // =====================================================

        public async Task<List<RappiCategoriaMapeoViewModel>> ObtenerCategoriasMapeoAsync()
        {
            return await _dbContext.RappiCategoriaMapeo
                .Include(x => x.CategoriaProducto)
                .OrderBy(x => x.CategoriaProducto.Descripcion)
                .Select(x => new RappiCategoriaMapeoViewModel
                {
                    Id = x.Id,
                    IdCateProducto = x.IdCateProducto,
                    CategoriaErpNombre = x.CategoriaProducto.Descripcion,
                    CategoriaRappi = x.CategoriaRappi,
                    Activo = x.Activo
                })
                .ToListAsync();
        }

        public async Task<RappiCategoriaMapeoViewModel> PrepararCategoriaMapeoAsync(int? id = null)
        {
            var vm = new RappiCategoriaMapeoViewModel();

            if (id.HasValue)
            {
                var map = await _dbContext.RappiCategoriaMapeo
                    .FirstOrDefaultAsync(x => x.Id == id.Value);

                if (map != null)
                {
                    vm.Id = map.Id;
                    vm.IdCateProducto = map.IdCateProducto;
                    vm.CategoriaRappi = map.CategoriaRappi;
                    vm.Activo = map.Activo;
                }
            }

            vm.CategoriasErp = await _dbContext.CategoriaProductos
                .OrderBy(x => x.Descripcion)
                .Select(x => new SelectListItem
                {
                    Value = x.IdCateProducto.ToString(),
                    Text = x.Descripcion
                })
                .ToListAsync();

            return vm;
        }

        public async Task GuardarCategoriaMapeoAsync(RappiCategoriaMapeoViewModel vm)
        {
            if (vm.Id == 0)
            {
                var existe = await _dbContext.RappiCategoriaMapeo
                    .AnyAsync(x => x.IdCateProducto == vm.IdCateProducto);

                if (existe)
                    throw new Exception("Esta categoría ERP ya tiene una categoría Rappi asociada.");

                var map = new RappiCategoriaMapeo
                {
                    IdCateProducto = vm.IdCateProducto,
                    CategoriaRappi = vm.CategoriaRappi,
                    Activo = vm.Activo
                };

                _dbContext.RappiCategoriaMapeo.Add(map);
            }
            else
            {
                var map = await _dbContext.RappiCategoriaMapeo
                    .FirstOrDefaultAsync(x => x.Id == vm.Id);

                if (map == null)
                    throw new Exception("No se encontró la homologación de categoría.");

                map.IdCateProducto = vm.IdCateProducto;
                map.CategoriaRappi = vm.CategoriaRappi;
                map.Activo = vm.Activo;
            }

            await _dbContext.SaveChangesAsync();
        }

        // =====================================================
        // PRODUCTOS CONFIG RAPPI
        // =====================================================

        public async Task<List<RappiProductoConfigViewModel>> ObtenerProductosConfigRappiAsync()
        {
            return await _dbContext.RappiProductoConfig
                .Include(x => x.Producto)
                .OrderBy(x => x.Producto.NombreProducto)
                .Select(x => new RappiProductoConfigViewModel
                {
                    Id = x.Id,
                    ProductoId = x.ProductoId,
                    NombreProducto = x.Producto.NombreProducto,
                    SkuRappi = x.SkuRappi,
                    NombreRappi = x.NombreRappi,
                    MarcaRappi = x.MarcaRappi,
                    Ean = x.Ean,
                    PublicarEnRappi = x.PublicarEnRappi,
                    Activo = x.Activo
                })
                .ToListAsync();
        }

        public async Task<RappiProductoConfigViewModel> PrepararProductoConfigAsync(int? id = null)
        {
            var vm = new RappiProductoConfigViewModel();

            if (id.HasValue)
            {
                var config = await _dbContext.RappiProductoConfig
                    .Include(x => x.Producto)
                    .FirstOrDefaultAsync(x => x.Id == id.Value);

                if (config != null)
                {
                    vm.Id = config.Id;
                    vm.ProductoId = config.ProductoId;
                    vm.NombreProducto = config.Producto.NombreProducto;
                    vm.SkuRappi = config.SkuRappi;
                    vm.NombreRappi = config.NombreRappi;
                    vm.DescripcionRappi = config.DescripcionRappi;
                    vm.MarcaRappi = config.MarcaRappi;
                    vm.Ean = config.Ean;
                    vm.EsPesable = config.EsPesable;
                    vm.EsPreempaquetado = config.EsPreempaquetado;
                    vm.CantidadPresentacion = config.CantidadPresentacion;
                    vm.UnidadMedidaRappi = config.UnidadMedidaRappi;
                    vm.PublicarEnRappi = config.PublicarEnRappi;
                    vm.Activo = config.Activo;
                }
            }

            vm.Productos = await _dbContext.Productos
                .Where(x => x.Estado == 1)
                .OrderBy(x => x.NombreProducto)
                .Select(x => new SelectListItem
                {
                    Value = x.Cod_Producto,
                    Text = x.Cod_Producto + " - " + x.NombreProducto
                })
                .ToListAsync();

            return vm;
        }

        public async Task GuardarProductoConfigAsync(RappiProductoConfigViewModel vm)
        {
            if (vm.Id == 0)
            {
                var existe = await _dbContext.RappiProductoConfig
                    .AnyAsync(x => x.ProductoId == vm.ProductoId);

                if (existe)
                    throw new Exception("Este producto ya tiene configuración para Rappi.");

                var config = new RappiProductoConfig
                {
                    ProductoId = vm.ProductoId,
                    SkuRappi = vm.SkuRappi,
                    NombreRappi = vm.NombreRappi,
                    DescripcionRappi = vm.DescripcionRappi,
                    MarcaRappi = vm.MarcaRappi,
                    Ean = vm.Ean,
                    EsPesable = vm.EsPesable,
                    EsPreempaquetado = vm.EsPreempaquetado,
                    CantidadPresentacion = vm.CantidadPresentacion,
                    UnidadMedidaRappi = vm.UnidadMedidaRappi,
                    PublicarEnRappi = vm.PublicarEnRappi,
                    Activo = vm.Activo
                };

                _dbContext.RappiProductoConfig.Add(config);
            }
            else
            {
                var config = await _dbContext.RappiProductoConfig
                    .FirstOrDefaultAsync(x => x.Id == vm.Id);

                if (config == null)
                    throw new Exception("No se encontró la configuración del producto.");

                config.ProductoId = vm.ProductoId;
                config.SkuRappi = vm.SkuRappi;
                config.NombreRappi = vm.NombreRappi;
                config.DescripcionRappi = vm.DescripcionRappi;
                config.MarcaRappi = vm.MarcaRappi;
                config.Ean = vm.Ean;
                config.EsPesable = vm.EsPesable;
                config.EsPreempaquetado = vm.EsPreempaquetado;
                config.CantidadPresentacion = vm.CantidadPresentacion;
                config.UnidadMedidaRappi = vm.UnidadMedidaRappi;
                config.PublicarEnRappi = vm.PublicarEnRappi;
                config.Activo = vm.Activo;
            }

            await _dbContext.SaveChangesAsync();
        }

        // =====================================================
        // VALIDACIÓN PRODUCTOS PERSONALIZADOS RAPPI
        // =====================================================

        public async Task<RappiExportResultadoViewModel> ValidarProductosPersonalizadosRappiAsync(
            int rappiTiendaId,
            bool soloConStock)
        {
            var resultado = new RappiExportResultadoViewModel();

            var tienda = await _dbContext.RappiTienda
                .FirstOrDefaultAsync(x => x.Id == rappiTiendaId && x.Activa);

            if (tienda == null)
            {
                resultado.Exitoso = false;
                resultado.Mensaje = "La tienda Rappi no existe o no está activa.";
                return resultado;
            }

            var query = _dbContext.InventarioSedes
                .Include(x => x.Producto)
                    .ThenInclude(p => p.Unidad)
                .Where(x => x.SedeId == tienda.SedeId)
                .Where(x => x.Producto.Estado == 1)
                .Where(x => x.Producto.EstadoWeb == 1);

            if (soloConStock)
            {
                query = query.Where(x => x.Cantidad > 0);
            }

            var inventarios = await query.ToListAsync();

            var categoriasMapeo = await _dbContext.RappiCategoriaMapeo
                .Where(x => x.Activo)
                .ToListAsync();

            var configs = await _dbContext.RappiProductoConfig
                .Where(x => x.Activo && x.PublicarEnRappi)
                .ToListAsync();

            var productosExport = new List<RappiProductoExportViewModel>();

            foreach (var inv in inventarios)
            {
                var producto = inv.Producto;

                var config = configs.FirstOrDefault(x => x.ProductoId == producto.Cod_Producto);
                var categoriaMap = categoriasMapeo.FirstOrDefault(x => x.IdCateProducto == producto.IdCatepro);

                var item = new RappiProductoExportViewModel
                {
                    Categoria = categoriaMap?.CategoriaRappi ?? string.Empty,
                    Nombre = !string.IsNullOrWhiteSpace(config?.NombreRappi)
                        ? config.NombreRappi
                        : producto.NombreProducto ?? string.Empty,
                    Sku = !string.IsNullOrWhiteSpace(config?.SkuRappi)
                        ? config.SkuRappi
                        : producto.Cod_Producto ?? string.Empty,
                    Marca = config?.MarcaRappi,
                    Ean = config?.Ean,
                    Descripcion = !string.IsNullOrWhiteSpace(config?.DescripcionRappi)
                        ? config.DescripcionRappi
                        : GenerarDescripcionRappi(producto),
                    EsPesable = config?.EsPesable == true ? "SI" : "NO",
                    EsPreempaquetado = config?.EsPreempaquetado == true ? "SI" : "NO",
                    Cantidad = config?.CantidadPresentacion ?? 1,
                    UnidadMedida = config?.UnidadMedidaRappi ?? "Und (unidades)",
                    StockSede = inv.Cantidad,
                    Precio = inv.PrecioUnitario ?? producto.ValorVentaProducto
                };

                ValidarProductoExport(item);

                productosExport.Add(item);
            }

            resultado.Productos = productosExport;
            resultado.TotalProductos = productosExport.Count;
            resultado.TotalValidos = productosExport.Count(x => x.EstadoValidacion == "OK");
            resultado.TotalConErrores = productosExport.Count(x => x.EstadoValidacion != "OK");
            resultado.Exitoso = resultado.TotalConErrores == 0;
            resultado.Mensaje = resultado.Exitoso
                ? "Todos los productos están listos para exportar."
                : "Hay productos con errores o datos pendientes.";

            return resultado;
        }

        public async Task<RappiExportResultadoViewModel> GenerarExcelProductosPersonalizadosRappiAsync(
            int rappiTiendaId,
            bool soloConStock,
            string usuarioId)
        {
            var resultado = await ValidarProductosPersonalizadosRappiAsync(rappiTiendaId, soloConStock);

            if (!resultado.Productos.Any())
            {
                resultado.Exitoso = false;
                resultado.Mensaje = "No hay productos para exportar.";
                return resultado;
            }

            var productosValidos = resultado.Productos
                .Where(x => x.EstadoValidacion == "OK")
                .ToList();

            if (!productosValidos.Any())
            {
                resultado.Exitoso = false;
                resultado.Mensaje = "No hay productos válidos para exportar.";
                return resultado;
            }

            var archivosGenerados = await GenerarArchivosExcelRappiAsync(productosValidos);

            var sincronizacion = new RappiSincronizacion
            {
                UsuarioId = usuarioId,
                ArchivoGenerado = string.Join(";", archivosGenerados),
                TotalFilas = resultado.TotalProductos,
                TotalActualizados = productosValidos.Count,
                TotalSinAsociar = resultado.TotalConErrores,
                Observaciones = "Generación de productos personalizados Rappi."
            };
            resultado.ArchivosGenerados = archivosGenerados;

            foreach (var item in resultado.Productos.Where(x => x.EstadoValidacion != "OK"))
            {
                sincronizacion.Detalles.Add(new RappiSincronizacionDetalle
                {
                    SkuRappi = item.Sku,
                    ProductoId = item.Sku,
                    NombreProductoRappi = item.Nombre,
                    Estado = item.EstadoValidacion,
                    Observacion = item.Observacion
                });
            }

            _dbContext.RappiSincronizacion.Add(sincronizacion);
            await _dbContext.SaveChangesAsync();

            resultado.Exitoso = true;
            resultado.Mensaje = "Validación generada correctamente. Pendiente conectar generación física del Excel.";

            return resultado;
        }

        // =====================================================
        // MÉTODOS PRIVADOS
        // =====================================================

        private async Task<List<string>> GenerarArchivosExcelRappiAsync(List<RappiProductoExportViewModel> productos)
        {
            var archivos = new List<string>();

            var carpeta = Path.Combine(_env.WebRootPath, "exports", "rappi");

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            var lotes = productos
                .Select((producto, index) => new { producto, index })
                .GroupBy(x => x.index / 500)
                .Select(g => g.Select(x => x.producto).ToList())
                .ToList();

            var contador = 1;

            foreach (var lote in lotes)
            {
                using var workbook = new ClosedXML.Excel.XLWorkbook();
                var ws = workbook.Worksheets.Add("Productos");

                // Encabezados en fila 5, datos desde fila 6
                ws.Cell(5, 2).Value = "Categoría";
                ws.Cell(5, 3).Value = "Nombre";
                ws.Cell(5, 4).Value = "SKU";
                ws.Cell(5, 5).Value = "Marca";
                ws.Cell(5, 6).Value = "EAN";
                ws.Cell(5, 7).Value = "Descripción";
                ws.Cell(5, 8).Value = "¿Es pesable?";
                ws.Cell(5, 9).Value = "¿Es preempaquetado?";
                ws.Cell(5, 10).Value = "Cantidad";
                ws.Cell(5, 11).Value = "Unidad de medida";

                var fila = 6;

                foreach (var item in lote)
                {
                    ws.Cell(fila, 2).Value = item.Categoria;
                    ws.Cell(fila, 3).Value = item.Nombre;
                    ws.Cell(fila, 4).Value = item.Sku;
                    ws.Cell(fila, 5).Value = item.Marca;
                    ws.Cell(fila, 6).Value = item.Ean;
                    ws.Cell(fila, 7).Value = item.Descripcion;
                    ws.Cell(fila, 8).Value = item.EsPesable;
                    ws.Cell(fila, 9).Value = item.EsPreempaquetado;
                    ws.Cell(fila, 10).Value = item.Cantidad;
                    ws.Cell(fila, 11).Value = item.UnidadMedida;

                    fila++;
                }

                ws.Columns().AdjustToContents();

                var nombreArchivo = $"Rappi_ProductosPersonalizados_{DateTime.Now:yyyyMMdd_HHmmss}_Parte_{contador}.xlsx";
                var rutaFisica = Path.Combine(carpeta, nombreArchivo);

                workbook.SaveAs(rutaFisica);

                var rutaWeb = $"/exports/rappi/{nombreArchivo}";
                archivos.Add(rutaWeb);

                contador++;
            }

            await Task.CompletedTask;

            return archivos;
        }

        private static string GenerarDescripcionRappi(Producto producto)
        {
            var partes = new List<string>();

            if (!string.IsNullOrWhiteSpace(producto.NombreProducto))
                partes.Add(producto.NombreProducto);

            if (!string.IsNullOrWhiteSpace(producto.CondicionProducto))
                partes.Add($"Condición: {producto.CondicionProducto}");

            if (!string.IsNullOrWhiteSpace(producto.AutenticidadProducto))
                partes.Add($"Autenticidad: {producto.AutenticidadProducto}");

            partes.Add("Producto disponible en MakroTecno.");

            return string.Join(". ", partes);
        }

        private static void ValidarProductoExport(RappiProductoExportViewModel item)
        {
            var errores = new List<string>();

            if (string.IsNullOrWhiteSpace(item.Categoria))
                errores.Add("Falta categoría Rappi.");

            if (string.IsNullOrWhiteSpace(item.Nombre))
                errores.Add("Falta nombre del producto.");

            if (string.IsNullOrWhiteSpace(item.Sku))
                errores.Add("Falta SKU.");

            if (string.IsNullOrWhiteSpace(item.Descripcion))
                errores.Add("Falta descripción.");

            if (item.Cantidad <= 0)
                errores.Add("La cantidad debe ser mayor a cero.");

            if (string.IsNullOrWhiteSpace(item.UnidadMedida))
                errores.Add("Falta unidad de medida.");

            if (!string.IsNullOrWhiteSpace(item.Ean) && !item.Ean.All(char.IsDigit))
                errores.Add("El EAN debe contener solo números.");

            if (errores.Any())
            {
                item.EstadoValidacion = "ERROR";
                item.Observacion = string.Join(" | ", errores);
            }
            else
            {
                item.EstadoValidacion = "OK";
                item.Observacion = null;
            }
        }
        public async Task<RappiActualizacionResultadoViewModel> PrepararActualizacionProductosRappiAsync()
        {
            var vm = new RappiActualizacionResultadoViewModel();

            vm.TiendasRappi = await _dbContext.RappiTienda
                .Where(x => x.Activa)
                .OrderBy(x => x.NombreTiendaRappi)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.NombreTiendaRappi + " - Sede ERP " + x.SedeId
                })
                .ToListAsync();

            return vm;
        }

        public async Task<RappiActualizacionResultadoViewModel> ActualizarProductosRappiDesdeExcelAsync(
            int rappiTiendaId,
            IFormFile archivo,
            string usuarioId)
        {
            var resultado = await PrepararActualizacionProductosRappiAsync();
            resultado.RappiTiendaId = rappiTiendaId;

            if (archivo == null || archivo.Length == 0)
            {
                resultado.Exitoso = false;
                resultado.Mensaje = "Debe cargar el archivo Excel de actualización descargado desde Rappi.";
                return resultado;
            }

            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

            if (extension != ".xlsx")
            {
                resultado.Exitoso = false;
                resultado.Mensaje = "El archivo debe estar en formato .xlsx.";
                return resultado;
            }

            var tienda = await _dbContext.RappiTienda
                .FirstOrDefaultAsync(x => x.Id == rappiTiendaId && x.Activa);

            if (tienda == null)
            {
                resultado.Exitoso = false;
                resultado.Mensaje = "La tienda Rappi no existe o no está activa.";
                return resultado;
            }

            await using var stream = archivo.OpenReadStream();

            using var workbook = new XLWorkbook(stream);

            var ws = workbook.Worksheets.FirstOrDefault(x =>
                x.Name.Equals("Productos", StringComparison.OrdinalIgnoreCase));

            if (ws == null)
            {
                resultado.Exitoso = false;
                resultado.Mensaje = "El Excel no contiene la hoja 'Productos'.";
                return resultado;
            }

            var detalles = new List<RappiActualizacionDetalleViewModel>();

            /*
                Estructura plantilla Rappi:
                Fila 3: encabezados principales.
                Fila 6 en adelante: productos.

                Columnas:
                B = ID
                C = ID de tienda
                D = Nombre de tienda
                E = ID del producto
                F = EAN
                G = SKU
                H = Nombre del producto
                I = Descripción
                J = Presentación
                K = Precio
                L = Descuento
                M = Disponibilidad
            */

            const int primeraFilaProductos = 6;

            var ultimaFila = ws.LastRowUsed()?.RowNumber() ?? 0;

            if (ultimaFila < primeraFilaProductos)
            {
                resultado.Exitoso = false;
                resultado.Mensaje = "No se encontraron productos para actualizar en la hoja Productos.";
                return resultado;
            }

            for (var fila = primeraFilaProductos; fila <= ultimaFila; fila++)
            {
                var skuRappi = ws.Cell(fila, 7).GetString()?.Trim();

                if (string.IsNullOrWhiteSpace(skuRappi))
                    continue;

                var detalle = new RappiActualizacionDetalleViewModel
                {
                    FilaExcel = fila,
                    IdTiendaRappi = ws.Cell(fila, 3).GetString()?.Trim(),
                    NombreTiendaRappi = ws.Cell(fila, 4).GetString()?.Trim(),
                    IdProductoRappi = ws.Cell(fila, 5).GetString()?.Trim(),
                    SkuRappi = skuRappi,
                    SkuErpDetectado = LimpiarSkuRappi(skuRappi),
                    NombreProductoRappi = ws.Cell(fila, 8).GetString()?.Trim(),
                    PrecioAnterior = LeerDecimalCelda(ws.Cell(fila, 11)),
                    DisponibilidadAnterior = ws.Cell(fila, 13).GetString()?.Trim()
                };

                var skuErp = detalle.SkuErpDetectado ?? skuRappi;

                var configRappi = await _dbContext.RappiProductoConfig
                    .Include(x => x.Producto)
                    .FirstOrDefaultAsync(x =>
                        x.Activo &&
                        x.PublicarEnRappi &&
                        (
                            x.SkuRappi == skuRappi ||
                            x.SkuRappi == skuErp ||
                            x.ProductoId == skuErp
                        ));

                Producto? producto = null;

                if (configRappi != null)
                {
                    producto = configRappi.Producto;
                }
                else
                {
                    producto = await _dbContext.Productos
                        .Include(x => x.Unidad)
                        .FirstOrDefaultAsync(x => x.Cod_Producto == skuErp);
                }

                if (producto == null)
                {
                    detalle.Estado = "NO_ENCONTRADO";
                    detalle.Observacion = "No se encontró producto en ERP por SKU Rappi ni por SKU limpio.";
                    detalles.Add(detalle);
                    continue;
                }

                detalle.ProductoErpId = producto.Cod_Producto;
                detalle.NombreProductoErp = producto.NombreProducto;

                var inventario = await _dbContext.InventarioSedes
                    .FirstOrDefaultAsync(x =>
                        x.SedeId == tienda.SedeId &&
                        x.ProductoId == producto.Cod_Producto);

                if (inventario == null)
                {
                    detalle.Estado = "SIN_INVENTARIO_SEDE";
                    detalle.Observacion = $"El producto existe en ERP, pero no tiene inventario registrado en la sede {tienda.SedeId}.";

                    // Se marca NO disponible en Rappi para evitar ventas sin inventario.
                    ws.Cell(fila, 13).Value = "NO";

                    detalle.DisponibilidadNueva = "NO";
                    detalles.Add(detalle);
                    continue;
                }

                var precio = inventario.PrecioUnitario ?? producto.ValorVentaProducto;

                if (precio == null || precio <= 0)
                {
                    detalle.Estado = "SIN_PRECIO";
                    detalle.Observacion = "El producto no tiene precio válido en InventarioSede ni en Producto.";

                    // No se modifica precio si no hay precio confiable.
                    var disponibleSinPrecio = producto.Estado == 1
                        && producto.EstadoWeb == 1
                        && inventario.Cantidad > 0;

                    ws.Cell(fila, 13).Value = disponibleSinPrecio ? "SI" : "NO";
                    detalle.StockSede = inventario.Cantidad;
                    detalle.DisponibilidadNueva = disponibleSinPrecio ? "SI" : "NO";

                    detalles.Add(detalle);
                    continue;
                }

                var disponible = producto.Estado == 1
                    && producto.EstadoWeb == 1
                    && inventario.Cantidad > 0;

                detalle.PrecioNuevo = precio.Value;
                detalle.DescuentoNuevo = 0;
                detalle.StockSede = inventario.Cantidad;
                detalle.DisponibilidadNueva = disponible ? "SI" : "NO";
                detalle.Estado = "OK";
                detalle.Observacion = null;

                // Actualizar únicamente las columnas permitidas por Rappi.
                ws.Cell(fila, 11).Value = precio.Value;               // K = Precio
                ws.Cell(fila, 12).Value = 0;                          // L = Descuento
                ws.Cell(fila, 13).Value = disponible ? "SI" : "NO";  // M = Disponibilidad

                detalles.Add(detalle);
            }

            resultado.Detalles = detalles;
            resultado.TotalFilas = detalles.Count;
            resultado.TotalActualizados = detalles.Count(x => x.Estado == "OK");
            resultado.TotalNoEncontrados = detalles.Count(x => x.Estado == "NO_ENCONTRADO");
            resultado.TotalSinPrecio = detalles.Count(x => x.Estado == "SIN_PRECIO");
            resultado.TotalSinInventario = detalles.Count(x => x.Estado == "SIN_INVENTARIO_SEDE");

            var carpeta = Path.Combine(_env.WebRootPath, "exports", "rappi");

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            var nombreArchivo = $"Rappi_ActualizacionProductos_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            var rutaFisica = Path.Combine(carpeta, nombreArchivo);

            workbook.SaveAs(rutaFisica);

            var rutaWeb = $"/exports/rappi/{nombreArchivo}";
            resultado.ArchivoGenerado = rutaWeb;

            var sincronizacion = new RappiSincronizacion
            {
                UsuarioId = usuarioId,
                ArchivoOriginal = archivo.FileName,
                ArchivoGenerado = rutaWeb,
                TotalFilas = resultado.TotalFilas,
                TotalActualizados = resultado.TotalActualizados,
                TotalSinAsociar = resultado.TotalNoEncontrados + resultado.TotalSinInventario,
                Observaciones = "Actualización de productos existentes Rappi desde Excel."
            };

            foreach (var item in detalles.Where(x => x.Estado != "OK"))
            {
                sincronizacion.Detalles.Add(new RappiSincronizacionDetalle
                {
                    SkuRappi = item.SkuRappi,
                    ProductoId = item.ProductoErpId ?? item.SkuErpDetectado,
                    NombreProductoRappi = item.NombreProductoRappi,
                    Estado = item.Estado,
                    Observacion = item.Observacion
                });
            }

            _dbContext.RappiSincronizacion.Add(sincronizacion);
            await _dbContext.SaveChangesAsync();

            resultado.Exitoso = true;
            resultado.Mensaje = "Archivo de actualización Rappi generado correctamente.";

            return resultado;
        }

        private static string LimpiarSkuRappi(string? sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
                return string.Empty;

            sku = sku.Trim();

            const string prefijoMakrotecno = "Makrotecnomt_";

            if (sku.StartsWith(prefijoMakrotecno, StringComparison.OrdinalIgnoreCase))
                return sku.Substring(prefijoMakrotecno.Length);

            return sku;
        }

        private static decimal? LeerDecimalCelda(IXLCell celda)
        {
            if (celda == null)
                return null;

            if (celda.TryGetValue<decimal>(out var valorDecimal))
                return valorDecimal;

            var texto = celda.GetString();

            if (string.IsNullOrWhiteSpace(texto))
                return null;

            texto = texto
                .Replace("$", "")
                .Replace(" ", "")
                .Trim();

            // Formato colombiano: 12.000,00
            if (texto.Contains(","))
            {
                texto = texto.Replace(".", "").Replace(",", ".");
            }

            if (decimal.TryParse(
                texto,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out var valorParseado))
            {
                return valorParseado;
            }

            return null;
        }
    }
}
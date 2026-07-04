using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Plataforma.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace Plataforma.Services
{
    public class ImportacionService : IImportacionService
    {
        private readonly BaseAdmContext _context;

        public ImportacionService(BaseAdmContext context)
        {
            _context = context;
        }

        public async Task<ResultadoImportacionDto> ProcesarTextoAsync(
            ImportacionProductosViewModel model,
            string usuario
        )
        {
            var campos = ObtenerCamposOrdenados(model);

            var importacion = new ImportacionesProducto
            {
                IdEmpresa = model.IdEmpresa,
                IdProveedor = model.IdProveedor,
                IdCatePro = model.IdCatePro,
                IdUnidad = model.IdUnidad,
                TipoOperacion = model.TipoOperacion,
                TipoEntrada = "Texto",
                TextoOriginal = model.TextoProductos,
                CamposSeleccionados = JsonConvert.SerializeObject(campos),
                Usuario = usuario,
                FechaImportacion = DateTime.Now,
                Estado = 1
            };

            _context.ImportacionesProductos.Add(importacion);
            await _context.SaveChangesAsync();

            var resultado = new ResultadoImportacionDto
            {
                IdImportacion = importacion.IdImportacion,
                TipoOperacion = model.TipoOperacion,
                CamposOrdenadosJson = JsonConvert.SerializeObject(campos)
            };

            if (string.IsNullOrWhiteSpace(model.TextoProductos))
            {
                resultado.Exitoso = false;
                resultado.Mensaje = "Debe ingresar información para importar.";
                return resultado;
            }

            var lineas = model.TextoProductos
                .Split('\n')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            foreach (var linea in lineas)
            {
                try
                {
                    if (!linea.Contains("$"))
                        continue;

                    var producto = ProcesarLineaTexto(linea, model);

                    if (producto == null)
                        continue;

                    await ValidarEstadoProcesoAsync(producto, model.TipoOperacion);

                    resultado.Productos.Add(producto);

                    _context.ImportacionesProductosDetalles.Add(new ImportacionesProductosDetalle
                    {
                        IdImportacion = importacion.IdImportacion,
                        CodProducto = producto.CodProducto,
                        NombreProducto = producto.NombreProducto,
                        LineaOriginal = linea,
                        DatosProcesados = JsonConvert.SerializeObject(producto),
                        EstadoProceso = producto.EstadoProceso,
                        Observacion = producto.Observacion
                    });
                }
                catch (Exception ex)
                {
                    resultado.Errores.Add($"Error en línea: {linea}. Detalle: {ex.Message}");

                    _context.ImportacionesProductosDetalles.Add(new ImportacionesProductosDetalle
                    {
                        IdImportacion = importacion.IdImportacion,
                        LineaOriginal = linea,
                        EstadoProceso = "ERROR",
                        Observacion = ex.Message
                    });
                }
            }

            importacion.TotalRegistros = lineas.Count;
            importacion.TotalProcesados = resultado.Productos.Count;
            importacion.TotalErrores = resultado.Errores.Count;

            await _context.SaveChangesAsync();

            resultado.TotalLeidos = importacion.TotalRegistros;
            resultado.TotalProcesados = importacion.TotalProcesados;
            resultado.TotalErrores = importacion.TotalErrores;
            resultado.Exitoso = true;
            resultado.Mensaje = "Información procesada correctamente. Revise la vista previa antes de confirmar.";

            return resultado;
        }

        public async Task<ResultadoImportacionDto> ProcesarExcelAsync(
            ImportacionProductosViewModel model,
            string usuario
        )
        {
            var campos = ObtenerCamposOrdenados(model);

            var resultado = new ResultadoImportacionDto
            {
                TipoOperacion = model.TipoOperacion,
                CamposOrdenadosJson = JsonConvert.SerializeObject(campos)
            };

            if (model.ArchivoExcel == null || model.ArchivoExcel.Length == 0)
            {
                resultado.Exitoso = false;
                resultado.Mensaje = "Debe adjuntar un archivo Excel.";
                return resultado;
            }

            if (!model.ArchivoExcel.FileName.EndsWith(".xlsx"))
            {
                resultado.Exitoso = false;
                resultado.Mensaje = "El archivo debe ser formato .xlsx.";
                return resultado;
            }

            var importacion = new ImportacionesProducto
            {
                IdEmpresa = model.IdEmpresa,
                IdProveedor = model.IdProveedor,
                IdCatePro = model.IdCatePro,
                IdUnidad = model.IdUnidad,
                TipoOperacion = model.TipoOperacion,
                TipoEntrada = "Excel",
                NombreArchivo = model.ArchivoExcel.FileName,
                CamposSeleccionados = JsonConvert.SerializeObject(campos),
                Usuario = usuario,
                FechaImportacion = DateTime.Now,
                Estado = 1
            };

            _context.ImportacionesProductos.Add(importacion);
            await _context.SaveChangesAsync();

            resultado.IdImportacion = importacion.IdImportacion;

            using var stream = model.ArchivoExcel.OpenReadStream();
            using var workbook = new XLWorkbook(stream);

            var ws = workbook.Worksheets.First();
            var range = ws.RangeUsed();

            if (range == null)
            {
                resultado.Exitoso = false;
                resultado.Mensaje = "El archivo Excel no contiene información.";
                return resultado;
            }

            var rows = range.RowsUsed().Skip(1).ToList();

            foreach (var row in rows)
            {
                try
                {
                    var producto = new ProductoImportadoPreviewDto
                    {
                        Estado = 1,
                        IdEmpresa = model.IdEmpresa,
                        IdCatePro = model.IdCatePro,
                        IdProveedor = model.IdProveedor,
                        IdUnidad = model.IdUnidad,
                        EstadoWeb = 0,
                        Iva = model.Iva ?? 0
                    };

                    for (int i = 0; i < campos.Count; i++)
                    {
                        var campo = campos[i];

                        var valor = row.Cell(i + 1).GetValue<string>()?.Trim();

                        AsignarValorCampo(producto, campo, valor);
                    }

                    producto.NombreProducto = LimpiarTexto(producto.NombreProducto);

                    if (string.IsNullOrWhiteSpace(producto.CodProducto) &&
                        !string.IsNullOrWhiteSpace(producto.NombreProducto))
                    {
                        producto.CodProducto = GenerarCodigoProducto(producto.NombreProducto);
                    }

                    await ValidarEstadoProcesoAsync(producto, model.TipoOperacion);

                    resultado.Productos.Add(producto);

                    _context.ImportacionesProductosDetalles.Add(new ImportacionesProductosDetalle
                    {
                        IdImportacion = importacion.IdImportacion,
                        CodProducto = producto.CodProducto,
                        NombreProducto = producto.NombreProducto,
                        LineaOriginal = string.Join(" | ", row.Cells().Select(c => c.GetValue<string>())),
                        DatosProcesados = JsonConvert.SerializeObject(producto),
                        EstadoProceso = producto.EstadoProceso,
                        Observacion = producto.Observacion
                    });
                }
                catch (Exception ex)
                {
                    resultado.Errores.Add($"Error procesando fila Excel: {ex.Message}");

                    _context.ImportacionesProductosDetalles.Add(new ImportacionesProductosDetalle
                    {
                        IdImportacion = importacion.IdImportacion,
                        EstadoProceso = "ERROR",
                        Observacion = ex.Message
                    });
                }
            }

            importacion.TotalRegistros = rows.Count;
            importacion.TotalProcesados = resultado.Productos.Count;
            importacion.TotalErrores = resultado.Errores.Count;

            await _context.SaveChangesAsync();

            resultado.TotalLeidos = importacion.TotalRegistros;
            resultado.TotalProcesados = importacion.TotalProcesados;
            resultado.TotalErrores = importacion.TotalErrores;
            resultado.Exitoso = true;
            resultado.Mensaje = "Archivo procesado correctamente. Revise la vista previa antes de confirmar.";

            return resultado;
        }

        public async Task<ResultadoImportacionDto> ConfirmarImportacionAsync(
            int idImportacion,
            string tipoOperacion,
            List<string> camposSeleccionados
        )
        {
            var resultado = new ResultadoImportacionDto
            {
                IdImportacion = idImportacion,
                TipoOperacion = tipoOperacion,
                CamposOrdenadosJson = JsonConvert.SerializeObject(camposSeleccionados)
            };

            var importacion = await _context.ImportacionesProductos
                .FirstOrDefaultAsync(x => x.IdImportacion == idImportacion);

            if (importacion == null)
            {
                resultado.Exitoso = false;
                resultado.Mensaje = "No se encontró la importación.";
                return resultado;
            }

            var detalles = await _context.ImportacionesProductosDetalles
                .Where(x => x.IdImportacion == idImportacion)
                .ToListAsync();

            foreach (var detalle in detalles)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(detalle.DatosProcesados))
                    {
                        detalle.EstadoProceso = "OMITIDO";
                        detalle.Observacion = "El registro no contiene datos procesados.";
                        continue;
                    }

                    var item = JsonConvert.DeserializeObject<ProductoImportadoPreviewDto>(detalle.DatosProcesados);

                    if (item == null)
                    {
                        detalle.EstadoProceso = "ERROR";
                        detalle.Observacion = "No fue posible leer los datos procesados.";
                        resultado.TotalErrores++;
                        continue;
                    }

                    var productoExistente = await _context.Productos
                        .FirstOrDefaultAsync(x => x.Cod_Producto == item.CodProducto);

                    if (tipoOperacion == "Crear")
                    {
                        if (productoExistente != null)
                        {
                            detalle.EstadoProceso = "ERROR";
                            detalle.Observacion = "El producto ya existe. No se creó nuevamente.";
                            resultado.Errores.Add($"{item.CodProducto} ya existe.");
                            resultado.TotalErrores++;
                            continue;
                        }

                        var nuevo = new Producto
                        {
                            Cod_Producto = item.CodProducto,
                            NombreProducto = item.NombreProducto,
                            CantidadProducto = (decimal)item.CantidadProducto,
                            ValorNetoProducto = item.ValorNetoProducto,
                            ValorVentaProducto = item.ValorVentaProducto,
                            ValorUnidad = item.ValorUnidad,
                            ID_Empresa = item.IdEmpresa,
                            Estado = item.Estado,
                            Ubicacion = item.Ubicacion,
                            IdCatepro = (int)item.IdCatePro,
                            idProveedor = (int)item.IdProveedor,
                            ImagenPath = item.ImagenPath,
                            AutenticidadProducto = item.AutenticidadProducto,
                            CondicionProducto = item.CondicionProducto,
                            EstadoWeb = (int)item.EstadoWeb,
                            IdUnidad = (int)item.IdUnidad,
                            Iva = item.Iva
                        };

                        _context.Productos.Add(nuevo);

                        detalle.EstadoProceso = "NUEVO";
                        detalle.Observacion = "Producto creado correctamente.";
                        resultado.TotalProcesados++;
                    }
                    else if (tipoOperacion == "Actualizar")
                    {
                        if (productoExistente == null)
                        {
                            detalle.EstadoProceso = "NO_EXISTE";
                            detalle.Observacion = "No se actualizó porque el producto no existe.";
                            resultado.Errores.Add($"{item.CodProducto} no existe.");
                            resultado.TotalErrores++;
                            continue;
                        }

                        AplicarCamposSeleccionados(
                            productoExistente,
                            item,
                            camposSeleccionados
                        );

                        detalle.EstadoProceso = "ACTUALIZADO";
                        detalle.Observacion = "Producto actualizado correctamente.";
                        resultado.TotalProcesados++;
                    }

                    resultado.Productos.Add(item);
                }
                catch (Exception ex)
                {
                    detalle.EstadoProceso = "ERROR";
                    detalle.Observacion = ex.Message;
                    resultado.Errores.Add(ex.Message);
                    resultado.TotalErrores++;
                }
            }

            importacion.TotalProcesados = resultado.TotalProcesados;
            importacion.TotalErrores = resultado.TotalErrores;

            await _context.SaveChangesAsync();

            resultado.TotalLeidos = detalles.Count;
            resultado.Exitoso = true;
            resultado.Mensaje = "Proceso de importación confirmado correctamente.";

            return resultado;
        }

        private void AplicarCamposSeleccionados(
            Producto productoExistente,
            ProductoImportadoPreviewDto item,
            List<string> camposSeleccionados
        )
        {
            if (camposSeleccionados.Contains("cod_producto"))
                productoExistente.Cod_Producto = item.CodProducto;

            if (camposSeleccionados.Contains("nombreProducto"))
                productoExistente.NombreProducto = item.NombreProducto;

            if (camposSeleccionados.Contains("cantidadProducto"))
                productoExistente.CantidadProducto = (decimal)item.CantidadProducto;

            if (camposSeleccionados.Contains("valorNetoProducto"))
                productoExistente.ValorNetoProducto = (decimal)item.ValorNetoProducto;

            if (camposSeleccionados.Contains("valorVentaProducto"))
                productoExistente.ValorVentaProducto = item.ValorVentaProducto;

            if (camposSeleccionados.Contains("valorUnidad"))
                productoExistente.ValorUnidad = item.ValorUnidad;

            if (camposSeleccionados.Contains("id_empresa"))
                productoExistente.ID_Empresa = item.IdEmpresa;

            if (camposSeleccionados.Contains("estado"))
                productoExistente.Estado = item.Estado;

            if (camposSeleccionados.Contains("Ubicacion"))
                productoExistente.Ubicacion = item.Ubicacion;

            if (camposSeleccionados.Contains("idCatePro"))
                productoExistente.IdCatepro = (int)item.IdCatePro;

            if (camposSeleccionados.Contains("idProveedor"))
                productoExistente.idProveedor = (int)item.IdProveedor;

            if (camposSeleccionados.Contains("imagenPath"))
                productoExistente.ImagenPath = item.ImagenPath;

            if (camposSeleccionados.Contains("AutenticidadProducto"))
                productoExistente.AutenticidadProducto = item.AutenticidadProducto;

            if (camposSeleccionados.Contains("CondicionProducto"))
                productoExistente.CondicionProducto = item.CondicionProducto;

            if (camposSeleccionados.Contains("EstadoWeb"))
                productoExistente.EstadoWeb = (int)item.EstadoWeb;

            if (camposSeleccionados.Contains("IdUnidad"))
                productoExistente.IdUnidad = (int)item.IdUnidad;

            if (camposSeleccionados.Contains("Iva"))
                productoExistente.Iva = item.Iva;
        }

        private async Task ValidarEstadoProcesoAsync(
            ProductoImportadoPreviewDto producto,
            string tipoOperacion
        )
        {
            var existe = await _context.Productos
                .AnyAsync(x => x.Cod_Producto == producto.CodProducto);

            if (tipoOperacion == "Crear")
            {
                producto.EstadoProceso = existe ? "Error" : "Nuevo";
                producto.Observacion = existe
                    ? "El producto ya existe. No debería crearse nuevamente."
                    : "Producto nuevo listo para crear.";
            }
            else
            {
                producto.EstadoProceso = existe ? "Actualizar" : "NoExiste";
                producto.Observacion = existe
                    ? "Producto listo para actualizar."
                    : "El producto no existe. Debe crearse primero.";
            }
        }

        private ProductoImportadoPreviewDto ProcesarLineaTexto(
            string linea,
            ImportacionProductosViewModel model
        )
        {
            var partes = linea.Split('$');

            if (partes.Length < 2)
                return null;

            var nombreOriginal = partes[0].Trim();
            var precioTexto = partes[1].Trim();

            var nombreLimpio = LimpiarTexto(nombreOriginal);
            var precio = ConvertirPrecio(precioTexto);

            if (string.IsNullOrWhiteSpace(nombreLimpio) || precio <= 0)
                return null;

            var valorVenta = CalcularValorVenta(precio, model.MargenPorcentaje ?? 0);

            return new ProductoImportadoPreviewDto
            {
                CodProducto = GenerarCodigoProducto(nombreLimpio),
                NombreProducto = nombreLimpio,
                CantidadProducto = 0,
                ValorNetoProducto = precio,
                ValorUnidad = precio,
                ValorVentaProducto = valorVenta,
                IdEmpresa = model.IdEmpresa,
                Estado = 1,
                Ubicacion = null,
                IdCatePro = model.IdCatePro,
                IdProveedor = model.IdProveedor,
                ImagenPath = null,
                AutenticidadProducto = DetectarAutenticidad(nombreLimpio),
                CondicionProducto = DetectarCondicion(nombreLimpio),
                EstadoWeb = 0,
                IdUnidad = model.IdUnidad,
                Iva = model.Iva ?? 0,
                LineaOriginal = linea
            };
        }

        private List<string> ObtenerCamposOrdenados(ImportacionProductosViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.CamposOrdenadosJson))
            {
                var campos = JsonConvert.DeserializeObject<List<string>>(model.CamposOrdenadosJson);

                if (campos != null && campos.Any())
                    return campos;
            }

            return model.CamposSeleccionados ?? new List<string>();
        }

        private void AsignarValorCampo(
            ProductoImportadoPreviewDto producto,
            string campo,
            string valor
        )
        {
            valor = LimpiarTexto(valor);

            switch (campo)
            {
                case "cod_producto":
                    producto.CodProducto = valor;
                    break;

                case "nombreProducto":
                    producto.NombreProducto = valor;
                    break;

                case "cantidadProducto":
                    producto.CantidadProducto = ConvertirDecimal(valor);
                    break;

                case "valorNetoProducto":
                    producto.ValorNetoProducto = ConvertirPrecio(valor);
                    break;

                case "valorVentaProducto":
                    producto.ValorVentaProducto = ConvertirPrecio(valor);
                    break;

                case "valorUnidad":
                    producto.ValorUnidad = ConvertirPrecio(valor);
                    break;

                case "id_empresa":
                    producto.IdEmpresa = valor;
                    break;

                case "estado":
                    producto.Estado = (int)(ConvertirDecimal(valor) ?? 1);
                    break;

                case "Ubicacion":
                    producto.Ubicacion = valor;
                    break;

                case "idCatePro":
                    producto.IdCatePro = (int?)ConvertirDecimal(valor);
                    break;

                case "idProveedor":
                    producto.IdProveedor = (int?)ConvertirDecimal(valor);
                    break;

                case "imagenPath":
                    producto.ImagenPath = valor;
                    break;

                case "AutenticidadProducto":
                    producto.AutenticidadProducto = valor;
                    break;

                case "CondicionProducto":
                    producto.CondicionProducto = valor;
                    break;

                case "EstadoWeb":
                    producto.EstadoWeb = (int?)ConvertirDecimal(valor);
                    break;

                case "IdUnidad":
                    producto.IdUnidad = (int?)ConvertirDecimal(valor);
                    break;

                case "Iva":
                    producto.Iva = ConvertirDecimal(valor);
                    break;
            }
        }

        public string LimpiarTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            var limpio = texto.Normalize(NormalizationForm.FormD);

            limpio = Regex.Replace(limpio, @"[^\w\s\-\+\./]", " ");
            limpio = Regex.Replace(limpio, @"\s+", " ");

            return limpio.Trim();
        }

        public decimal ConvertirPrecio(string precioTexto)
        {
            if (string.IsNullOrWhiteSpace(precioTexto))
                return 0;

            var match = Regex.Match(precioTexto, @"[\d\.\,]+");

            if (!match.Success)
                return 0;

            var limpio = match.Value
                .Replace(".", "")
                .Replace(",", "");

            if (!decimal.TryParse(limpio, out var valor))
                return 0;

            if (valor > 0 && valor < 10000)
                valor *= 1000;

            return valor;
        }

        private decimal? ConvertirDecimal(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return null;

            texto = texto.Replace(".", "").Replace(",", ".");

            if (decimal.TryParse(texto, out var valor))
                return valor;

            return null;
        }

        private decimal CalcularValorVenta(decimal valorBase, decimal margen)
        {
            if (margen <= 0)
                return valorBase;

            var valor = valorBase + (valorBase * margen / 100);

            return Math.Ceiling(valor / 1000) * 1000;
        }

        public string GenerarCodigoProducto(string nombreProducto)
        {
            if (string.IsNullOrWhiteSpace(nombreProducto))
                return Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();

            var limpio = LimpiarTexto(nombreProducto).ToUpper();

            var prefijo = DetectarPrefijoCodigo(limpio);

            var palabrasIgnorar = new[]
            {
                "GB", "G", "SIM", "FISICA", "ESIM", "E", "DOBLE",
                "DE", "CON", "WIFI", "ORIGINAL", "REPLICA",
                "NUEVO", "USADO", "CPO", "PLUS"
            };

            var partes = limpio
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(x => !palabrasIgnorar.Contains(x))
                .ToList();

            var abreviado = "";

            foreach (var parte in partes.Take(4))
            {
                if (parte.All(char.IsDigit))
                {
                    abreviado += "-" + parte;
                }
                else if (parte.Length <= 4)
                {
                    abreviado += parte;
                }
                else
                {
                    abreviado += parte.Substring(0, Math.Min(4, parte.Length));
                }
            }

            abreviado = Regex.Replace(abreviado, @"[^A-Z0-9\-]", "");
            abreviado = abreviado.Trim('-');

            var codigo = $"{prefijo}-{abreviado}";

            if (codigo.Length > 255)
                codigo = codigo.Substring(0, 255);

            return codigo;
        }

        private string DetectarPrefijoCodigo(string nombre)
        {
            if (nombre.Contains("SCOOTER")) return "SCOOTER";
            if (nombre.Contains("AIRE ACONDICIONADO")) return "AIREAC";
            if (nombre.Contains("IPHONE")) return "CEL";
            if (nombre.Contains("XIAOMI")) return "CEL";
            if (nombre.Contains("REDMI")) return "CEL";
            if (nombre.Contains("SAMSUNG")) return "CEL";
            if (nombre.Contains("MOTO")) return "CEL";
            if (nombre.Contains("HONOR")) return "CEL";
            if (nombre.Contains("ZTE")) return "CEL";
            if (nombre.Contains("OPPO")) return "CEL";
            if (nombre.Contains("TECNO")) return "CEL";
            if (nombre.Contains("TABLET")) return "TAB";
            if (nombre.Contains("RELOJ")) return "REL";
            if (nombre.Contains("PORTATIL")) return "PORT";
            if (nombre.Contains("TV")) return "TV";
            if (nombre.Contains("PARLANTE")) return "PARL";
            if (nombre.Contains("CONSOLA")) return "CONS";

            return "PROD";
        }

        private string DetectarCondicion(string nombre)
        {
            var texto = nombre.ToUpper();

            if (texto.Contains("USADO"))
                return "Usado";

            if (texto.Contains("CPO"))
                return "CPO";

            return "Nuevo";
        }

        private string DetectarAutenticidad(string nombre)
        {
            var texto = nombre.ToUpper();

            if (texto.Contains("REPLICA"))
                return "Replica";

            if (texto.Contains("ORIGINAL"))
                return "Original";

            return null;
        }
    }
}
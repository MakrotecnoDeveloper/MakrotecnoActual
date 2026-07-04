using DocumentFormat.OpenXml.InkML;
using MakroTecno.Models;
using MakroTecno.ViewModels.Compras;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Servicios.Implementacion
{
    public class ComprasService : IComprasService
    {
        private readonly BaseAdmContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ComprasService(BaseAdmContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<List<Proveedores>> ObtenerProveedoresAsync()
        {
            return await _dbContext.Set<Proveedores>().ToListAsync();
        }

        public async Task<Proveedores> BuscarProveedorPorIdAsync(int idProveedor)
        {
            return await _dbContext.Set<Proveedores>().FindAsync(idProveedor);
        }

        public async Task<List<Producto>> BuscarProductosPorCodigoAsync(string codigo)
        {
            return await _dbContext.Set<Producto>()
                                 .Where(p => p.Cod_Producto.Contains(codigo))
                                 .ToListAsync();
        }

        public async Task<bool> InsertarCompraAsync(CompraViewModel model)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var subtotal = model.Productos.Sum(p => p.VTotal);

                var total = subtotal;

                if (model.Iva > 0)
                {
                    total = total + ((total * model.Iva) / 100);
                }

                total = total - model.DescuentoFactura;

                var compra = new Compras
                {
                    IdProveedor = model.IdProveedor,
                    ValorTotal = total,
                    FechaCompra = DateTime.Now,
                    Estado = 1,
                    Iva = model.Iva,
                    DescuentoFactura = model.DescuentoFactura,
                    CodFacturaExterno = model.CodFacturaExterno
                };

                _dbContext.Compras.Add(compra);
                await _dbContext.SaveChangesAsync();

                foreach (var producto in model.Productos)
                {
                    var productoExistente = await _dbContext.Productos
                        .FirstOrDefaultAsync(p => p.Cod_Producto == producto.Codigo);

                    if (productoExistente != null)
                    {
                        productoExistente.CantidadProducto += producto.Stock;

                        productoExistente.ValorUnidad = producto.ValorUnidad;
                        productoExistente.ValorVentaProducto = producto.ValorVentaProducto;

                        _dbContext.Productos.Update(productoExistente);
                    }

                    var detalle = new DetalleCompra
                    {
                        IdCompra = compra.IdCompra,
                        Codigo = producto.Codigo,
                        Stock = producto.Stock,

                        // Se mantienen estos campos si DetalleCompra todavía se llama VNeto/VVenta
                        VNeto = producto.ValorUnidad,
                        VVenta = producto.ValorVentaProducto,

                        VTotal = producto.VTotal
                    };

                    _dbContext.DetalleCompras.Add(detalle);
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                Console.WriteLine($"Error InsertarCompraAsync: {ex.Message}");

                return false;
            }
        }

        public async Task<List<Compras>> ObtenerComprasAsync()
        {
            return await _dbContext.Set<Compras>()
                                 .Include(c => c.Detalles)
                                 .ToListAsync();
        }

        public async Task<List<FacturaCompra>> ObtenerFacturasCompraAsync()
        {
            return await _dbContext.FacturaCompra
                .Include(x => x.Proveedor)
                .Where(x => x.Activo)
                .OrderByDescending(x => x.FechaRegistro)
                .ToListAsync();
        }

        public async Task<FacturaCompra?> ObtenerFacturaCompraPorIdAsync(int id)
        {
            return await _dbContext.FacturaCompra
                .Include(x => x.Proveedor)
                .FirstOrDefaultAsync(x => x.IdFacturaCompra == id);
        }

        public async Task CrearFacturaCompraAsync(FacturaCompraViewModel model)
        {
            if (model.ArchivoFactura == null || model.ArchivoFactura.Length == 0)
                throw new Exception("Debe subir una imagen o PDF de la factura.");

            bool existeProveedor = await _dbContext.Proveedores
                .AnyAsync(p => p.IdProveedor == model.IdProveedor);

            if (!existeProveedor)
                throw new Exception("El proveedor seleccionado no existe.");

            bool existeFactura = await _dbContext.FacturaCompra.AnyAsync(x =>
                x.IdProveedor == model.IdProveedor &&
                x.PrefijoFactura == model.PrefijoFactura &&
                x.NumeroFactura == model.NumeroFactura
            );

            if (existeFactura)
                throw new Exception("Ya existe una factura registrada para este proveedor con el mismo prefijo y número.");

            string rutaRelativa = await GuardarArchivoFacturaAsync(model.ArchivoFactura);

            var factura = new FacturaCompra
            {
                IdProveedor = model.IdProveedor,
                PrefijoFactura = model.PrefijoFactura,
                NumeroFactura = model.NumeroFactura,
                FechaCompra = model.FechaCompra,
                Observacion = model.Observacion,
                RutaArchivo = rutaRelativa,
                NombreArchivoOriginal = model.ArchivoFactura.FileName,
                FechaRegistro = DateTime.Now,
                Activo = true,
                ArchivoConservado = true
            };

            _dbContext.FacturaCompra.Add(factura);
            await _dbContext.SaveChangesAsync();
        }

        public async Task ActualizarFacturaCompraAsync(FacturaCompraViewModel model)
        {
            var factura = await _dbContext.FacturaCompra
                .FirstOrDefaultAsync(x => x.IdFacturaCompra == model.IdFacturaCompra && x.Activo);

            if (factura == null)
                throw new Exception("No se encontró la factura de compra.");

            if (model.IdProveedor <= 0)
                throw new Exception("Debe seleccionar un proveedor válido.");

            bool existeOtraFactura = await _dbContext.FacturaCompra.AnyAsync(x =>
                x.IdFacturaCompra != model.IdFacturaCompra &&
                x.IdProveedor == model.IdProveedor &&
                x.PrefijoFactura == model.PrefijoFactura &&
                x.NumeroFactura == model.NumeroFactura
            );

            if (existeOtraFactura)
                throw new Exception("Ya existe otra factura registrada con el mismo proveedor, prefijo y número de factura.");

            factura.NumeroFactura = model.NumeroFactura;
            factura.PrefijoFactura = model.PrefijoFactura;
            factura.IdProveedor = model.IdProveedor;
            factura.FechaCompra = model.FechaCompra;
            factura.Observacion = model.Observacion;

            if (model.ArchivoFactura != null && model.ArchivoFactura.Length > 0)
            {
                string nuevaRuta = await GuardarArchivoFacturaAsync(model.ArchivoFactura);

                factura.RutaArchivo = nuevaRuta;
                factura.NombreArchivoOriginal = model.ArchivoFactura.FileName;
            }

            _dbContext.FacturaCompra.Update(factura);
            await _dbContext.SaveChangesAsync();
        }

        public async Task InactivarFacturaCompraAsync(int id, string? motivoInactivacion = null, string? usuario = null)
        {
            var factura = await _dbContext.FacturaCompra
                .FirstOrDefaultAsync(x => x.IdFacturaCompra == id && x.Activo);

            if (factura == null)
                throw new Exception("No se encontró la factura de compra o ya se encuentra inactiva.");

            factura.Activo = false;
            factura.FechaInactivacion = DateTime.Now;
            factura.MotivoInactivacion = motivoInactivacion;
            factura.UsuarioInactivacion = usuario;
            factura.ArchivoConservado = true;

            _dbContext.FacturaCompra.Update(factura);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<string> GuardarArchivoFacturaAsync(IFormFile archivo)
        {
            string[] extensionesPermitidas = { ".jpg", ".jpeg", ".png", ".webp", ".pdf" };

            string extension = Path.GetExtension(archivo.FileName).ToLower();

            if (!extensionesPermitidas.Contains(extension))
                throw new Exception("Solo se permiten archivos JPG, JPEG, PNG, WEBP o PDF.");

            long maxSize = 5 * 1024 * 1024;

            if (archivo.Length > maxSize)
                throw new Exception("El archivo no puede superar los 5 MB.");

            string carpeta = Path.Combine(
                _webHostEnvironment.WebRootPath,
                "img",
                "facturas-compras"
            );

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            string nombreArchivo = $"{Guid.NewGuid()}{extension}";

            string rutaFisica = Path.Combine(carpeta, nombreArchivo);

            using (var stream = new FileStream(rutaFisica, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            string rutaRelativa = $"/img/facturas-compras/{nombreArchivo}";

            return rutaRelativa;
        }
        public async Task<List<SelectListItem>> ObtenerProveedoresSelectAsync()
        {
            return await _dbContext.Proveedores
                .OrderBy(p => p.RazonSocial)
                .Select(p => new SelectListItem
                {
                    Value = p.IdProveedor.ToString(),
                    Text = p.Nit + " - " + p.RazonSocial
                })
                .ToListAsync();
        }

    }
}

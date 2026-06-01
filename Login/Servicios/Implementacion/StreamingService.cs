using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Models.Dto.Streaming;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Servicios.Implementacion
{
    public class StreamingService : IStreamingService
    {
        private readonly BaseAdmContext _dbContext;
        public StreamingService(BaseAdmContext dbContext)
        {
            _dbContext = dbContext;
        }
        //a
        public void InserPlataformaService(int idPlataforma, string descripcion, int valorventa, int valorneto, DateTime fechaInipago, DateTime fechaFinpago, int cantidad, string correo, string contrasena, int cedula, int estado)
        {
            var nuevaPlataforma = new Plataformasuscripcion
            {
                IdPlataforma = idPlataforma,
                Descripcion = descripcion,
                ValorVenta = valorventa,
                ValorNeto = valorneto,
                FechaIniPago = fechaInipago,
                FechaFinPago = fechaFinpago,
                Cantidad = cantidad,
                Correo = correo,
                Contrasena = contrasena,
                CedulaEmpleado = cedula,
                Estado = estado
            };

            // Agregar el nuevo producto al DbContext y guardar los cambios en la base de datos
            _dbContext.Plataformasuscripcion.Add(nuevaPlataforma);
            _dbContext.SaveChanges();
        }
        public List<Plataformas> TraerPlataformasExistentes()
        {
            return _dbContext.Plataformas.ToList();
        }
        public List<Plataformasuscripcion> SuscripcionesActivas()
        {
            return _dbContext.Plataformasuscripcion.ToList();
        }
        public async Task<List<Plataformasuscripcion>> ObtenerSuscripcionesActivas(int plataformaId)
        {
            // Consultar las suscripciones activas para una plataforma específica
            var suscripciones = await _dbContext.Plataformasuscripcion
                .Where(s => s.Estado == 1 && s.IdPlataforma == plataformaId)
                .ToListAsync();

            return suscripciones;
        }
        public async Task<List<ClientePlataformaDTO>> ObtenerDatosSuscripcion(int suscripcionId)
        {
            var datos = await _dbContext.ClientesPlataforma
                .Where(cp => cp.IdPltfSuscripcion == suscripcionId && cp.Estado == 1)
                .Join(_dbContext.Plataformasuscripcion,
                    cp => cp.IdPltfSuscripcion,
                    ps => ps.IdPltfSuscripcion,
                    (cp, ps) => new ClientePlataformaDTO
                    {
                        IdCliente = cp.IdCliPltf,
                        IdClientePlataforma = ps.IdPlataforma,
                        CorreoPlataforma = ps.Correo,
                        ClavePlataforma = ps.Contrasena,
                        ClavePerfil = cp.ClavePerfil,
                        FechaIni = cp.FechaIniPago,
                        FechaFin = cp.FechaFinPago,
                        Plataforma = ps.IdPlataforma,
                        NombrePlataforma = ps.Descripcion,
                        Estado = cp.Estado,
                        IdPltfSuscripcion = cp.IdPltfSuscripcion
                    })
                .ToListAsync();

            return datos;
        }
        public async Task<List<ClientePlataformaDTO>> ObtenerDatosPlataforma(int suscripcionId)
        {
            var datos = await _dbContext.Plataformasuscripcion
                .Where(ps => ps.IdPltfSuscripcion == suscripcionId && ps.Estado == 1)
                .Select(ps => new ClientePlataformaDTO
                {
                    IdCliente = ps.IdPltfSuscripcion,
                    IdClientePlataforma = ps.IdPlataforma,
                    NombreCliente = ps.Descripcion,
                    CorreoPlataforma = ps.Correo,
                    ClavePlataforma = ps.Contrasena,
                    FechaIni = ps.FechaIniPago,
                    FechaFin = ps.FechaFinPago,
                    Plataforma = ps.Cantidad,
                    Estado = ps.Estado
                })
            .ToListAsync();

            return datos;
        }
        public async Task<bool> EliminarClienteAsync(int idClientePlataforma)
        {
            try
            {
                var cliente = await _dbContext.ClientesPlataforma.FindAsync(idClientePlataforma);
                if (cliente == null)
                {
                    return false; // Cliente no encontrado
                }

                _dbContext.ClientesPlataforma.Remove(cliente);
                await _dbContext.SaveChangesAsync();
                return true; // Eliminado con éxito
            }
            catch (Exception)
            {
                return false; // Manejo de errores
            }
        }
        public void ServicioInsertarVentClientPlataforma(string nombrecliente, string celularcliente, string correo, string contrasena, int idPltfSuscripcion, int cantidad, string ppm, DateTime feciniplat, DateTime fecfinplat, int valorventa, int valorneto, int cedula, int estado, string clave)
        {
            var plataforma = _dbContext.Plataformasuscripcion.SingleOrDefault(p => p.IdPltfSuscripcion == idPltfSuscripcion);
            if (plataforma != null)
            {
                // Si la plataforma existe, actualizar el campo cantidad
                int cantidadActual = plataforma.Cantidad;
                int cantidadNueva = cantidadActual - cantidad;
                if (cantidadNueva < 0)
                {
                    Console.WriteLine("La plataforma no tiene esa cantidad de espacios disponibles");
                }
                else
                {
                    //agregar cuenta
                    var nuevoVentClientPltf = new ClientesPlataforma
                    {
                        IdPltfSuscripcion = idPltfSuscripcion,
                        Cantidad = cantidad,
                        Ppm = ppm,
                        FechaIniPago = feciniplat,
                        FechaFinPago = fecfinplat,
                        ValorVenta = valorventa,
                        ValorNeto = valorneto,
                        CedulaEmpleado = cedula,
                        Estado = estado,
                        ClavePerfil = clave,
                    };

                    // Agregar el nuevo producto al DbContext y guardar los cambios en la base de datos
                    _dbContext.ClientesPlataforma.Add(nuevoVentClientPltf);
                    _dbContext.SaveChanges();

                    //cantidad
                    plataforma.Cantidad = cantidadNueva;
                    // Guardar los cambios en la base de datos
                    _dbContext.SaveChanges();
                }
            }
            else
            {
                Console.WriteLine("Plataforma no encontrada.");
            }
        }
        public async Task ActualizarCliente(int id, int estado, int idCliente)
        {
            // Buscar la plataforma en la base de datos
            var plataforma = await _dbContext.Plataformasuscripcion.SingleOrDefaultAsync(p => p.IdPltfSuscripcion == id);
            if (plataforma != null)
            {
                // Si la plataforma existe, actualizar el campo cantidad
                int cantidadActual = plataforma.Cantidad;
                int cantidadReducida = 1; // Este es el valor que quieres reducir
                int cantidadNueva = cantidadActual + cantidadReducida;
                // Asignar la nueva cantidad a la plataforma
                plataforma.Cantidad = cantidadNueva;

                // Guardar los cambios en la base de datos
                await _dbContext.SaveChangesAsync();
            }
            var clientePlataforma = await _dbContext.ClientesPlataforma.SingleOrDefaultAsync(c => c.IdCliPltf == idCliente);
            if (clientePlataforma != null)
            {
                clientePlataforma.Estado = estado;
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task<List<ClienteStreamingBusquedaDTO>> BuscarClientesStreamingAsync(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino))
            {
                return new List<ClienteStreamingBusquedaDTO>();
            }

            termino = termino.Trim().ToLower();

            var clientesBase = await _dbContext.ClientesStreaming
                .Where(c =>
                    c.Estado == 1 &&
                    (
                        (c.NombreCliente != null && c.NombreCliente.ToLower().Contains(termino)) ||
                        (c.CelularCliente != null && c.CelularCliente.Contains(termino)) ||
                        (c.Correo != null && c.Correo.ToLower().Contains(termino))
                    )
                )
                .Select(c => new
                {
                    c.IdClienteStreaming,
                    c.NombreCliente,
                    c.CelularCliente,
                    c.Correo
                })
                .ToListAsync();

            var clientesAgrupados = clientesBase
                .GroupBy(c => new
                {
                    Nombre = (c.NombreCliente ?? "").Trim().ToLower(),
                    Celular = (c.CelularCliente ?? "").Trim(),
                    Correo = (c.Correo ?? "").Trim().ToLower()
                })
                .Select(g => new ClienteStreamingBusquedaDTO
                {
                    IdsClienteStreaming = string.Join(",", g.Select(x => x.IdClienteStreaming)),
                    NombreCliente = g.FirstOrDefault()?.NombreCliente,
                    CelularCliente = g.FirstOrDefault()?.CelularCliente,
                    CorreoCliente = g.FirstOrDefault()?.Correo
                })
                .OrderBy(c => c.NombreCliente)
                .Take(10)
                .ToList();

            return clientesAgrupados;
        }

        public async Task<List<CuentaClienteStreamingDTO>> ObtenerCuentasPorClientesAsync(string idsClienteStreaming)
        {
            if (string.IsNullOrWhiteSpace(idsClienteStreaming))
            {
                return new List<CuentaClienteStreamingDTO>();
            }

            var ids = idsClienteStreaming
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.TryParse(x, out var id) ? id : 0)
                .Where(x => x > 0)
                .Distinct()
                .ToList();

            if (!ids.Any())
            {
                return new List<CuentaClienteStreamingDTO>();
            }

            var cuentas = await (
                from cp in _dbContext.ClientesPlataforma

                join cs in _dbContext.ClientesStreaming
                    on cp.IdClienteStreaming equals cs.IdClienteStreaming

                join ps in _dbContext.Plataformasuscripcion
                    on cp.IdPltfSuscripcion equals ps.IdPltfSuscripcion

                join p in _dbContext.Plataformas
                    on ps.IdPlataforma equals p.IdPlataforma

                where ids.Contains(cp.IdClienteStreaming)

                select new CuentaClienteStreamingDTO
                {
                    IdClienteStreaming = cs.IdClienteStreaming,
                    NombreCliente = cs.NombreCliente,
                    CelularCliente = cs.CelularCliente,
                    CorreoCliente = cs.Correo,

                    IdClientePlataforma = cp.IdCliPltf,
                    IdPltfSuscripcion = cp.IdPltfSuscripcion,

                    IdPlataforma = p.IdPlataforma,
                    NombrePlataforma = p.NombrePltf,
                    NombreSuscripcion = ps.Descripcion,

                    CorreoPlataforma = ps.Correo,
                    ClavePlataforma = ps.Contrasena,
                    ClavePerfil = cp.ClavePerfil,

                    Cantidad = cp.Cantidad,
                    Ppm = cp.Ppm,

                    FechaIni = cp.FechaIniPago,
                    FechaFin = cp.FechaFinPago,

                    ValorVenta = cp.ValorVenta,
                    ValorNeto = cp.ValorNeto,

                    Estado = cp.Estado
                }
            )
            .OrderBy(c => c.NombrePlataforma)
            .ThenBy(c => c.FechaFin)
            .ToListAsync();

            return cuentas;
        }
        public async Task<(bool ok, string mensaje)> ActualizarCuentaClienteStreamingAsync(ActualizarCuentaClienteStreamingDTO model)
        {
            var cuenta = await _dbContext.ClientesPlataforma
                .FirstOrDefaultAsync(x => x.IdCliPltf == model.IdClientePlataforma);

            if (cuenta == null)
            {
                return (false, "No se encontró la cuenta del cliente.");
            }

            if (model.Cantidad <= 0)
            {
                return (false, "La cantidad debe ser mayor a cero.");
            }

            if (model.FechaFinPago < model.FechaIniPago)
            {
                return (false, "La fecha fin no puede ser menor que la fecha inicio.");
            }

            cuenta.FechaIniPago = model.FechaIniPago;
            cuenta.FechaFinPago = model.FechaFinPago;
            cuenta.ClavePerfil = model.ClavePerfil;
            cuenta.Cantidad = model.Cantidad;
            cuenta.Ppm = model.Ppm;
            cuenta.ValorVenta = model.ValorVenta;
            cuenta.ValorNeto = model.ValorNeto;
            cuenta.Estado = model.Estado;

            await _dbContext.SaveChangesAsync();

            return (true, "Datos de la cuenta actualizados correctamente.");
        }
    }
}
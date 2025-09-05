using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Servicios.Implementacion
{
    public class StreamingPlataformService : IStreamingPlataformService
    {
        private readonly BaseAdmContext _dbContext;
        private readonly ILogger<ProductoService> _logger;
        public StreamingPlataformService(BaseAdmContext dbContext, IConfiguration configuration, ILogger<ProductoService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
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
                        NombreCliente = cp.NombreCliente,
                        CorreoPlataforma = ps.Correo,
                        ClavePlataforma = ps.Contrasena,
                        ClavePerfil = cp.ClavePerfil,
                        Celular = cp.CelularCliente,
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
                        NombreCliente = nombrecliente,
                        CelularCliente = celularcliente,
                        Correo = correo,
                        Clave = contrasena,
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
    }
}

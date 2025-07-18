using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Plataforma.Models;
using Plataforma.Services.Contracts;
using Plataforma.Servicios.Implementacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plataforma.Services
{
    public class PlataformaWebService : IPlataformaWeb
    {
        private readonly BaseAdmContext _dbContext;
        private readonly ILogger<ProductoService> _logger;
        public PlataformaWebService(BaseAdmContext dbContext, IConfiguration configuration, ILogger<ProductoService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
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


    }
}

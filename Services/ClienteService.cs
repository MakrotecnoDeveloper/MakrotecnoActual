using Plataforma.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plataforma.Services
{
    public class ClienteService : IClienteService
    {

        private readonly BaseAdmContext _dbContext;
        public ClienteService(BaseAdmContext dbContext)
        {
            _dbContext = dbContext;
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
    }
}

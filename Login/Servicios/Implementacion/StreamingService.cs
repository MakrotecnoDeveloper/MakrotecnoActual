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
        public async Task<List<CuentaStreamingConsultaDTO>> ConsultarCuentasStreamingAsync(FiltroCuentasStreamingDTO filtro)
        {
            var hoy = DateTime.Today;

            var query =
                from cp in _dbContext.ClientesPlataforma

                join cs in _dbContext.ClientesStreaming
                    on cp.IdClienteStreaming equals cs.IdClienteStreaming

                join ps in _dbContext.Plataformasuscripcion
                    on cp.IdPltfSuscripcion equals ps.IdPltfSuscripcion

                join p in _dbContext.Plataformas
                    on ps.IdPlataforma equals p.IdPlataforma

                select new
                {
                    cp,
                    cs,
                    ps,
                    p
                };

            if (!string.IsNullOrWhiteSpace(filtro.Termino))
            {
                var termino = filtro.Termino.Trim().ToLower();

                query = query.Where(x =>
                    (x.cs.NombreCliente != null && x.cs.NombreCliente.ToLower().Contains(termino)) ||
                    (x.cs.CelularCliente != null && x.cs.CelularCliente.Contains(termino)) ||
                    (x.cs.Correo != null && x.cs.Correo.ToLower().Contains(termino)) ||
                    (x.ps.Correo != null && x.ps.Correo.ToLower().Contains(termino))
                );
            }

            if (filtro.FechaInicio.HasValue)
            {
                var fechaInicio = filtro.FechaInicio.Value.Date;

                query = query.Where(x => x.cp.FechaFinPago.Date >= fechaInicio);
            }

            if (filtro.FechaFin.HasValue)
            {
                var fechaFin = filtro.FechaFin.Value.Date;

                query = query.Where(x => x.cp.FechaFinPago.Date <= fechaFin);
            }

            var indicador = filtro.Indicador?.Trim().ToUpper();

            if (indicador == "VENCIDAS")
            {
                query = query.Where(x => x.cp.FechaFinPago.Date < hoy);
            }
            else if (indicador == "PROXIMAS")
            {
                var limite = hoy.AddDays(5);

                query = query.Where(x =>
                    x.cp.FechaFinPago.Date >= hoy &&
                    x.cp.FechaFinPago.Date <= limite
                );
            }
            else if (indicador == "AL_DIA")
            {
                var limite = hoy.AddDays(5);

                query = query.Where(x => x.cp.FechaFinPago.Date > limite);
            }

            var datos = await query
                .Select(x => new CuentaStreamingConsultaDTO
                {
                    IdClientePlataforma = x.cp.IdCliPltf,
                    IdClienteStreaming = x.cs.IdClienteStreaming,

                    NombreCliente = x.cs.NombreCliente,
                    CelularCliente = x.cs.CelularCliente,
                    CorreoCliente = x.cs.Correo,

                    IdPlataforma = x.p.IdPlataforma,
                    NombrePlataforma = x.p.NombrePltf,

                    IdPltfSuscripcion = x.ps.IdPltfSuscripcion,
                    NombreSuscripcion = x.ps.Descripcion,

                    CorreoPlataforma = x.ps.Correo,
                    ClavePlataforma = x.ps.Contrasena,

                    ClavePerfil = x.cp.ClavePerfil,

                    Cantidad = x.cp.Cantidad,
                    Ppm = x.cp.Ppm,

                    FechaIniPago = x.cp.FechaIniPago,
                    FechaFinPago = x.cp.FechaFinPago,

                    ValorVenta = x.cp.ValorVenta,
                    ValorNeto = x.cp.ValorNeto,

                    Estado = x.cp.Estado,

                    TipoCuenta = x.ps.TipoCuenta
                })
                .OrderBy(x => x.FechaFinPago)
                .ThenBy(x => x.NombreCliente)
                .ToListAsync();

            foreach (var item in datos)
            {
                item.DiasParaVencer = (item.FechaFinPago.Date - hoy).Days;

                if (item.FechaFinPago.Date < hoy)
                {
                    item.EstadoCalculado = "Vencida";
                }
                else if (item.FechaFinPago.Date >= hoy && item.FechaFinPago.Date <= hoy.AddDays(5))
                {
                    item.EstadoCalculado = "Próxima a vencer";
                }
                else
                {
                    item.EstadoCalculado = "Al día";
                }
            }

            return datos;
        }
        public async Task<(bool ok, string mensaje)> RenovarCuentaStreamingAsync(RenovarCuentaStreamingDTO model)
        {
            var clientePlataforma = await _dbContext.ClientesPlataforma
                .FirstOrDefaultAsync(x => x.IdCliPltf == model.IdClientePlataforma);

            if (clientePlataforma == null)
            {
                return (false, "No se encontró la cuenta asignada al cliente.");
            }

            var suscripcion = await _dbContext.Plataformasuscripcion
                .FirstOrDefaultAsync(x => x.IdPltfSuscripcion == clientePlataforma.IdPltfSuscripcion);

            if (suscripcion == null)
            {
                return (false, "No se encontró la suscripción de la plataforma.");
            }

            if (model.FechaFinPago < model.FechaIniPago)
            {
                return (false, "La fecha fin no puede ser menor que la fecha inicio.");
            }

            var tipoCuenta = suscripcion.TipoCuenta?.Trim().ToUpper() ?? "VARIABLE";

            clientePlataforma.FechaIniPago = model.FechaIniPago;
            clientePlataforma.FechaFinPago = model.FechaFinPago;
            clientePlataforma.ValorVenta = model.ValorVenta;
            clientePlataforma.ValorNeto = model.ValorNeto;
            clientePlataforma.Estado = model.Estado;

            if (tipoCuenta == "VARIABLE")
            {
                suscripcion.Correo = model.CorreoPlataforma?.Trim();
                suscripcion.Contrasena = model.ContrasenaPlataforma?.Trim();
                clientePlataforma.ClavePerfil = model.ClavePerfil?.Trim();
            }

            if (tipoCuenta == "FIJA")
            {
                clientePlataforma.ClavePerfil = model.ClavePerfil?.Trim();
            }

            await _dbContext.SaveChangesAsync();

            return (true, "Cuenta renovada correctamente.");
        }
        public async Task<List<EstructuraPlataformaDTO>> ObtenerEstructuraPorPlataformaAsync(int idPlataforma)
        {
            var hoy = DateTime.Today;

            var cuentasBase = await _dbContext.Plataformasuscripcion
                .Where(ps => ps.IdPlataforma == idPlataforma)
                .Join(_dbContext.Plataformas,
                    ps => ps.IdPlataforma,
                    p => p.IdPlataforma,
                    (ps, p) => new
                    {
                        ps,
                        p
                    })
                .OrderBy(x => x.ps.Descripcion)
                .ToListAsync();

            var idsSuscripciones = cuentasBase
                .Select(x => x.ps.IdPltfSuscripcion)
                .ToList();

            var asignaciones = await (
                from cp in _dbContext.ClientesPlataforma

                join cs in _dbContext.ClientesStreaming
                    on cp.IdClienteStreaming equals cs.IdClienteStreaming

                where idsSuscripciones.Contains(cp.IdPltfSuscripcion)

                select new
                {
                    cp,
                    cs
                }
            ).ToListAsync();

            var resultado = cuentasBase.Select(cuenta =>
            {
                var perfiles = asignaciones
                    .Where(x => x.cp.IdPltfSuscripcion == cuenta.ps.IdPltfSuscripcion)
                    .Select(x =>
                    {
                        var dias = (x.cp.FechaFinPago.Date - hoy).Days;

                        string estadoCalculado;

                        if (x.cp.Estado == 0)
                        {
                            estadoCalculado = "Inactiva";
                        }
                        else if (x.cp.FechaFinPago.Date < hoy)
                        {
                            estadoCalculado = "Vencida";
                        }
                        else if (x.cp.FechaFinPago.Date >= hoy && x.cp.FechaFinPago.Date <= hoy.AddDays(5))
                        {
                            estadoCalculado = "Próxima a vencer";
                        }
                        else
                        {
                            estadoCalculado = "Al día";
                        }

                        return new PerfilCuentaDTO
                        {
                            IdClientePlataforma = x.cp.IdCliPltf,
                            IdClienteStreaming = x.cs.IdClienteStreaming,

                            NombreCliente = x.cs.NombreCliente,
                            CelularCliente = x.cs.CelularCliente,
                            CorreoCliente = x.cs.Correo,

                            ClavePerfil = x.cp.ClavePerfil,
                            Ppm = x.cp.Ppm,
                            Cantidad = x.cp.Cantidad,

                            FechaIniPago = x.cp.FechaIniPago,
                            FechaFinPago = x.cp.FechaFinPago,

                            ValorVenta = x.cp.ValorVenta,
                            ValorNeto = x.cp.ValorNeto,

                            Estado = x.cp.Estado,
                            EstadoCalculado = estadoCalculado,
                            DiasParaVencer = dias
                        };
                    })
                    .OrderBy(x => x.FechaFinPago)
                    .ThenBy(x => x.NombreCliente)
                    .ToList();

                var totalAsignados = perfiles.Count;
                var cuposDisponibles = cuenta.ps.Cantidad - totalAsignados;

                if (cuposDisponibles < 0)
                {
                    cuposDisponibles = 0;
                }

                return new EstructuraPlataformaDTO
                {
                    IdPltfSuscripcion = cuenta.ps.IdPltfSuscripcion,
                    IdPlataforma = cuenta.p.IdPlataforma,

                    NombrePlataforma = cuenta.p.NombrePltf,
                    DescripcionCuenta = cuenta.ps.Descripcion,

                    CorreoPlataforma = cuenta.ps.Correo,
                    ContrasenaPlataforma = cuenta.ps.Contrasena,

                    TipoCuenta = cuenta.ps.TipoCuenta,

                    Cantidad = cuenta.ps.Cantidad,
                    ValorVenta = cuenta.ps.ValorVenta,
                    ValorNeto = cuenta.ps.ValorNeto,
                    EstadoCuenta = cuenta.ps.Estado,

                    TotalAsignados = totalAsignados,
                    CuposDisponibles = cuposDisponibles,

                    TotalVencidos = perfiles.Count(x => x.EstadoCalculado == "Vencida"),
                    TotalProximos = perfiles.Count(x => x.EstadoCalculado == "Próxima a vencer"),
                    TotalAlDia = perfiles.Count(x => x.EstadoCalculado == "Al día"),

                    Perfiles = perfiles
                };
            })
            .ToList();

            return resultado;
        }
        public Task<List<Plataformas>> ObtenerPlataformasAsync()
        {
            return _dbContext.Plataformas.ToListAsync();
        }
        public async Task<(bool ok, string mensaje)> ActualizarCuentaPlanAsync(ActualizarCuentaPlanDTO model)
        {
            var cuenta = await _dbContext.Plataformasuscripcion
                .FirstOrDefaultAsync(x => x.IdPltfSuscripcion == model.IdPltfSuscripcion);

            if (cuenta == null)
            {
                return (false, "No se encontró la cuenta / plan.");
            }

            if (string.IsNullOrWhiteSpace(model.Descripcion))
            {
                return (false, "La descripción es obligatoria.");
            }

            if (model.Cantidad <= 0)
            {
                return (false, "La cantidad / cupos debe ser mayor a cero.");
            }

            cuenta.Descripcion = model.Descripcion.Trim();
            cuenta.TipoCuenta = model.TipoCuenta?.Trim();
            cuenta.Correo = model.Correo?.Trim();
            cuenta.Contrasena = model.Contrasena?.Trim();
            cuenta.Cantidad = model.Cantidad;
            cuenta.ValorVenta = model.ValorVenta;
            cuenta.ValorNeto = model.ValorNeto;
            cuenta.Estado = model.Estado;

            await _dbContext.SaveChangesAsync();

            return (true, "Cuenta / plan actualizado correctamente.");
        }
        public async Task<(bool ok, string mensaje)> AsignarClienteACuentaAsync(AsignarClienteACuentaDTO model)
        {
            var cuenta = await _dbContext.Plataformasuscripcion
                .FirstOrDefaultAsync(x => x.IdPltfSuscripcion == model.IdPltfSuscripcion);

            if (cuenta == null)
            {
                return (false, "No se encontró la cuenta / plan.");
            }

            var cliente = await _dbContext.ClientesStreaming
                .FirstOrDefaultAsync(x => x.IdClienteStreaming == model.IdClienteStreaming);

            if (cliente == null)
            {
                return (false, "No se encontró el cliente seleccionado.");
            }

            if (model.FechaFinPago < model.FechaIniPago)
            {
                return (false, "La fecha fin no puede ser menor que la fecha inicio.");
            }

            if (model.Cantidad <= 0)
            {
                return (false, "La cantidad debe ser mayor a cero.");
            }

            var asignados = await _dbContext.ClientesPlataforma
                .CountAsync(x => x.IdPltfSuscripcion == model.IdPltfSuscripcion && x.Estado == 1);

            if (cuenta.Cantidad > 0 && asignados >= cuenta.Cantidad)
            {
                return (false, "Esta cuenta / plan ya no tiene cupos disponibles.");
            }

            var asignacion = new ClientesPlataforma
            {
                IdClienteStreaming = cliente.IdClienteStreaming,
                IdPltfSuscripcion = cuenta.IdPltfSuscripcion,
                FechaIniPago = model.FechaIniPago,
                FechaFinPago = model.FechaFinPago,
                ClavePerfil = model.ClavePerfil?.Trim(),
                Cantidad = model.Cantidad,
                Ppm = model.Ppm,
                ValorVenta = model.ValorVenta,
                ValorNeto = model.ValorNeto,
                Estado = model.Estado
            };

            _dbContext.ClientesPlataforma.Add(asignacion);

            await _dbContext.SaveChangesAsync();

            return (true, "Cliente asignado correctamente a la cuenta / plan.");
        }
        public async Task<(bool ok, string mensaje)> ActualizarPerfilCuentaAsync(ActualizarPerfilCuentaDTO model)
        {
            var perfil = await _dbContext.ClientesPlataforma
                .FirstOrDefaultAsync(x => x.IdCliPltf == model.IdClientePlataforma);

            if (perfil == null)
            {
                return (false, "No se encontró el perfil o participante.");
            }

            if (model.FechaFinPago < model.FechaIniPago)
            {
                return (false, "La fecha fin no puede ser menor que la fecha inicio.");
            }

            if (model.Cantidad <= 0)
            {
                return (false, "La cantidad debe ser mayor a cero.");
            }

            perfil.FechaIniPago = model.FechaIniPago;
            perfil.FechaFinPago = model.FechaFinPago;
            perfil.ClavePerfil = model.ClavePerfil?.Trim();
            perfil.Cantidad = model.Cantidad;
            perfil.Ppm = model.Ppm;
            perfil.ValorVenta = model.ValorVenta;
            perfil.ValorNeto = model.ValorNeto;
            perfil.Estado = model.Estado;

            await _dbContext.SaveChangesAsync();

            return (true, "Perfil actualizado correctamente.");
        }
    }
}
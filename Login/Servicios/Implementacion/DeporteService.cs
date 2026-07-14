using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Plataforma.Data;
using Plataforma.Models;
using Plataforma.Models.ViewModels.Deporte;
using Plataforma.Services.Interfaces;

namespace Plataforma.Services;

public class DeporteService : IDeporteService
{
    private readonly BaseAdmContext _context;

    public DeporteService(BaseAdmContext context)
    {
        _context = context;
    }

    public async Task<CronometroResultadosVm> ConstruirCronometroResultadosAsync(
    string? cedula,
    int? idPruebaDeportiva,
    string? idEmpresa,
    int? idSede)
    {
        var cedulaLimpia = cedula?.Trim();

        var vm = new CronometroResultadosVm
        {
            Cedula = cedulaLimpia,
            BusquedaRealizada = !string.IsNullOrWhiteSpace(cedulaLimpia),
            IdPruebaDeportivaSeleccionada = idPruebaDeportiva,
            Deportes = await ObtenerDeportesAsync(),
            PruebasDeportivas = await ObtenerPruebasDeportivasAsync()
        };

        if (string.IsNullOrWhiteSpace(cedulaLimpia))
            return vm;

        var cliente = await BuscarClientePorCedulaAsync(cedulaLimpia);

        if (cliente == null)
        {
            vm.Mensaje = "No se encontró un cliente/deportista con esa cédula.";
            return vm;
        }

        vm.ClienteEncontrado = true;
        vm.IdCliente = cliente.IdCliente;
        vm.NombreCliente = cliente.NombreCliente ?? "Cliente sin nombre";
        vm.Cedula = cliente.CedulaCliente?.ToString() ?? cedulaLimpia;

        vm.NuevoTiempo.Cedula = vm.Cedula;
        vm.NuevoTiempo.IdPruebaDeportiva = idPruebaDeportiva ?? 0;

        var resultadosQuery = _context.ResultadosDeportivos
            .Include(r => r.Deportista)
            .Include(r => r.SesionTomaTiempo)
                .ThenInclude(s => s.PruebaDeportiva)
                    .ThenInclude(p => p.Deporte)
            .Where(r =>
                r.Deportista.IdCliente == cliente.IdCliente &&
                (idEmpresa == null || r.SesionTomaTiempo.IdEmpresa == idEmpresa));

        if (idPruebaDeportiva.HasValue && idPruebaDeportiva.Value > 0)
        {
            resultadosQuery = resultadosQuery
                .Where(r => r.SesionTomaTiempo.IdPruebaDeportiva == idPruebaDeportiva.Value);
        }

        var resultados = await resultadosQuery
            .OrderByDescending(r => r.SesionTomaTiempo.FechaSesion)
            .ToListAsync();

        vm.Resultados = resultados.Select(r => new ResultadoDeportivoItemVm
        {
            IdResultadoDeportivo = r.IdResultadoDeportivo,
            FechaPrueba = r.SesionTomaTiempo.FechaSesion,
            Deporte = r.SesionTomaTiempo.PruebaDeportiva.Deporte.Nombre,
            Prueba = r.SesionTomaTiempo.PruebaDeportiva.Nombre,
            Tiempo = r.Tiempo,
            Posicion = r.Posicion,
            Observaciones = r.Observaciones
        }).ToList();

        if (resultados.Any())
        {
            vm.MejorTiempo = resultados.Min(r => r.Tiempo);
            vm.UltimoTiempo = resultados
                .OrderByDescending(r => r.SesionTomaTiempo.FechaSesion)
                .First()
                .Tiempo;

            vm.PromedioTiempo = TimeSpan.FromTicks(
                Convert.ToInt64(resultados.Average(r => r.Tiempo.Ticks)));
        }
        else
        {
            vm.Mensaje = "Cliente encontrado. Aún no tiene tiempos registrados para el filtro actual.";
        }

        return vm;
    }

    public async Task<(bool Ok, string Mensaje)> RegistrarTiempoDeportivoAsync(
    RegistrarTiempoDeportivoVm vm,
    string? idEmpresa,
    int? idSede,
    string usuario)
    {
        if (string.IsNullOrWhiteSpace(vm.Cedula))
            return (false, "Digite la cédula del deportista.");

        if (vm.IdPruebaDeportiva <= 0)
            return (false, "Seleccione la prueba deportiva.");

        if (!TryParseTiempo(vm.TiempoTexto, out var tiempo))
            return (false, "El tiempo no tiene un formato válido. Use mm:ss.ff, hh:mm:ss.ff o segundos.");

        var cliente = await BuscarClientePorCedulaAsync(vm.Cedula);

        if (cliente == null)
            return (false, "No se encontró un cliente/deportista con esa cédula.");

        var prueba = await _context.PruebasDeportivas
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IdPruebaDeportiva == vm.IdPruebaDeportiva && p.Activo);

        if (prueba == null)
            return (false, "La prueba deportiva seleccionada no existe o está inactiva.");

        var deportista = await ObtenerOCrearDeportistaAsync(
            cliente.IdCliente,
            prueba.IdDeporte,
            idEmpresa,
            idSede);

        var sesion = new SesionTomaTiempo
        {
            IdPruebaDeportiva = vm.IdPruebaDeportiva,
            FechaSesion = vm.FechaPrueba == default ? DateTime.Now : vm.FechaPrueba,
            IdEmpresa = idEmpresa,
            IdSede = idSede,
            UsuarioRegistro = usuario
        };

        _context.SesionesTomaTiempo.Add(sesion);
        await _context.SaveChangesAsync();

        _context.ResultadosDeportivos.Add(new ResultadoDeportivo
        {
            IdSesionTomaTiempo = sesion.IdSesionTomaTiempo,
            IdDeportista = deportista.IdDeportista,
            Tiempo = tiempo,
            Posicion = 1,
            Observaciones = vm.Observaciones
        });

        await _context.SaveChangesAsync();

        return (true, "Tiempo registrado correctamente.");
    }

    public async Task<(bool Ok, string Mensaje)> RegistrarTiemposGrupoAsync(RegistrarTiemposGrupoVm vm, string? idEmpresa, int? idSede, string usuario)
    {
        if (vm.IdPruebaDeportiva <= 0)
            return (false, "Seleccione la prueba deportiva.");

        var tiemposValidos = vm.Tiempos
            .Where(t => t.IdCliente > 0 && !string.IsNullOrWhiteSpace(t.TiempoTexto))
            .ToList();

        if (!tiemposValidos.Any())
            return (false, "Debe registrar al menos un tiempo.");

        var prueba = await _context.PruebasDeportivas
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IdPruebaDeportiva == vm.IdPruebaDeportiva && p.Activo);

        if (prueba == null)
            return (false, "La prueba deportiva seleccionada no existe o esta inactiva.");

        var sesion = new SesionTomaTiempo
        {
            IdPruebaDeportiva = vm.IdPruebaDeportiva,
            FechaSesion = vm.FechaSesion == default ? DateTime.Now : vm.FechaSesion,
            Observaciones = vm.ObservacionesSesion,
            IdEmpresa = idEmpresa,
            IdSede = idSede,
            UsuarioRegistro = usuario
        };

        _context.SesionesTomaTiempo.Add(sesion);
        await _context.SaveChangesAsync();

        foreach (var item in tiemposValidos)
        {
            if (!TryParseTiempo(item.TiempoTexto, out var tiempo))
                return (false, $"El tiempo del cliente {item.IdCliente} no tiene un formato valido.");

            var deportista = await ObtenerOCrearDeportistaAsync(item.IdCliente, prueba.IdDeporte, idEmpresa, idSede);

            _context.ResultadosDeportivos.Add(new ResultadoDeportivo
            {
                IdSesionTomaTiempo = sesion.IdSesionTomaTiempo,
                IdDeportista = deportista.IdDeportista,
                Tiempo = tiempo,
                Posicion = item.Posicion,
                Observaciones = item.Observaciones
            });
        }

        await _context.SaveChangesAsync();
        return (true, "Tiempos grupales registrados correctamente.");
    }

    public async Task<List<ClienteBusquedaDeportivaVm>> BuscarClientesDeportistasAsync(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return new List<ClienteBusquedaDeportivaVm>();

        texto = texto.Trim();

        var esNumero = int.TryParse(texto, out var cedulaNumero);

        return await _context.Clientes
            .AsNoTracking()
            .Where(c =>
                (esNumero && c.CedulaCliente == cedulaNumero) ||
                (c.NombreCliente != null && c.NombreCliente.Contains(texto)))
            .OrderBy(c => c.NombreCliente)
            .Take(10)
            .Select(c => new ClienteBusquedaDeportivaVm
            {
                IdCliente = c.IdCliente,
                CedulaCliente = c.CedulaCliente.HasValue
                    ? c.CedulaCliente.Value.ToString()
                    : "",
                NombreCompleto = c.NombreCliente ?? "Sin nombre"
            })
            .ToListAsync();
    }

    public async Task<List<SelectListItem>> ObtenerDeportesAsync()
    {
        return await _context.Deportes
            .Where(d => d.Activo)
            .OrderBy(d => d.Nombre)
            .Select(d => new SelectListItem
            {
                Value = d.IdDeporte.ToString(),
                Text = d.Nombre
            })
            .ToListAsync();
    }

    public async Task<List<SelectListItem>> ObtenerPruebasDeportivasAsync(int? idDeporte = null)
    {
        var query = _context.PruebasDeportivas
            .Include(p => p.Deporte)
            .Where(p => p.Activo);

        if (idDeporte.HasValue && idDeporte.Value > 0)
            query = query.Where(p => p.IdDeporte == idDeporte.Value);

        return await query
            .OrderBy(p => p.Deporte.Nombre)
            .ThenBy(p => p.Nombre)
            .Select(p => new SelectListItem
            {
                Value = p.IdPruebaDeportiva.ToString(),
                Text = p.Deporte.Nombre + " - " + p.Nombre
            })
            .ToListAsync();
    }

    private async Task<Deportista> ObtenerOCrearDeportistaAsync(int idCliente, int idDeporte, string? idEmpresa, int? idSede)
    {
        var deportista = await _context.Deportistas.FirstOrDefaultAsync(d =>
            d.IdCliente == idCliente &&
            d.IdDeporte == idDeporte &&
            (idEmpresa == null || d.IdEmpresa == idEmpresa));

        if (deportista != null)
            return deportista;

        deportista = new Deportista
        {
            IdCliente = idCliente,
            IdDeporte = idDeporte,
            IdEmpresa = idEmpresa,
            IdSede = idSede,
            FechaRegistro = DateTime.Now,
            Activo = true
        };

        _context.Deportistas.Add(deportista);
        await _context.SaveChangesAsync();

        return deportista;
    }

    private async Task<Clientes?> BuscarClientePorCedulaAsync(string cedula)
    {
        cedula = cedula.Trim();

        if (!int.TryParse(cedula, out var cedulaNumero))
            return null;

        return await _context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CedulaCliente == cedulaNumero);
    }

    private static string ObtenerNombreCliente(Clientes cliente)
    {
        var nombreCompleto = ObtenerValorPropiedad(cliente, "NombreCompleto", "NombreCliente", "RazonSocial");
        if (!string.IsNullOrWhiteSpace(nombreCompleto))
            return nombreCompleto;

        var nombres = ObtenerValorPropiedad(cliente, "Nombres", "Nombre", "NombreCliente") ?? string.Empty;
        var apellidos = ObtenerValorPropiedad(cliente, "Apellidos", "Apellido", "ApellidoCliente") ?? string.Empty;
        var combinado = $"{nombres} {apellidos}".Trim();

        return string.IsNullOrWhiteSpace(combinado) ? "Cliente sin nombre" : combinado;
    }

    private static string? ObtenerValorPropiedad(object objeto, params string[] nombres)
    {
        foreach (var nombre in nombres)
        {
            var propiedad = objeto.GetType().GetProperty(nombre, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            var valor = propiedad?.GetValue(objeto);
            if (valor != null)
                return Convert.ToString(valor, CultureInfo.InvariantCulture);
        }

        return null;
    }

    private static bool TryParseTiempo(string? texto, out TimeSpan tiempo)
    {
        tiempo = default;
        if (string.IsNullOrWhiteSpace(texto))
            return false;

        texto = texto.Trim().Replace(',', '.');

        if (TimeSpan.TryParseExact(texto, new[] { @"m\:ss\.ff", @"mm\:ss\.ff", @"h\:mm\:ss\.ff", @"hh\:mm\:ss\.ff" }, CultureInfo.InvariantCulture, out tiempo))
            return true;

        if (decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out var segundos))
        {
            tiempo = TimeSpan.FromSeconds((double)segundos);
            return true;
        }

        return TimeSpan.TryParse(texto, CultureInfo.InvariantCulture, out tiempo);
    }
    public async Task<EstilosDeportivosAdminVm> ConstruirEstilosDeportivosAdminAsync(
    int? idDeporte,
    string? buscarDeporte,
    string? buscarEstilo,
    int paginaDeportes,
    int paginaEstilos)
    {
        const int pageSizeDeportes = 10;
        const int pageSizeEstilos = 10;

        paginaDeportes = paginaDeportes <= 0 ? 1 : paginaDeportes;
        paginaEstilos = paginaEstilos <= 0 ? 1 : paginaEstilos;

        var deportesQuery = _context.Deportes
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscarDeporte))
        {
            buscarDeporte = buscarDeporte.Trim();
            deportesQuery = deportesQuery.Where(d => d.Nombre.Contains(buscarDeporte));
        }

        var totalDeportes = await deportesQuery.CountAsync();

        var deportes = await deportesQuery
            .OrderBy(d => d.Nombre)
            .Skip((paginaDeportes - 1) * pageSizeDeportes)
            .Take(pageSizeDeportes)
            .Select(d => new DeporteAdminItemVm
            {
                IdDeporte = d.IdDeporte,
                Nombre = d.Nombre,
                Descripcion = d.Descripcion,
                Activo = d.Activo,
                CantidadEstilos = _context.PruebasDeportivas.Count(p => p.IdDeporte == d.IdDeporte)
            })
            .ToListAsync();

        if (!idDeporte.HasValue && deportes.Any())
            idDeporte = deportes.First().IdDeporte;

        var estilosQuery = _context.PruebasDeportivas
            .Include(p => p.Deporte)
            .AsNoTracking()
            .AsQueryable();

        if (idDeporte.HasValue && idDeporte.Value > 0)
            estilosQuery = estilosQuery.Where(p => p.IdDeporte == idDeporte.Value);

        if (!string.IsNullOrWhiteSpace(buscarEstilo))
        {
            buscarEstilo = buscarEstilo.Trim();

            estilosQuery = estilosQuery.Where(p =>
                p.Nombre.Contains(buscarEstilo) ||
                (p.Modalidad != null && p.Modalidad.Contains(buscarEstilo)) ||
                (p.UnidadMedida != null && p.UnidadMedida.Contains(buscarEstilo)));
        }

        var totalEstilos = await estilosQuery.CountAsync();

        var estilos = await estilosQuery
            .OrderBy(p => p.Deporte.Nombre)
            .ThenBy(p => p.Nombre)
            .Skip((paginaEstilos - 1) * pageSizeEstilos)
            .Take(pageSizeEstilos)
            .Select(p => new EstiloDeportivoItemVm
            {
                IdPruebaDeportiva = p.IdPruebaDeportiva,
                IdDeporte = p.IdDeporte,
                Deporte = p.Deporte.Nombre,
                Nombre = p.Nombre,
                Distancia = p.Distancia,
                UnidadMedida = p.UnidadMedida,
                Modalidad = p.Modalidad,
                Activo = p.Activo
            })
            .ToListAsync();

        return new EstilosDeportivosAdminVm
        {
            IdDeporteSeleccionado = idDeporte,
            BuscarDeporte = buscarDeporte,
            BuscarEstilo = buscarEstilo,
            PaginaDeportes = paginaDeportes,
            PaginaEstilos = paginaEstilos,
            TotalPaginasDeportes = (int)Math.Ceiling(totalDeportes / (double)pageSizeDeportes),
            TotalPaginasEstilos = (int)Math.Ceiling(totalEstilos / (double)pageSizeEstilos),
            Deportes = deportes,
            Estilos = estilos,
            Formulario = new EstiloDeportivoFormVm
            {
                IdDeporte = idDeporte ?? 0,
                Activo = true
            }
        };
    }
    public async Task<(bool Ok, string Mensaje)> CrearEstiloDeportivoAsync(EstiloDeportivoFormVm vm)
    {
        if (vm.IdDeporte <= 0)
            return (false, "Seleccione un deporte.");

        if (string.IsNullOrWhiteSpace(vm.Nombre))
            return (false, "Digite el nombre del estilo o prueba deportiva.");

        var deporteExiste = await _context.Deportes
            .AnyAsync(d => d.IdDeporte == vm.IdDeporte && d.Activo);

        if (!deporteExiste)
            return (false, "El deporte seleccionado no existe o está inactivo.");

        var nombre = vm.Nombre.Trim();

        var existe = await _context.PruebasDeportivas.AnyAsync(p =>
            p.IdDeporte == vm.IdDeporte &&
            p.Nombre == nombre &&
            p.Modalidad == vm.Modalidad);

        if (existe)
            return (false, "Ya existe un estilo/prueba similar para este deporte.");

        var estilo = new PruebaDeportiva
        {
            IdDeporte = vm.IdDeporte,
            Nombre = nombre,
            Distancia = vm.Distancia,
            UnidadMedida = string.IsNullOrWhiteSpace(vm.UnidadMedida) ? null : vm.UnidadMedida.Trim(),
            Modalidad = string.IsNullOrWhiteSpace(vm.Modalidad) ? null : vm.Modalidad.Trim(),
            Activo = vm.Activo
        };

        _context.PruebasDeportivas.Add(estilo);
        await _context.SaveChangesAsync();

        return (true, "Estilo/prueba deportiva creada correctamente.");
    }
    public async Task<(bool Ok, string Mensaje)> ActualizarEstiloDeportivoAsync(EstiloDeportivoFormVm vm)
    {
        if (vm.IdPruebaDeportiva <= 0)
            return (false, "No se recibió el identificador del estilo/prueba.");

        if (vm.IdDeporte <= 0)
            return (false, "Seleccione un deporte.");

        if (string.IsNullOrWhiteSpace(vm.Nombre))
            return (false, "Digite el nombre del estilo o prueba deportiva.");

        var estilo = await _context.PruebasDeportivas
            .FirstOrDefaultAsync(p => p.IdPruebaDeportiva == vm.IdPruebaDeportiva);

        if (estilo == null)
            return (false, "El estilo/prueba deportiva no existe.");

        estilo.IdDeporte = vm.IdDeporte;
        estilo.Nombre = vm.Nombre.Trim();
        estilo.Distancia = vm.Distancia;
        estilo.UnidadMedida = string.IsNullOrWhiteSpace(vm.UnidadMedida) ? null : vm.UnidadMedida.Trim();
        estilo.Modalidad = string.IsNullOrWhiteSpace(vm.Modalidad) ? null : vm.Modalidad.Trim();
        estilo.Activo = vm.Activo;

        await _context.SaveChangesAsync();

        return (true, "Estilo/prueba deportiva actualizada correctamente.");
    }
    public async Task<(bool Ok, string Mensaje)> CambiarEstadoEstiloDeportivoAsync(int idPruebaDeportiva)
    {
        var estilo = await _context.PruebasDeportivas
            .FirstOrDefaultAsync(p => p.IdPruebaDeportiva == idPruebaDeportiva);

        if (estilo == null)
            return (false, "El estilo/prueba deportiva no existe.");

        estilo.Activo = !estilo.Activo;

        await _context.SaveChangesAsync();

        return (true, estilo.Activo
            ? "Estilo/prueba deportiva activada correctamente."
            : "Estilo/prueba deportiva inactivada correctamente.");
    }
}


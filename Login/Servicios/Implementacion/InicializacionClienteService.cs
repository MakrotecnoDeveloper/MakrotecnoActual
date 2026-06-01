using MakroTecno.Models;
using MakroTecno.Models.ViewModels.InicializacionClientes;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;

namespace MakroTecno.Services.InicializacionClientes
{
    public class InicializacionClienteService : IInicializacionClienteService
    {
        private readonly BaseAdmContext _context;

        public InicializacionClienteService(BaseAdmContext context)
        {
            _context = context;
        }

        public async Task<InicializacionClienteViewModel> ObtenerPanelAsync(string? nit)
        {
            var model = new InicializacionClienteViewModel
            {
                NitBuscado = nit,
                BusquedaRealizada = !string.IsNullOrWhiteSpace(nit),
                Planes = await ObtenerPlanesAsync(),
                ActividadesEconomicas = await ObtenerActividadesEconomicasAsync()
            };

            if (string.IsNullOrWhiteSpace(nit))
            {
                model.ModulosDisponibles = await ObtenerModulosDisponiblesAsync(null);
                return model;
            }

            nit = nit.Trim();

            var empresa = await _context.Empresas
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id_empresa == nit);

            if (empresa == null)
            {
                model.ClienteExiste = false;
                model.NuevoCliente.IdEmpresa = nit;
                model.ModulosDisponibles = await ObtenerModulosDisponiblesAsync(null);
                return model;
            }

            model.ClienteExiste = true;

            model.Empresa = await ObtenerEmpresaResumenAsync(nit);
            model.Licencia = await ObtenerLicenciaResumenAsync(nit);
            model.Sedes = await ObtenerSedesAsync(nit);
            model.Pdvs = await ObtenerPdvsAsync(nit);
            model.Empleados = await ObtenerEmpleadosAsync(nit);
            model.ModulosDisponibles = await ObtenerModulosDisponiblesAsync(nit);
            model.ModulosActivos = model.ModulosDisponibles
                .Where(m => m.ActivoParaCliente)
                .ToList();

            return model;
        }

        public async Task<(bool Ok, string Mensaje)> CrearClienteInicialAsync(CrearClienteInicialViewModel model)
        {
            if (model == null)
                return (false, "No se recibió información para crear el cliente.");

            if (string.IsNullOrWhiteSpace(model.IdEmpresa))
                return (false, "El NIT de la empresa es obligatorio.");

            if (string.IsNullOrWhiteSpace(model.NombreEmpresa))
                return (false, "El nombre de la empresa es obligatorio.");

            if (model.IdPlan <= 0)
                return (false, "Debes seleccionar un plan.");

            if (model.ActividadEconomicaId <= 0)
                return (false, "Debes seleccionar una actividad económica.");

            if (model.CedulaAdministrador <= 0)
                return (false, "La cédula del administrador es obligatoria.");

            var idEmpresa = model.IdEmpresa.Trim();

            var existeEmpresa = await _context.Empresas
                .AnyAsync(e => e.Id_empresa == idEmpresa);

            if (existeEmpresa)
                return (false, "Ya existe una empresa registrada con este NIT.");

            var planExiste = await _context.Planes
                .AnyAsync(p => p.IdPlan == model.IdPlan);

            if (!planExiste)
                return (false, "El plan seleccionado no existe.");

            var actividadExiste = await _context.ActividadesEconomicas
                .AnyAsync(a => a.IdActividad == model.ActividadEconomicaId);

            if (!actividadExiste)
                return (false, "La actividad económica seleccionada no existe.");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var empresa = new Empresas
                {
                    Id_empresa = idEmpresa,
                    Nombre = model.NombreEmpresa.Trim(),
                    Pais = string.IsNullOrWhiteSpace(model.Pais) ? "Colombia" : model.Pais.Trim(),
                    Direccion = model.DireccionEmpresa,
                    Telefono = model.TelefonoEmpresa,
                    Estado = "Activo",
                    ActividadEconomicaId = model.ActividadEconomicaId
                };

                _context.Empresas.Add(empresa);
                await _context.SaveChangesAsync();

                var licencia = new LicenciasEmpresa
                {
                    IdEmpresa = idEmpresa,
                    IdPlan = model.IdPlan,
                    FechaInicio = model.FechaInicioLicencia == default
                        ? DateTime.Today
                        : model.FechaInicioLicencia,
                    FechaFin = model.FechaFinLicencia == default
                        ? DateTime.Today.AddYears(1)
                        : model.FechaFinLicencia,
                    Estado = "Activo"
                };

                _context.LicenciasEmpresa.Add(licencia);
                await _context.SaveChangesAsync();

                var sede = new Sede
                {
                    Id_empresa = idEmpresa,
                    NombreSede = string.IsNullOrWhiteSpace(model.NombreSede)
                        ? "Principal"
                        : model.NombreSede.Trim(),
                    Ciudad = model.CiudadSede,
                    Direccion = model.DireccionSede,
                    Telefono = model.TelefonoSede
                };

                _context.Sede.Add(sede);
                await _context.SaveChangesAsync();

                var pdv = new Infopdv
                {
                    NombreInfoPDV = string.IsNullOrWhiteSpace(model.NombrePdv)
                        ? "Caja"
                        : model.NombrePdv.Trim(),
                    Id_Sede = sede.Id_sede
                };

                _context.Infopdv.Add(pdv);
                await _context.SaveChangesAsync();

                var empleadoExistente = await _context.Empleado
                    .FirstOrDefaultAsync(e => e.Cedula == model.CedulaAdministrador);

                if (empleadoExistente == null)
                {
                    var empleado = new Empleados
                    {
                        Cedula = model.CedulaAdministrador,
                        Nombre = model.NombreAdministrador?.Trim(),
                        Apellido = model.ApellidoAdministrador?.Trim(),
                        Genero = model.GeneroAdministrador,
                        Correo = model.CorreoAdministrador,
                        Rh = model.RhAdministrador,
                        Celular = model.CelularAdministrador,
                        Contrasena = string.IsNullOrWhiteSpace(model.ContrasenaAdministrador)
                            ? "123"
                            : model.ContrasenaAdministrador
                    };

                    _context.Empleado.Add(empleado);
                    await _context.SaveChangesAsync();
                }

                var existeEmpleadoEmpresa = await _context.EmpleadoEmpresa
                    .AnyAsync(x => x.Cedula == model.CedulaAdministrador && x.Id_empresa == idEmpresa);

                if (!existeEmpleadoEmpresa)
                {
                    var empleadoEmpresa = new EmpleadoEmpresa
                    {
                        Cedula = model.CedulaAdministrador,
                        Id_empresa = idEmpresa
                    };

                    _context.EmpleadoEmpresa.Add(empleadoEmpresa);
                    await _context.SaveChangesAsync();
                }

                var cargoAdministrador = await _context.TipoCargo
                    .FirstOrDefaultAsync(c =>
                        c.Id_empresa == idEmpresa &&
                        c.NombreCargo == "Administrador");

                if (cargoAdministrador == null)
                {
                    cargoAdministrador = new TipoCargo
                    {
                        NombreCargo = "Administrador",
                        DescripcionCargo = "Administrador del Sistema",
                        Id_empresa = idEmpresa
                    };

                    _context.TipoCargo.Add(cargoAdministrador);
                    await _context.SaveChangesAsync();
                }

                var existeSedeEmpleado = await _context.Sedeempleado
                    .AnyAsync(x =>
                        x.Id_sede == sede.Id_sede &&
                        x.Cedula == model.CedulaAdministrador);

                if (!existeSedeEmpleado)
                {
                    var sedeEmpleado = new Sedeempleado
                    {
                        Id_sede = sede.Id_sede,
                        Cedula = model.CedulaAdministrador,
                        Id_cargo = cargoAdministrador.Id_tipo
                    };

                    _context.Sedeempleado.Add(sedeEmpleado);
                    await _context.SaveChangesAsync();
                }

                await AsignarModulosBaseDelPlanAsync(idEmpresa, model.IdPlan);
                await AsignarModulosPorActividadEconomicaAsync(idEmpresa, model.ActividadEconomicaId);

                await transaction.CommitAsync();

                return (true, "Cliente creado e inicializado correctamente.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return (false, $"No fue posible inicializar el cliente. Detalle: {ex.Message}");
            }
        }

        public async Task<(bool Ok, string Mensaje)> CambiarPlanAsync(string idEmpresa, int idPlan)
        {
            if (string.IsNullOrWhiteSpace(idEmpresa))
                return (false, "No se recibió el NIT de la empresa.");

            if (idPlan <= 0)
                return (false, "No se recibió el plan.");

            idEmpresa = idEmpresa.Trim();

            var empresaExiste = await _context.Empresas
                .AnyAsync(e => e.Id_empresa == idEmpresa);

            if (!empresaExiste)
                return (false, "La empresa no existe.");

            var planExiste = await _context.Planes
                .AnyAsync(p => p.IdPlan == idPlan);

            if (!planExiste)
                return (false, "El plan seleccionado no existe.");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var licenciaActual = await _context.LicenciasEmpresa
                    .Where(l => l.IdEmpresa == idEmpresa && l.Estado == "Activo")
                    .OrderByDescending(l => l.FechaInicio)
                    .FirstOrDefaultAsync();

                if (licenciaActual == null)
                {
                    var nuevaLicencia = new LicenciasEmpresa
                    {
                        IdEmpresa = idEmpresa,
                        IdPlan = idPlan,
                        FechaInicio = DateTime.Today,
                        FechaFin = DateTime.Today.AddYears(1),
                        Estado = "Activo"
                    };

                    _context.LicenciasEmpresa.Add(nuevaLicencia);
                }
                else
                {
                    licenciaActual.IdPlan = idPlan;
                }

                await _context.SaveChangesAsync();

                await SincronizarModulosPorCambioPlanAsync(idEmpresa, idPlan);

                await transaction.CommitAsync();

                return (true, "Plan actualizado correctamente.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"No fue posible cambiar el plan. Detalle: {ex.Message}");
            }
        }

        public async Task<(bool Ok, string Mensaje)> ActivarModuloAsync(string idEmpresa, int idModulo)
        {
            if (string.IsNullOrWhiteSpace(idEmpresa))
                return (false, "No se recibió el NIT de la empresa.");

            if (idModulo <= 0)
                return (false, "No se recibió el módulo.");

            idEmpresa = idEmpresa.Trim();

            var empresaExiste = await _context.Empresas
                .AnyAsync(e => e.Id_empresa == idEmpresa);

            if (!empresaExiste)
                return (false, "La empresa no existe.");

            var moduloExiste = await _context.Modulos
                .AnyAsync(m => m.IdModulo == idModulo);

            if (!moduloExiste)
                return (false, "El módulo no existe.");

            var moduloEmpresa = await _context.ModulosEmpresa
                .FirstOrDefaultAsync(x => x.IdEmpresa == idEmpresa && x.IdModulo == idModulo);

            if (moduloEmpresa == null)
            {
                moduloEmpresa = new ModulosEmpresa
                {
                    IdEmpresa = idEmpresa,
                    IdModulo = idModulo,
                    Activo = true,
                    EsAdicional = true,
                    FechaAsignacion = DateTime.Now
                };

                _context.ModulosEmpresa.Add(moduloEmpresa);
            }
            else
            {
                moduloEmpresa.Activo = true;
                moduloEmpresa.FechaAsignacion = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return (true, "Módulo activado correctamente.");
        }

        public async Task<(bool Ok, string Mensaje)> DesactivarModuloAsync(string idEmpresa, int idModulo)
        {
            if (string.IsNullOrWhiteSpace(idEmpresa))
                return (false, "No se recibió el NIT de la empresa.");

            if (idModulo <= 0)
                return (false, "No se recibió el módulo.");

            idEmpresa = idEmpresa.Trim();

            var moduloEmpresa = await _context.ModulosEmpresa
                .FirstOrDefaultAsync(x => x.IdEmpresa == idEmpresa && x.IdModulo == idModulo);

            if (moduloEmpresa == null)
            {
                moduloEmpresa = new ModulosEmpresa
                {
                    IdEmpresa = idEmpresa,
                    IdModulo = idModulo,
                    Activo = false,
                    EsAdicional = false,
                    FechaAsignacion = DateTime.Now
                };

                _context.ModulosEmpresa.Add(moduloEmpresa);
            }
            else
            {
                moduloEmpresa.Activo = false;
            }

            await _context.SaveChangesAsync();

            return (true, "Módulo desactivado correctamente.");
        }

        private async Task<List<PlanResumenViewModel>> ObtenerPlanesAsync()
        {
            return await _context.Planes
                .AsNoTracking()
                .OrderBy(p => p.PrecioMensual)
                .Select(p => new PlanResumenViewModel
                {
                    IdPlan = p.IdPlan,
                    NombrePlan = p.NombrePlan,
                    PrecioMensual = p.PrecioMensual,
                    LimiteEmpresas = p.LimiteEmpresas,
                    LimiteSedes = p.LimiteSedes,
                    LimitePDV = p.LimitePDV,
                    LimiteUsuarios = p.LimiteUsuarios,
                    LimiteProductos = p.LimiteProductos,
                    LimiteClientes = p.LimiteClientes
                })
                .ToListAsync();
        }

        private async Task<List<ActividadEconomicaResumenViewModel>> ObtenerActividadesEconomicasAsync()
        {
            return await _context.ActividadesEconomicas
                .AsNoTracking()
                .OrderBy(a => a.NombreActividad)
                .Select(a => new ActividadEconomicaResumenViewModel
                {
                    IdActividad = a.IdActividad,
                    NombreActividad = a.NombreActividad,
                    Descripcion = a.Descripcion
                })
                .ToListAsync();
        }

        private async Task<EmpresaResumenViewModel?> ObtenerEmpresaResumenAsync(string idEmpresa)
        {
            return await _context.Empresas
                .AsNoTracking()
                .Where(e => e.Id_empresa == idEmpresa)
                .Select(e => new EmpresaResumenViewModel
                {
                    IdEmpresa = e.Id_empresa,
                    Nombre = e.Nombre,
                    Pais = e.Pais,
                    Direccion = e.Direccion,
                    Telefono = e.Telefono,
                    Estado = e.Estado,
                    ActividadEconomica = _context.ActividadesEconomicas
                        .Where(a => a.IdActividad == e.ActividadEconomicaId)
                        .Select(a => a.NombreActividad)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();
        }

        private async Task<LicenciaResumenViewModel?> ObtenerLicenciaResumenAsync(string idEmpresa)
        {
            return await _context.LicenciasEmpresa
                .AsNoTracking()
                .Where(l => l.IdEmpresa == idEmpresa && l.Estado == "Activo")
                .OrderByDescending(l => l.FechaInicio)
                .Select(l => new LicenciaResumenViewModel
                {
                    IdLicencia = l.IdLicencia,
                    NombrePlan = _context.Planes
                        .Where(p => p.IdPlan == l.IdPlan)
                        .Select(p => p.NombrePlan)
                        .FirstOrDefault() ?? "Sin plan",
                    FechaInicio = l.FechaInicio,
                    FechaFin = l.FechaFin,
                    Estado = l.Estado
                })
                .FirstOrDefaultAsync();
        }

        private async Task<List<SedeResumenViewModel>> ObtenerSedesAsync(string idEmpresa)
        {
            return await _context.Sede
                .AsNoTracking()
                .Where(s => s.Id_empresa == idEmpresa)
                .OrderBy(s => s.NombreSede)
                .Select(s => new SedeResumenViewModel
                {
                    IdSede = s.Id_sede,
                    NombreSede = s.NombreSede,
                    Ciudad = s.Ciudad,
                    Direccion = s.Direccion,
                    Telefono = s.Telefono
                })
                .ToListAsync();
        }

        private async Task<List<PdvResumenViewModel>> ObtenerPdvsAsync(string idEmpresa)
        {
            return await _context.Infopdv
                .AsNoTracking()
                .Where(p => _context.Sede.Any(s => s.Id_sede == p.Id_Sede && s.Id_empresa == idEmpresa))
                .OrderBy(p => p.NombreInfoPDV)
                .Select(p => new PdvResumenViewModel
                {
                    IdInfoPdv = p.InfopdvId,
                    NombreInfoPdv = p.NombreInfoPDV,
                    IdSede = p.Id_Sede,
                    NombreSede = _context.Sede
                        .Where(s => s.Id_sede == p.Id_Sede)
                        .Select(s => s.NombreSede)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }

        private async Task<List<EmpleadoResumenViewModel>> ObtenerEmpleadosAsync(string idEmpresa)
        {
            return await _context.EmpleadoEmpresa
                .AsNoTracking()
                .Where(ee => ee.Id_empresa == idEmpresa)
                .Select(ee => new EmpleadoResumenViewModel
                {
                    Cedula = ee.Cedula,

                    NombreCompleto = _context.Empleado
                        .Where(e => e.Cedula == ee.Cedula)
                        .Select(e => (e.Nombre ?? "") + " " + (e.Apellido ?? ""))
                        .FirstOrDefault() ?? "",

                    Correo = _context.Empleado
                        .Where(e => e.Cedula == ee.Cedula)
                        .Select(e => e.Correo)
                        .FirstOrDefault(),

                    Celular = _context.Empleado
                        .Where(e => e.Cedula == ee.Cedula)
                        .Select(e => e.Celular)
                        .FirstOrDefault(),

                    Cargo = _context.Sedeempleado
                        .Where(se => se.Cedula == ee.Cedula)
                        .Join(
                            _context.TipoCargo,
                            se => se.Id_cargo,
                            tc => tc.Id_tipo,
                            (se, tc) => tc.NombreCargo
                        )
                        .FirstOrDefault(),

                    Sede = _context.Sedeempleado
                        .Where(se => se.Cedula == ee.Cedula)
                        .Join(
                            _context.Sede,
                            se => se.Id_sede,
                            s => s.Id_sede,
                            (se, s) => s.NombreSede
                        )
                        .FirstOrDefault()
                })
                .ToListAsync();
        }

        private async Task<List<ModuloResumenViewModel>> ObtenerModulosDisponiblesAsync(string? idEmpresa)
        {
            var modulos = await _context.Modulos
                .AsNoTracking()
                .OrderBy(m => m.NombreModulo)
                .Select(m => new ModuloResumenViewModel
                {
                    IdModulo = m.IdModulo,
                    NombreModulo = m.NombreModulo,
                    Descripcion = m.Descripcion,
                    EsGenerico = m.EsGenerico,
                    EsEspecializado = m.EsEspecializado,
                    ActivoParaCliente = false
                })
                .ToListAsync();

            if (string.IsNullOrWhiteSpace(idEmpresa))
                return modulos;

            idEmpresa = idEmpresa.Trim();

            var empresa = await _context.Empresas
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id_empresa == idEmpresa);

            if (empresa == null)
                return modulos;

            var licenciaActiva = await _context.LicenciasEmpresa
                .AsNoTracking()
                .Where(l => l.IdEmpresa == idEmpresa && l.Estado == "Activo")
                .OrderByDescending(l => l.FechaInicio)
                .FirstOrDefaultAsync();

            var modulosDelPlan = new List<int>();

            if (licenciaActiva != null)
            {
                modulosDelPlan = await _context.PlanesModulos
                    .AsNoTracking()
                    .Where(pm => pm.IdPlan == licenciaActiva.IdPlan)
                    .Select(pm => pm.IdModulo)
                    .Distinct()
                    .ToListAsync();
            }

            var modulosActividad = await _context.ModulosActividadEconomica
                .AsNoTracking()
                .Where(ma => ma.IdActividad == empresa.ActividadEconomicaId)
                .Select(ma => ma.IdModulo)
                .Distinct()
                .ToListAsync();

            var modulosEmpresa = await _context.ModulosEmpresa
                .AsNoTracking()
                .Where(me => me.IdEmpresa == idEmpresa)
                .ToListAsync();

            foreach (var modulo in modulos)
            {
                var relacionEmpresa = modulosEmpresa
                    .FirstOrDefault(me => me.IdModulo == modulo.IdModulo);

                if (relacionEmpresa != null)
                {
                    modulo.ActivoParaCliente = relacionEmpresa.Activo;
                }
                else
                {
                    modulo.ActivoParaCliente =
                        modulosDelPlan.Contains(modulo.IdModulo) ||
                        modulosActividad.Contains(modulo.IdModulo);
                }
            }

            return modulos;
        }

        private async Task AsignarModulosBaseDelPlanAsync(string idEmpresa, int idPlan)
        {
            var modulosDelPlan = await _context.PlanesModulos
                .AsNoTracking()
                .Where(pm => pm.IdPlan == idPlan)
                .Select(pm => pm.IdModulo)
                .Distinct()
                .ToListAsync();

            foreach (var idModulo in modulosDelPlan)
            {
                var existe = await _context.ModulosEmpresa
                    .AnyAsync(me => me.IdEmpresa == idEmpresa && me.IdModulo == idModulo);

                if (!existe)
                {
                    _context.ModulosEmpresa.Add(new ModulosEmpresa
                    {
                        IdEmpresa = idEmpresa,
                        IdModulo = idModulo,
                        Activo = true,
                        EsAdicional = false,
                        FechaAsignacion = DateTime.Now
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task AsignarModulosPorActividadEconomicaAsync(string idEmpresa, int idActividad)
        {
            var modulosActividad = await _context.ModulosActividadEconomica
                .AsNoTracking()
                .Where(ma => ma.IdActividad == idActividad)
                .Select(ma => ma.IdModulo)
                .Distinct()
                .ToListAsync();

            foreach (var idModulo in modulosActividad)
            {
                var moduloEmpresa = await _context.ModulosEmpresa
                    .FirstOrDefaultAsync(me => me.IdEmpresa == idEmpresa && me.IdModulo == idModulo);

                if (moduloEmpresa == null)
                {
                    _context.ModulosEmpresa.Add(new ModulosEmpresa
                    {
                        IdEmpresa = idEmpresa,
                        IdModulo = idModulo,
                        Activo = true,
                        EsAdicional = true,
                        FechaAsignacion = DateTime.Now
                    });
                }
                else
                {
                    moduloEmpresa.Activo = true;
                    moduloEmpresa.EsAdicional = true;
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task SincronizarModulosPorCambioPlanAsync(string idEmpresa, int nuevoIdPlan)
        {
            var modulosNuevoPlan = await _context.PlanesModulos
                .AsNoTracking()
                .Where(pm => pm.IdPlan == nuevoIdPlan)
                .Select(pm => pm.IdModulo)
                .Distinct()
                .ToListAsync();

            var modulosEmpresaActuales = await _context.ModulosEmpresa
                .Where(me => me.IdEmpresa == idEmpresa)
                .ToListAsync();

            foreach (var idModulo in modulosNuevoPlan)
            {
                var moduloEmpresa = modulosEmpresaActuales
                    .FirstOrDefault(me => me.IdModulo == idModulo);

                if (moduloEmpresa == null)
                {
                    _context.ModulosEmpresa.Add(new ModulosEmpresa
                    {
                        IdEmpresa = idEmpresa,
                        IdModulo = idModulo,
                        Activo = true,
                        EsAdicional = false,
                        FechaAsignacion = DateTime.Now
                    });
                }
                else
                {
                    moduloEmpresa.Activo = true;
                    moduloEmpresa.EsAdicional = false;
                }
            }

            foreach (var moduloActual in modulosEmpresaActuales)
            {
                var perteneceAlNuevoPlan = modulosNuevoPlan.Contains(moduloActual.IdModulo);

                if (!perteneceAlNuevoPlan && !moduloActual.EsAdicional)
                {
                    moduloActual.Activo = false;
                }
            }

            await _context.SaveChangesAsync();
        }
        public async Task<(bool Ok, string Mensaje)> CrearModuloAsync(CrearModuloViewModel model)
        {
            if (model == null)
                return (false, "No se recibió información del módulo.");

            if (string.IsNullOrWhiteSpace(model.NombreModulo))
                return (false, "El nombre del módulo es obligatorio.");

            if (!model.EsGenerico && !model.EsEspecializado)
                return (false, "Debes indicar si el módulo es genérico o especializado.");

            if (string.IsNullOrWhiteSpace(model.Header))
                return (false, "El Header del menú es obligatorio.");

            if (string.IsNullOrWhiteSpace(model.Grupo))
                return (false, "El Grupo del menú es obligatorio.");

            if (string.IsNullOrWhiteSpace(model.Controller))
                return (false, "El Controller del menú es obligatorio.");

            if (string.IsNullOrWhiteSpace(model.Action))
                return (false, "El Action del menú es obligatorio.");

            if (model.OrdenHeader <= 0)
                return (false, "El OrdenHeader debe ser mayor a 0.");

            if (model.OrdenGrupo <= 0)
                return (false, "El OrdenGrupo debe ser mayor a 0.");

            if (model.OrdenOpcion <= 0)
                return (false, "El OrdenOpcion debe ser mayor a 0.");

            if (string.IsNullOrWhiteSpace(model.IconoHeader))
                return (false, "El IconoHeader del menú es obligatorio.");

            if (string.IsNullOrWhiteSpace(model.IconoGrupo))
                return (false, "El IconoGrupo del menú es obligatorio.");

            if (string.IsNullOrWhiteSpace(model.IconoOpcion))
                return (false, "El IconoOpcion del menú es obligatorio.");

            var nombreModulo = model.NombreModulo.Trim();
            var controller = model.Controller.Trim();
            var action = model.Action.Trim();

            var existeModulo = await _context.Modulos
                .AnyAsync(m => m.NombreModulo == nombreModulo);

            if (existeModulo)
                return (false, "Ya existe un módulo con ese nombre.");

            var existeMenuConMismaRuta = await _context.MenuOpciones
                .AnyAsync(m => m.Controller == controller && m.Action == action);

            if (existeMenuConMismaRuta)
                return (false, "Ya existe una opción de menú con el mismo Controller y Action.");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var modulo = new Modulos
                {
                    NombreModulo = nombreModulo,
                    Descripcion = string.IsNullOrWhiteSpace(model.Descripcion)
                        ? "N/A"
                        : model.Descripcion.Trim(),
                    EsGenerico = model.EsGenerico,
                    EsEspecializado = model.EsEspecializado
                };

                _context.Modulos.Add(modulo);
                await _context.SaveChangesAsync();

                var menuOpcion = new MenuOpciones
                {
                    IdModulo = modulo.IdModulo,
                    Header = model.Header.Trim(),
                    Grupo = model.Grupo.Trim(),
                    IconoHeader = model.IconoHeader.Trim(),
                    IconoGrupo = model.IconoGrupo.Trim(),
                    IconoOpcion = model.IconoOpcion.Trim(),
                    Controller = controller,
                    Action = action,
                    OrdenHeader = model.OrdenHeader,
                    OrdenGrupo = model.OrdenGrupo,
                    OrdenOpcion = model.OrdenOpcion
                };

                _context.MenuOpciones.Add(menuOpcion);

                if (model.AgregarAlPlan)
                {
                    if (model.IdPlan == null || model.IdPlan <= 0)
                        return (false, "Seleccionaste agregar al plan, pero no seleccionaste un plan válido.");

                    var existePlan = await _context.Planes
                        .AnyAsync(p => p.IdPlan == model.IdPlan.Value);

                    if (!existePlan)
                        return (false, "El plan seleccionado no existe.");

                    _context.PlanesModulos.Add(new PlanesModulos
                    {
                        IdPlan = model.IdPlan.Value,
                        IdModulo = modulo.IdModulo
                    });
                }

                if (model.AgregarAEmpresaActual)
                {
                    if (string.IsNullOrWhiteSpace(model.IdEmpresa))
                        return (false, "Seleccionaste agregar a la empresa actual, pero no se recibió el NIT.");

                    var idEmpresa = model.IdEmpresa.Trim();

                    var existeEmpresa = await _context.Empresas
                        .AnyAsync(e => e.Id_empresa == idEmpresa);

                    if (!existeEmpresa)
                        return (false, "La empresa seleccionada no existe.");

                    _context.ModulosEmpresa.Add(new ModulosEmpresa
                    {
                        IdEmpresa = idEmpresa,
                        IdModulo = modulo.IdModulo,
                        Activo = true,
                        EsAdicional = true,
                        FechaAsignacion = DateTime.Now
                    });
                }

                if (model.AgregarAActividadEconomica)
                {
                    if (model.IdActividad == null || model.IdActividad <= 0)
                        return (false, "Seleccionaste agregar a una actividad económica, pero no seleccionaste una actividad válida.");

                    var existeActividad = await _context.ActividadesEconomicas
                        .AnyAsync(a => a.IdActividad == model.IdActividad.Value);

                    if (!existeActividad)
                        return (false, "La actividad económica seleccionada no existe.");

                    _context.ModulosActividadEconomica.Add(new ModulosActividadEconomica
                    {
                        IdActividad = model.IdActividad.Value,
                        IdModulo = modulo.IdModulo
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, "Módulo y opción de menú creados correctamente.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"No fue posible crear el módulo. Detalle: {ex.Message}");
            }
        }
    }
}
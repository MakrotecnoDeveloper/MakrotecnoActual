using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using System.Text.Json;

namespace Plataforma.Services
{
    public partial class NominaService : INominaService
    {
        private readonly BaseAdmContext _context;

        public NominaService(BaseAdmContext context)
        {
            _context = context;
        }

        public async Task<GeneracionNovedadesResultado> GenerarNovedadesDesdeCierreCajaAsync(int idPeriodo, string usuario)
        {
            var periodo = await _context.PeriodosNomina
            .FirstOrDefaultAsync(x => x.IdPeriodo == idPeriodo);

            if (periodo == null)
                throw new Exception("El período de nómina no existe.");

            if (!string.Equals(periodo.Estado, "Abierto", StringComparison.OrdinalIgnoreCase))
                throw new Exception("Solo se pueden generar novedades cuando el período está en estado Abierto.");

            var conceptosComisionables = await _context.ConceptosNomina
                .Where(x =>
                    x.EsActivo &&
                    x.Naturaleza == "Ingreso" &&
                    x.ModoCalculo == "Porcentaje" &&
                    x.IdServicio != null)
                .ToListAsync();

            if (!conceptosComisionables.Any())
                return new GeneracionNovedadesResultado();

            var asignaciones = await _context.DetalleConceptosEmpleado
                .Where(x =>
                    x.Estado &&
                    x.FechaInicio <= periodo.FechaFin &&
                    (x.FechaFin == null || x.FechaFin >= periodo.FechaInicio))
                .ToListAsync();

            var servicios = await _context.Servicio.ToListAsync();

            var servicioPorNombre = servicios
                .Where(x => !string.IsNullOrWhiteSpace(x.NombreServicio))
                .GroupBy(x => NormalizarTexto(x.NombreServicio!))
                .ToDictionary(g => g.Key, g => g.First());

            var servicioPorId = servicios.ToDictionary(x => x.IdServicio, x => x);

            var fechaFinExclusiva = periodo.FechaFin.Date.AddDays(1);

            var cierres = await _context.CierreCajas
                .Where(x =>
                    x.Fecha >= periodo.FechaInicio &&
                    x.Fecha < fechaFinExclusiva &&
                    !string.IsNullOrWhiteSpace(x.ConceptosJson))
                .OrderBy(x => x.Fecha)
                .ToListAsync();

            var resultado = new GeneracionNovedadesResultado();

            var novedadesExistentes = await _context.NovedadesNomina
                .Where(x => x.IdPeriodo == idPeriodo && x.Origen == "CierreCaja")
                .ToListAsync();

            foreach (var cierre in cierres)
            {
                var detalles = ParsearConceptosJson(cierre.ConceptosJson!);

                foreach (var item in detalles)
                {
                    var servicioEncontrado = ResolverServicio(item, servicioPorNombre, servicioPorId);
                    if (servicioEncontrado == null)
                        continue;

                    var concepto = conceptosComisionables
                        .FirstOrDefault(x => x.IdServicio == servicioEncontrado.IdServicio);

                    if (concepto == null)
                        continue;

                    var vNeto = item.VNeto ?? item.TotalVNeto ?? item.MontoVNeto ?? 0m;
                    var subTotal = item.SubTotal ?? item.TotalSubTotal ?? item.MontoSubTotal ?? 0m;
                    var utilidad = item.Utilidad ?? (subTotal - vNeto);

                    if (utilidad <= 0)
                        continue;

                    resultado.BaseVentasProcesada += utilidad;
                    resultado.VentasProcesadas++;

                    var asignacionesConcepto = asignaciones
                        .Where(a => a.IdConcepto == concepto.IdConcepto)
                        .ToList();

                    if (!asignacionesConcepto.Any())
                    {
                        resultado.RegistrosSinAsignacion++;
                        continue;
                    }

                    foreach (var asignacion in asignacionesConcepto)
                    {
                        var porcentaje = asignacion.PorcentajePersonalizado
                                        ?? concepto.Porcentaje
                                        ?? 0m;

                        if (porcentaje <= 0)
                            continue;

                        var valorParticipacion = Math.Round(
                            utilidad * (porcentaje / 100m),
                            2,
                            MidpointRounding.AwayFromZero);

                        if (valorParticipacion <= 0)
                            continue;

                        var fechaCierre = cierre.Fecha.Date;

                        // Ahora la novedad queda identificada por cierre diario
                        var documentoOrigen =
                            $"CIERRECAJA|CIERRE:{cierre.IdFlujoCaja}|FECHA:{fechaCierre:yyyy-MM-dd}|SER:{servicioEncontrado.IdServicio}|CON:{concepto.IdConcepto}|EMP:{asignacion.Cedula}";

                        var novedadExistente = novedadesExistentes.FirstOrDefault(x =>
                            x.Cedula == asignacion.Cedula &&
                            x.IdConcepto == concepto.IdConcepto &&
                            x.DocumentoOrigen == documentoOrigen);

                        if (novedadExistente == null)
                        {
                            var novedad = new NovedadNomina
                            {
                                Cedula = asignacion.Cedula,
                                IdConcepto = concepto.IdConcepto,
                                IdPeriodo = idPeriodo,
                                FechaNovedad = fechaCierre, // fecha real del cierre
                                Cantidad = 1,
                                BaseValor = utilidad,
                                PorcentajeAplicado = porcentaje,
                                Valor = valorParticipacion,
                                DocumentoOrigen = documentoOrigen,
                                Origen = "CierreCaja",
                                Estado = "Pendiente",
                                Observacion = $"Generada desde cierre #{cierre.IdFlujoCaja} por {usuario}",
                                FechaCreacion = DateTime.Now
                            };

                            _context.NovedadesNomina.Add(novedad);
                            novedadesExistentes.Add(novedad); // evita duplicados dentro de la misma ejecución
                            resultado.NovedadesCreadas++;
                        }
                        else if (novedadExistente.Estado == "Pendiente")
                        {
                            novedadExistente.FechaNovedad = fechaCierre;
                            novedadExistente.Cantidad = 1;
                            novedadExistente.BaseValor = utilidad;
                            novedadExistente.PorcentajeAplicado = porcentaje;
                            novedadExistente.Valor = valorParticipacion;
                            novedadExistente.Observacion = $"Actualizada desde cierre #{cierre.IdFlujoCaja} por {usuario}";
                            resultado.NovedadesActualizadas++;
                        }

                        resultado.TotalComisionesGeneradas += valorParticipacion;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return resultado;
        }


        private Servicio? ResolverServicio(
    CierreCajaConceptoJsonItem item,
    Dictionary<string, Servicio> servicioPorNombre,
    Dictionary<int, Servicio> servicioPorId)
        {
            // 1. Primero intenta resolver por IdServicio
            if (item.IdServicio.HasValue && item.IdServicio.Value > 0)
            {
                return servicioPorId.TryGetValue(item.IdServicio.Value, out var servicioPorCodigo)
                    ? servicioPorCodigo
                    : null;
            }

            // 2. Si no viene IdServicio, intenta por nombre
            var nombreServicio = item.Servicio ?? item.NombreServicio ?? item.Concepto;

            if (string.IsNullOrWhiteSpace(nombreServicio))
                return null;

            var key = NormalizarTexto(nombreServicio);

            return servicioPorNombre.TryGetValue(key, out var servicioPorTexto)
                ? servicioPorTexto
                : null;
        }


        private static string NormalizarTexto(string texto)
        {
            return texto.Trim().ToUpperInvariant();
        }

        private List<CierreCajaConceptoJsonItem> ParsearConceptosJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new List<CierreCajaConceptoJsonItem>();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            try
            {
                using var doc = JsonDocument.Parse(json);

                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    return JsonSerializer.Deserialize<List<CierreCajaConceptoJsonItem>>(json, options)
                           ?? new List<CierreCajaConceptoJsonItem>();
                }

                if (doc.RootElement.ValueKind == JsonValueKind.Object)
                {
                    var root = doc.RootElement;

                    foreach (var prop in new[] { "servicios", "conceptos", "items", "detalle", "detalleServicios" })
                    {
                        if (root.TryGetProperty(prop, out var arr) && arr.ValueKind == JsonValueKind.Array)
                        {
                            return JsonSerializer.Deserialize<List<CierreCajaConceptoJsonItem>>(arr.GetRawText(), options)
                                   ?? new List<CierreCajaConceptoJsonItem>();
                        }
                    }
                }
            }
            catch
            {
                // Si el JSON viene con otro formato, simplemente no genera novedades.
            }

            return new List<CierreCajaConceptoJsonItem>();
        }



        public async Task<LiquidacionNomina> GenerarLiquidacionAsync(int cedula, int idPeriodo, string usuario)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();

            var contrato = await _context.Contratos
                .FirstOrDefaultAsync(x => x.Cedula == cedula && x.Estado == "Activo");

            if (contrato == null)
                throw new Exception("El empleado no tiene contrato activo.");

            var periodo = await _context.PeriodosNomina
            .FirstOrDefaultAsync(x => x.IdPeriodo == idPeriodo);

            if (periodo == null)
                throw new Exception("El período no existe.");

            if (!string.Equals(periodo.Estado, "Calculado", StringComparison.OrdinalIgnoreCase))
                throw new Exception("Solo se pueden generar liquidaciones cuando el período está en estado Calculado.");

            var yaExiste = await _context.LiquidacionesNomina
                .AnyAsync(x => x.Cedula == cedula && x.IdPeriodo == idPeriodo);

            if (yaExiste)
                throw new Exception("Ya existe una liquidación para este empleado en ese período.");

            var conceptoSalario = await _context.ConceptosNomina
                .FirstOrDefaultAsync(x => x.Codigo == "SALARIO" && x.EsActivo);

            if (conceptoSalario == null)
                throw new Exception("No existe el concepto SALARIO en ConceptosNomina.");

            var asignaciones = await _context.DetalleConceptosEmpleado
                .Include(x => x.ConceptoNomina)
                .Where(x =>
                    x.Cedula == cedula &&
                    x.Estado &&
                    x.FechaInicio <= periodo.FechaFin &&
                    (x.FechaFin == null || x.FechaFin >= periodo.FechaInicio))
                .ToListAsync();

            var novedades = await _context.NovedadesNomina
                .Include(x => x.ConceptoNomina)
                .Where(x =>
                    x.Cedula == cedula &&
                    x.IdPeriodo == idPeriodo &&
                    x.Estado == "Pendiente")
                .ToListAsync();

            var detalles = new List<DetalleLiquidacion>();

            detalles.Add(new DetalleLiquidacion
            {
                IdConcepto = conceptoSalario.IdConcepto,
                NombreConcepto = conceptoSalario.Nombre,
                Naturaleza = conceptoSalario.Naturaleza,
                BaseAplicada = contrato.SalarioBase,
                Cantidad = 1,
                ValorUnitario = contrato.SalarioBase,
                Monto = contrato.SalarioBase,
                OrigenDetalle = "Contrato"
            });

            foreach (var asignacion in asignaciones)
            {
                var concepto = asignacion.ConceptoNomina;

                if (concepto.Codigo == "SALARIO")
                    continue;

                if (concepto.ModoCalculo != "Fijo")
                    continue;

                var valor = asignacion.ValorFijoPersonalizado
                            ?? concepto.ValorFijo
                            ?? 0m;

                if (valor <= 0)
                    continue;

                detalles.Add(new DetalleLiquidacion
                {
                    IdConcepto = concepto.IdConcepto,
                    NombreConcepto = concepto.Nombre,
                    Naturaleza = concepto.Naturaleza,
                    BaseAplicada = valor,
                    Cantidad = 1,
                    ValorUnitario = valor,
                    Monto = valor,
                    OrigenDetalle = "Asignacion"
                });
            }

            foreach (var novedad in novedades)
            {
                detalles.Add(new DetalleLiquidacion
                {
                    IdConcepto = novedad.IdConcepto,
                    NombreConcepto = novedad.ConceptoNomina.Nombre,
                    Naturaleza = novedad.ConceptoNomina.Naturaleza,
                    BaseAplicada = novedad.BaseValor,
                    Cantidad = novedad.Cantidad,
                    PorcentajeAplicado = novedad.PorcentajeAplicado,
                    ValorUnitario = novedad.Valor,
                    Monto = novedad.Valor,
                    OrigenDetalle = novedad.Origen,
                    Observacion = novedad.Observacion
                });

                novedad.Estado = "Aplicada";
            }

            var totalDevengado = detalles
                .Where(x => x.Naturaleza == "Ingreso")
                .Sum(x => x.Monto);

            var totalDeducciones = detalles
                .Where(x => x.Naturaleza == "Deduccion")
                .Sum(x => x.Monto);

            var liquidacion = new LiquidacionNomina
            {
                Cedula = cedula,
                IdContrato = contrato.IdContrato,
                IdPeriodo = idPeriodo,
                FechaLiquidacion = DateTime.Today,
                TotalDevengado = totalDevengado,
                TotalDeducciones = totalDeducciones,
                NetoPagar = totalDevengado - totalDeducciones,
                Estado = "Confirmada",
                UsuarioLiquida = usuario,
                FechaCreacion = DateTime.Now,
                DetalleLiquidacion = detalles
            };

            _context.LiquidacionesNomina.Add(liquidacion);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return liquidacion;
        }
        private class ResumenVentaNominaDto
        {
            public int Cedula { get; set; }
            public int IdServicio { get; set; }
            public decimal Cantidad { get; set; }
            public decimal BaseVentas { get; set; }
            public int CantidadLineas { get; set; }
        }
        public async Task<LiquidacionNomina?> ObtenerLiquidacionDetalleAsync(int idLiquidacion)
        {
            return await _context.LiquidacionesNomina
                .Include(x => x.PeriodoNomina)
                .Include(x => x.Contrato)
                .Include(x => x.DetalleLiquidacion)
                .FirstOrDefaultAsync(x => x.IdLiquidacion == idLiquidacion);
        }
        public async Task<List<Empleados>> ObtenerEmpleadosAsync()
        {
            return await _context.Empleado
                .OrderBy(x => x.Nombre)
                .ThenBy(x => x.Apellido)
                .ToListAsync();
        }

        public async Task<List<Servicio>> ObtenerServiciosAsync()
        {
            return await _context.Servicio
                .OrderBy(x => x.NombreServicio)
                .ToListAsync();
        }

        public async Task<List<Contrato>> ObtenerContratosAsync()
        {
            return await _context.Contratos
                .Include(x => x.Empleado)
                .OrderByDescending(x => x.IdContrato)
                .ToListAsync();
        }

        public async Task CrearContratoAsync(ContratoFormVm vm)
        {
            var empleadoExiste = await _context.Empleado.AnyAsync(x => x.Cedula == vm.Cedula);
            if (!empleadoExiste)
                throw new Exception("El empleado no existe.");

            if (vm.FechaFin.HasValue && vm.FechaFin.Value.Date < vm.FechaInicio.Date)
                throw new Exception("La fecha fin no puede ser menor que la fecha inicio.");

            if (vm.SalarioBase < 0)
                throw new Exception("El salario base no puede ser negativo.");

            if (vm.Estado == "Activo")
            {
                var yaTieneActivo = await _context.Contratos
                    .AnyAsync(x => x.Cedula == vm.Cedula && x.Estado == "Activo");

                if (yaTieneActivo)
                    throw new Exception("El empleado ya tiene un contrato activo.");
            }

            var cargo = await _context.TipoCargo
                .Include(x => x.Area)
                .FirstOrDefaultAsync(x => x.Id_tipo == vm.IdTipoCargo);

            if (cargo == null)
                throw new Exception("Debe seleccionar un cargo válido.");

            var entity = new Contrato
            {
                Cedula = vm.Cedula,
                TipoContrato = vm.TipoContrato,
                TipoSalario = vm.TipoSalario,
                IdTipoCargo = vm.IdTipoCargo,
                Cargo = cargo.NombreCargo,
                Area = cargo.Area?.NombreArea,
                SalarioBase = vm.SalarioBase,
                AuxilioTransporteAplica = vm.AuxilioTransporteAplica,
                FechaInicio = vm.FechaInicio,
                FechaFin = vm.FechaFin,
                Estado = vm.Estado,
                Observaciones = vm.Observaciones,
                FechaCreacion = DateTime.Now
            };

            _context.Contratos.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<PeriodoNomina>> ObtenerPeriodosAsync()
        {
            return await _context.PeriodosNomina
                .OrderByDescending(x => x.FechaInicio)
                .ToListAsync();
        }

        public async Task CrearPeriodoAsync(PeriodoNominaFormVm vm)
        {
            if (vm.FechaFin.Date < vm.FechaInicio.Date)
                throw new Exception("La fecha fin no puede ser menor que la fecha inicio.");

            var existe = await _context.PeriodosNomina
                .AnyAsync(x => x.FechaInicio == vm.FechaInicio && x.FechaFin == vm.FechaFin);

            if (existe)
                throw new Exception("Ya existe un período con ese rango de fechas.");

            var entity = new PeriodoNomina
            {
                Descripcion = vm.Descripcion,
                TipoPeriodo = vm.TipoPeriodo,
                FechaInicio = vm.FechaInicio,
                FechaFin = vm.FechaFin,
                FechaPago = vm.FechaPago,
                Estado = vm.Estado,
                FechaCreacion = DateTime.Now
            };

            _context.PeriodosNomina.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ConceptoNomina>> ObtenerConceptosAsync()
        {
            return await _context.ConceptosNomina
                .Include(x => x.Servicio)
                .OrderBy(x => x.OrdenVisualizacion)
                .ThenBy(x => x.Nombre)
                .ToListAsync();
        }

        public async Task CrearConceptoAsync(ConceptoNominaFormVm vm)
        {
            var codigo = vm.Codigo.Trim().ToUpperInvariant();

            var existe = await _context.ConceptosNomina.AnyAsync(x => x.Codigo == codigo);
            if (existe)
                throw new Exception("Ya existe un concepto con ese código.");

            var entity = new ConceptoNomina
            {
                Codigo = codigo,
                Nombre = vm.Nombre.Trim(),
                Naturaleza = vm.Naturaleza,
                ModoCalculo = vm.ModoCalculo,
                BaseCalculo = vm.BaseCalculo,
                IdServicio = vm.IdServicio,
                Porcentaje = vm.Porcentaje,
                ValorFijo = vm.ValorFijo,
                AplicaPrestaciones = vm.AplicaPrestaciones,
                AplicaSeguridadSocial = vm.AplicaSeguridadSocial,
                AplicaParafiscales = vm.AplicaParafiscales,
                OrdenVisualizacion = vm.OrdenVisualizacion,
                EsActivo = vm.EsActivo,
                FechaCreacion = DateTime.Now
            };

            _context.ConceptosNomina.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<DetalleConceptoEmpleado>> ObtenerAsignacionesAsync(int? cedula = null)
        {
            var query = _context.DetalleConceptosEmpleado
                .Include(x => x.Empleado)
                .Include(x => x.ConceptoNomina)
                .AsQueryable();

            if (cedula.HasValue)
                query = query.Where(x => x.Cedula == cedula.Value);

            return await query
                .OrderByDescending(x => x.IdDetalle)
                .ToListAsync();
        }

        public async Task AsignarConceptoAsync(AsignarConceptoEmpleadoVm vm)
        {
            var empleadoExiste = await _context.Empleado.AnyAsync(x => x.Cedula == vm.Cedula);
            if (!empleadoExiste)
                throw new Exception("El empleado no existe.");

            var conceptoExiste = await _context.ConceptosNomina.AnyAsync(x => x.IdConcepto == vm.IdConcepto);
            if (!conceptoExiste)
                throw new Exception("El concepto no existe.");

            if (vm.FechaFin.HasValue && vm.FechaFin.Value.Date < vm.FechaInicio.Date)
                throw new Exception("La fecha fin no puede ser menor que la fecha inicio.");

            var duplicadoActivo = await _context.DetalleConceptosEmpleado.AnyAsync(x =>
                x.Cedula == vm.Cedula &&
                x.IdConcepto == vm.IdConcepto &&
                x.Estado);

            if (duplicadoActivo)
                throw new Exception("Ese empleado ya tiene ese concepto activo.");

            var entity = new DetalleConceptoEmpleado
            {
                Cedula = vm.Cedula,
                IdConcepto = vm.IdConcepto,
                PorcentajePersonalizado = vm.PorcentajePersonalizado,
                ValorFijoPersonalizado = vm.ValorFijoPersonalizado,
                FechaInicio = vm.FechaInicio,
                FechaFin = vm.FechaFin,
                Estado = vm.Estado,
                Observacion = vm.Observacion,
                FechaCreacion = DateTime.Now
            };

            _context.DetalleConceptosEmpleado.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<LiquidacionNomina>> ObtenerLiquidacionesAsync(int? idPeriodo = null, int? cedula = null)
        {
            var query = _context.LiquidacionesNomina
                .Include(x => x.PeriodoNomina)
                .AsQueryable();

            if (idPeriodo.HasValue)
                query = query.Where(x => x.IdPeriodo == idPeriodo.Value);

            if (cedula.HasValue)
                query = query.Where(x => x.Cedula == cedula.Value);

            return await query
                .OrderByDescending(x => x.IdLiquidacion)
                .ToListAsync();
        }

        public async Task<List<LiquidacionNomina>> ObtenerUltimasLiquidacionesAsync(int take = 15)
        {
            return await _context.LiquidacionesNomina
                .Include(x => x.PeriodoNomina)
                .OrderByDescending(x => x.IdLiquidacion)
                .Take(take)
                .ToListAsync();
        }


        /*DTO Inicio*/
        private class AcumuladoServicioCierre
        {
            public int IdServicio { get; set; }
            public decimal VNeto { get; set; }
            public decimal SubTotal { get; set; }
            public decimal Utilidad { get; set; }
            public int CantidadCierres { get; set; }
        }

        private class CierreCajaConceptoJsonItem
        {
            public int? IdServicio { get; set; }
            public string? Servicio { get; set; }
            public string? NombreServicio { get; set; }
            public string? Concepto { get; set; }

            public decimal? VNeto { get; set; }
            public decimal? TotalVNeto { get; set; }
            public decimal? MontoVNeto { get; set; }

            public decimal? SubTotal { get; set; }
            public decimal? TotalSubTotal { get; set; }
            public decimal? MontoSubTotal { get; set; }

            public decimal? Utilidad { get; set; }
        }
        /*DTO Final*/

        /*Inicio de Areas, Contrato y Cargo*/
        public async Task<List<Area>> ObtenerAreasAsync()
        {
            return await _context.Areas
                .OrderBy(x => x.NombreArea)
                .ToListAsync();
        }

        public async Task CrearAreaAsync(AreaFormVm vm)
        {
            var existe = await _context.Areas
                .AnyAsync(x => x.NombreArea == vm.NombreArea);

            if (existe)
                throw new Exception("Ya existe un área con ese nombre.");

            var entity = new Area
            {
                NombreArea = vm.NombreArea.Trim(),
                DescripcionArea = vm.DescripcionArea,
                Estado = vm.Estado
            };

            _context.Areas.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TipoCargo>> ObtenerCargosAsync()
        {
            return await _context.TipoCargo
                .Include(x => x.Area)
                .OrderBy(x => x.NombreCargo)
                .ToListAsync();
        }

        public async Task AsignarAreaCargoAsync(AsignarAreaCargoVm vm)
        {
            var cargo = await _context.TipoCargo.FirstOrDefaultAsync(x => x.Id_tipo == vm.IdTipoCargo);
            if (cargo == null)
                throw new Exception("El cargo no existe.");

            var area = await _context.Areas.FirstOrDefaultAsync(x => x.IdArea == vm.IdArea);
            if (area == null)
                throw new Exception("El área no existe.");

            cargo.IdArea = vm.IdArea;

            await _context.SaveChangesAsync();
        }
        public async Task CambiarEstadoPeriodoAsync(int idPeriodo, string nuevoEstado)
        {
            var periodo = await _context.PeriodosNomina
                .FirstOrDefaultAsync(x => x.IdPeriodo == idPeriodo);

            if (periodo == null)
                throw new Exception("El período no existe.");

            var estadoActual = (periodo.Estado ?? "").Trim();

            var estadosPermitidos = new[] { "Abierto", "Calculado", "Cerrado", "Pagado" };
            if (!estadosPermitidos.Contains(nuevoEstado))
                throw new Exception("El estado solicitado no es válido.");

            // Flujo obligatorio
            var siguienteEstadoValido = estadoActual switch
            {
                "Abierto" => "Calculado",
                "Calculado" => "Cerrado",
                "Cerrado" => "Pagado",
                "Pagado" => "",
                _ => throw new Exception("El estado actual del período no es válido.")
            };

            if (string.IsNullOrWhiteSpace(siguienteEstadoValido))
                throw new Exception("El período ya está en estado final.");

            if (!string.Equals(siguienteEstadoValido, nuevoEstado, StringComparison.OrdinalIgnoreCase))
                throw new Exception($"Solo se permite pasar de {estadoActual} a {siguienteEstadoValido}.");

            var fechaInicio = periodo.FechaInicio.Date;
            var fechaFinExclusiva = periodo.FechaFin.Date.AddDays(1);

            if (nuevoEstado == "Calculado")
            {
                var existeCierre = await _context.CierreCajas
                    .AnyAsync(x =>
                        x.Fecha >= fechaInicio &&
                        x.Fecha < fechaFinExclusiva &&
                        !string.IsNullOrWhiteSpace(x.ConceptosJson));

                if (!existeCierre)
                    throw new Exception("No puedes pasar a Calculado porque el período no tiene cierres de caja.");

                var existeNovedad = await _context.NovedadesNomina
                    .AnyAsync(x => x.IdPeriodo == idPeriodo);

                if (!existeNovedad)
                    throw new Exception("No puedes pasar a Calculado porque aún no se han generado novedades.");
            }

            if (nuevoEstado == "Cerrado")
            {
                var existeLiquidacion = await _context.LiquidacionesNomina
                    .AnyAsync(x => x.IdPeriodo == idPeriodo);

                if (!existeLiquidacion)
                    throw new Exception("No puedes pasar a Cerrado porque no hay liquidaciones generadas.");

                var novedadesPendientes = await _context.NovedadesNomina
                    .AnyAsync(x => x.IdPeriodo == idPeriodo && x.Estado == "Pendiente");

                if (novedadesPendientes)
                    throw new Exception("No puedes pasar a Cerrado porque todavía hay novedades pendientes por aplicar.");
            }

            if (nuevoEstado == "Pagado")
            {
                var liquidaciones = await _context.LiquidacionesNomina
                    .Where(x => x.IdPeriodo == idPeriodo)
                    .ToListAsync();

                if (!liquidaciones.Any())
                    throw new Exception("No puedes pasar a Pagado porque no hay liquidaciones generadas.");

                foreach (var liquidacion in liquidaciones)
                {
                    if (!liquidacion.FechaPago.HasValue)
                        liquidacion.FechaPago = periodo.FechaPago ?? DateTime.Today;

                    if (string.IsNullOrWhiteSpace(liquidacion.Estado) || liquidacion.Estado == "Confirmada")
                        liquidacion.Estado = "Pagada";
                }
            }

            periodo.Estado = nuevoEstado;

            await _context.SaveChangesAsync();
        }
        /*Fin de Areas, Contrato y Cargo*/
    }
}
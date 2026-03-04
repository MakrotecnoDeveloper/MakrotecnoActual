using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using System.Text.Json;

namespace Plataforma.Servicios.Implementacion
{
    public class NominaService : INominaService
    {
        private readonly BaseAdmContext _dbContext;
        public NominaService(BaseAdmContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task CrearContratoAsync(int Cedula, string TipoContrato, int SalarioBase, DateTime FechaInicio, DateTime FechaFin, bool Activo)
        {
            var crearContrato = new Contratos { 
            
                Cedula = Cedula,
                TipoContrato = TipoContrato,
                SalarioBase = SalarioBase,
                FechaFin = FechaFin,
                FechaInicio = FechaInicio,
                Activo = Activo

            };
            _dbContext.Contratos.Add(crearContrato);
            await _dbContext.SaveChangesAsync();
        }
        public List<ConceptoServicioVM> GetConceptos(string? json)
        {
            if (string.IsNullOrEmpty(json)) return new();
            return JsonSerializer.Deserialize<List<ConceptoServicioVM>>(json) ?? new();
        }
        public List<string> ObtenerServicios()
        {
            // 1. Servicios base de catálogo
            var serviciosBase = _dbContext.Servicio
                .Select(s => s.NombreServicio)
                .ToList();

            // 2. Servicios dinámicos de ConceptosJson
            var serviciosCaja = _dbContext.CierreCajas
            .Where(f => f.TipoMovimiento == "Cierre Diario")
            .AsEnumerable() // ⬅️ aquí forzamos a traer los datos a memoria
            .SelectMany(f => GetConceptos(f.ConceptosJson))
            .Select(c => c.NombreServicio)
            .Distinct()
            .ToList();

            // 3. Unión sin duplicados
            return serviciosBase
                .Union(serviciosCaja)
                .OrderBy(s => s)
                .ToList();
        }
        public async Task CrearConceptoNominaAsync(string Nombre, string Tipo, string ServicioAsociado, decimal Porcentaje, decimal ValorFijo)
        {
            var crearConceptoNomina = new ConceptoNomina
            {
                Nombre = Nombre,
                Tipo = Tipo,
                ServicioAsociado = ServicioAsociado,
                Porcentaje = Porcentaje,
                ValorFijo = ValorFijo
            };
            _dbContext.ConceptosNomina.Add(crearConceptoNomina);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AsociarConceptoAsync(int Cedula, int IdConcepto)
        {
            var asociarConceptosEmpleado = new DetalleConceptosEmpleado
            {
                Cedula = Cedula,
                IdConcepto = IdConcepto
            };
            _dbContext.DetallesConceptosEmpleado.Add(asociarConceptosEmpleado);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<LiquidacionNomina> LiquidarPeriodoAsync(int cedulaEmpleado, DateTime fechaInicio, DateTime fechaFin)
        {
            var empleado = await _dbContext.Empleado
                .Include(e => e.Contratos)
                .FirstOrDefaultAsync(e => e.Cedula == cedulaEmpleado);

            if (empleado == null)
                throw new Exception("Empleado no encontrado.");

            var contrato = empleado.Contratos.FirstOrDefault(c =>
                (!c.FechaFin.HasValue || c.FechaFin >= fechaFin));

            var conceptos = await _dbContext.DetallesConceptosEmpleado
                .Include(dc => dc.Concepto)
                .Where(dc => dc.Cedula == cedulaEmpleado)
                .ToListAsync();

            decimal salarioBase = contrato?.SalarioBase ?? 0;
            decimal totalIngresos = salarioBase;
            decimal totalDeducciones = 0;

            // Aplicar conceptos
            foreach (var detalle in conceptos) // conceptos asignados al empleado
            {
                if (detalle.Concepto.Tipo == "Ingreso")
                {
                    if (!string.IsNullOrEmpty(detalle.Concepto.ServicioAsociado))
                    {
                        // Paso 1: traer los flujoCajas que cumplen el filtro desde SQL
                        var flujoCajas = await _dbContext.CierreCajas
                            .Where(f => f.Fecha >= fechaInicio && f.Fecha <= fechaFin
                                        && f.TipoMovimiento == "Cierre Diario")
                            .ToListAsync(); // <-- ejecuta la consulta en la BD aquí

                        // Paso 2: ya en memoria deserializamos y filtramos por el servicio
                        var totalVentas = flujoCajas
                            .SelectMany(f => f.GetConceptos()) // se ejecuta en memoria
                            .Where(c => c.NombreServicio == detalle.Concepto.ServicioAsociado)
                            .Sum(c => c.TotalSubTotal);

                        var valor = (detalle.Concepto.Porcentaje ?? 0) * totalVentas / 100;
                        totalIngresos += valor;
                    }
                }
                else if (detalle.Concepto.Tipo == "Deduccion")
                {
                    totalDeducciones += detalle.Concepto.ValorFijo ?? 0;
                }
            }

            var liquidacion = new LiquidacionNomina
            {
                Cedula = cedulaEmpleado,
                FechaLiquidacion = fechaInicio,
                PeriodoFin = fechaFin,
                TotalIngresos = totalIngresos,
                TotalDeducciones = totalDeducciones,
                NetoPagar = totalIngresos - totalDeducciones
            };

            _dbContext.LiquidacionesNomina.Add(liquidacion);
            await _dbContext.SaveChangesAsync();

            return liquidacion;
        }

    }
}
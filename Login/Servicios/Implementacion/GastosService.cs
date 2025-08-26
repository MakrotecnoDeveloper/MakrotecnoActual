using DocumentFormat.OpenXml.Bibliography;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using System.Text.Json;
namespace Plataforma.Servicios.Implementacion
{
    public class GastosService : IGastosService
    {
        private readonly BaseAdmContext _dbContext;
        public GastosService(BaseAdmContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<GastosMensuales>> ListarPorMesAsync(int year, int month)
        {
            var inicio = new DateTime(year, month, 1);
            var fin = inicio.AddMonths(1);
            return await _dbContext.GastosMensuales
                .Where(g => g.Fecha >= inicio && g.Fecha < fin)
                .OrderByDescending(g => g.Fecha)
                .ThenBy(g => g.Nombre)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<GastosMensuales> CrearAsync(GastosMensuales nuevo)
        {
            _dbContext.GastosMensuales.Add(nuevo);
            await _dbContext.SaveChangesAsync();
            return nuevo;
        }

        public async Task<GastosMensuales?> EditarAsync(int id, GastosMensuales cambios)
        {
            var dbGasto = await _dbContext.GastosMensuales.FindAsync(id);
            if (dbGasto == null) return null;

            dbGasto.Nombre = cambios.Nombre;
            dbGasto.Monto = cambios.Monto;
            dbGasto.Fecha = cambios.Fecha;
            dbGasto.Categoria = cambios.Categoria;
            dbGasto.Frecuencia = cambios.Frecuencia;
            dbGasto.Notas = cambios.Notas;

            await _dbContext.SaveChangesAsync();
            return dbGasto;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var dbGasto = await _dbContext.GastosMensuales.FindAsync(id);
            if (dbGasto == null) return false;
            _dbContext.GastosMensuales.Remove(dbGasto);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public Task<GastosMensuales?> ObtenerAsync(int id) => _dbContext.GastosMensuales.AsNoTracking().FirstOrDefaultAsync(g => g.IdGasto == id);
    }
}

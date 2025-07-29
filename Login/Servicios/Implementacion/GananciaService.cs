using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Servicios.Implementacion
{
    public class GananciaService : IGananciaService
    {
        private readonly BaseAdmContext _dbContext;
        public GananciaService(BaseAdmContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Ganancias>> ListarGananciasAsync()
        {
            return await _dbContext.Ganancias.OrderByDescending(g => g.Fecha).ToListAsync();
        }

        public async Task RegistrarGananciaAsync(Ganancias ganancia)
        {
            _dbContext.Ganancias.Add(ganancia);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<GananciaViewModel> ObtenerGananciaDelDiaAsync(int cedulaUsuario)
        {
            return await _dbContext.VistaGananciaDiarias
        .Where(v => v.Cedula == cedulaUsuario)
        .Select(v => new GananciaViewModel
        {
            Cedula = v.Cedula,
            TotalIngresos = v.TotalIngresos,
            TotalCostos = v.TotalCostos,
            TotalUtilidad = v.TotalUtilidad
        })
        .FirstOrDefaultAsync();
        }

        public async Task GuardarGananciaDelDiaAsync(GananciaViewModel model)
        {
            if (model != null)
            {
                _dbContext.Ganancias.Add(new Ganancias
                {
                    Cedula = model.Cedula,
                    Fecha = DateTime.Now,
                    Utilidad = model.TotalUtilidad,
                    Ingresos = model.TotalIngresos,
                    Costos = model.TotalCostos
                });

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Servicios.Implementacion
{
    public class MenuService : IMenuService
    {
        private readonly BaseAdmContext _dbContext;
        private readonly ILogger<MenuService> _logger;
        public MenuService(BaseAdmContext dbContext, ILogger<MenuService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public async Task<List<ModuloMenuDto>> ObtenerMenuEstructuradoPorEmpresaAsync(string idEmpresa, int idCargo)
        {
            if (string.IsNullOrEmpty(idEmpresa) || idCargo <= 0)
                return new List<ModuloMenuDto>();

            var modulosActivosIds = await _dbContext.CargoModuloPermiso
                .Where(p => p.EmpresaId == idEmpresa
                         && p.CargoId == idCargo
                         && p.Activo)
                .Select(p => p.ModuloId)
                .Distinct()
                .ToListAsync();

            if (!modulosActivosIds.Any())
                return new List<ModuloMenuDto>();

            var opciones = await _dbContext.MenuOpciones
                .Include(o => o.Modulo)
                .Where(o => modulosActivosIds.Contains(o.IdModulo))
                .ToListAsync();

            if (!opciones.Any())
                return new List<ModuloMenuDto>();

            var resultado = opciones
                .GroupBy(o => (o.Header ?? "").Trim().ToUpper())
                .Select(headerGroup =>
                {
                    var primerHeader = headerGroup
                        .OrderBy(x => x.OrdenHeader)
                        .ThenBy(x => x.Header)
                        .First();

                    return new ModuloMenuDto
                    {
                        Header = primerHeader.Header?.Trim(),
                        IconoHeader = primerHeader.IconoHeader,
                        OrdenHeader = headerGroup.Min(x => x.OrdenHeader),

                        Grupos = headerGroup
                            .GroupBy(o => (o.Grupo ?? "").Trim().ToUpper())
                            .Select(grupo =>
                            {
                                var primerGrupo = grupo
                                    .OrderBy(x => x.OrdenGrupo)
                                    .ThenBy(x => x.Grupo)
                                    .First();

                                return new GrupoMenuDto
                                {
                                    NombreGrupo = primerGrupo.Grupo?.Trim(),
                                    IconoGrupo = primerGrupo.IconoGrupo,
                                    OrdenGrupo = grupo.Min(x => x.OrdenGrupo),

                                    Opciones = grupo
                                        .OrderBy(o => o.OrdenOpcion)
                                        .Select(o => new OpcionMenuDto
                                        {
                                            Titulo = o.Modulo.NombreModulo,
                                            Controller = o.Controller,
                                            Action = o.Action,
                                            Icono = o.IconoOpcion,
                                            OrdenOpcion = o.OrdenOpcion
                                        })
                                        .ToList()
                                };
                            })
                            .OrderBy(g => g.OrdenGrupo)
                            .ToList()
                    };
                })
                .OrderBy(h => h.OrdenHeader)
                .ToList();

            return resultado;
        }
    }
}
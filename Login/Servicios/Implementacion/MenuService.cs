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

            // 1. Obtener los módulos activos del cargo y empresa
            var modulosActivosIds = await _dbContext.CargoModuloPermiso
                .Where(p => p.EmpresaId == idEmpresa
                         && p.CargoId == idCargo
                         && p.Activo)
                .Select(p => p.ModuloId)
                .Distinct()
                .ToListAsync();

            if (!modulosActivosIds.Any())
                return new List<ModuloMenuDto>();

            // 2. Traer las opciones de menú asociadas a esos módulos
            var opciones = await _dbContext.MenuOpciones
                .Include(o => o.Modulo)
                .Where(o => modulosActivosIds.Contains(o.IdModulo))
                .ToListAsync();

            if (!opciones.Any())
                return new List<ModuloMenuDto>();

            // 3. Construir estructura Header -> Grupo -> Opciones
            var resultado = opciones
                .GroupBy(o => new { o.Header, o.IconoHeader, o.OrdenHeader })
                .OrderBy(g => g.Key.OrdenHeader)
                .Select(headerGroup => new ModuloMenuDto
                {
                    Header = headerGroup.Key.Header,
                    IconoHeader = headerGroup.Key.IconoHeader,
                    OrdenHeader = headerGroup.Key.OrdenHeader,

                    Grupos = headerGroup
                        .GroupBy(o => new { o.Grupo, o.IconoGrupo, o.OrdenGrupo })
                        .OrderBy(g2 => g2.Key.OrdenGrupo)
                        .Select(grupo => new GrupoMenuDto
                        {
                            NombreGrupo = grupo.Key.Grupo,
                            IconoGrupo = grupo.Key.IconoGrupo,
                            OrdenGrupo = grupo.Key.OrdenGrupo,

                            Opciones = grupo
                                .OrderBy(o => o.OrdenOpcion)
                                .Select(o => new OpcionMenuDto
                                {
                                    Titulo = o.Modulo.NombreModulo,
                                    Controller = o.Controller,
                                    Action = o.Action,
                                    Icono = o.IconoOpcion,
                                    OrdenOpcion = o.OrdenOpcion
                                }).ToList()
                        }).ToList()
                }).ToList();

            return resultado;
        }
    }
}
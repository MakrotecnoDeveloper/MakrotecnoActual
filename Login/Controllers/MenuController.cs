using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Controllers
{
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly BaseAdmContext _dbContext;
        public MenuController(IMenuService menuService, BaseAdmContext dbContext)
        {
            _menuService = menuService;
            _dbContext = dbContext;
        }
        // GET: /PermisosMenu/Index?cargoId=1
        [Authorize]
        [HttpGet]
        public IActionResult Index(int? cargoId)
        {
            // 1. EmpresaId desde el claim
            var empresaIdClaim = User.FindFirst("EmpresaId")?.Value;
            if (empresaIdClaim == null)
                return Unauthorized();

            // 2. Plan y actividad económica
            var licencia = _dbContext.LicenciasEmpresa
                .FirstOrDefault(l => l.IdEmpresa == empresaIdClaim && l.Estado == "Activo");

            if (licencia == null)
                return NotFound("La empresa no tiene licencia activa.");

            int idPlan = licencia.IdPlan;

            var actividadEconomica = _dbContext.Empresas
                .Where(e => e.Id_empresa == empresaIdClaim)
                .Select(e => e.ActividadEconomicaId)
                .FirstOrDefault();

            // 3. Módulos genéricos por plan (PlanesModulos)
            var modulosGenericosIds = _dbContext.PlanesModulos
                .Where(pm => pm.IdPlan == idPlan)
                .Select(pm => pm.IdModulo)
                .ToList();

            // 4. Módulos por actividad económica
            var modulosActividadIds = _dbContext.ModulosActividadEconomica
                .Where(ma => ma.IdActividad == actividadEconomica)
                .Select(ma => ma.IdModulo)
                .ToList();

            // 6. Cargos de la empresa (para el combo)
            var cargosEmpresa = _dbContext.TipoCargo
                .Where(c => c.Id_empresa == empresaIdClaim)
                .Select(c => new SelectListItem
                {
                    Value = c.Id_tipo.ToString(),
                    Text = c.NombreCargo
                })
                .ToList();

            var vm = new PermisosMenuViewModel
            {
                EmpresaId = empresaIdClaim,
                CargoIdSeleccionado = cargoId,
                Cargos = cargosEmpresa
            };

            // Si todavía no hay cargo seleccionado, solo llenamos combos
            if (cargoId == null && cargosEmpresa.Any())
            {
                vm.CargoIdSeleccionado = int.Parse(cargosEmpresa.First().Value);
                cargoId = vm.CargoIdSeleccionado;
            }

            if (cargoId != null)
            {
                // Permisos existentes para ese cargo
                var permisosCargo = _dbContext.CargoModuloPermiso
                    .Where(p => p.EmpresaId == empresaIdClaim && p.CargoId == cargoId)
                    .ToList();

                // 5. Traer nombres de módulos y mapear

                // Genéricos
                var permisosCargoList = permisosCargo.ToList();

                var modulosFiltrados = _dbContext.Modulos
                .Where(m =>
                    modulosGenericosIds.Contains(m.IdModulo) &&
                    m.EsGenerico == true &&
                    m.EsEspecializado == false
                )
                .ToList();

                vm.ModulosGenericos = modulosFiltrados
                .Select(m => new ModuloPermisoItemVM
                {
                    ModuloId = m.IdModulo,
                    NombreModulo = m.NombreModulo,
                    EsGenerico = true,
                    BloqueModulo = string.IsNullOrWhiteSpace(m.Descripcion) ? "General" : m.Descripcion,
                    Activo = permisosCargoList.Any(p => p.ModuloId == m.IdModulo && p.Activo)
                })
                .OrderBy(m => m.BloqueModulo)
                .ThenBy(m => m.NombreModulo)
                .ToList();

                // Por actividad

                // Cargar los módulos por actividad ANTES del Select
                var modulosActividad = _dbContext.Modulos
                .Where(m =>
                    modulosActividadIds.Contains(m.IdModulo) &&
                    m.EsEspecializado == true &&
                    m.EsGenerico == false
                )
                .ToList();

                // Construir el ViewModel en memoria (aquí sí puedes usar Any)
                vm.ModulosActividad = modulosActividad
                .Select(m => new ModuloPermisoItemVM
                {
                    ModuloId = m.IdModulo,
                    NombreModulo = m.NombreModulo,
                    EsGenerico = false,
                    BloqueModulo = string.IsNullOrWhiteSpace(m.Descripcion) ? "General" : m.Descripcion,
                    Activo = permisosCargoList.Any(p => p.ModuloId == m.IdModulo && p.Activo)
                })
                .OrderBy(m => m.BloqueModulo)
                .ThenBy(m => m.NombreModulo)
                .ToList();

            }

            return View(vm);
        }

        // POST: /PermisosMenu/Guardar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Guardar(PermisosMenuViewModel model)
        {
            var empresaIdClaim = User.FindFirst("EmpresaId")?.Value;
            if (empresaIdClaim == null)
                return Unauthorized();

            if (model.CargoIdSeleccionado == null)
                return BadRequest("Debe seleccionar un cargo.");

            int cargoId = model.CargoIdSeleccionado.Value;

            var modulosVM = model.ModulosGenericos
                .Concat(model.ModulosActividad)
                .ToList();

            // Permisos actuales en BD
            var permisosActuales = _dbContext.CargoModuloPermiso
                .Where(p => p.EmpresaId == empresaIdClaim && p.CargoId == cargoId)
                .ToList();

            // Recorremos cada módulo del formulario
            foreach (var modulo in modulosVM)
            {
                var permiso = permisosActuales
                    .FirstOrDefault(p => p.ModuloId == modulo.ModuloId);

                if (permiso == null)
                {
                    // Si no existe y viene marcado, lo creamos
                    if (modulo.Activo)
                    {
                        _dbContext.CargoModuloPermiso.Add(new CargoModuloPermiso
                        {
                            EmpresaId = empresaIdClaim,
                            CargoId = cargoId,
                            ModuloId = modulo.ModuloId,
                            Activo = true,
                            FechaCreacion = DateTime.UtcNow
                        });
                    }
                }
                else
                {
                    // Si existe, solo actualizamos el campo Activo
                    permiso.Activo = modulo.Activo;
                    permiso.FechaModificacion = DateTime.UtcNow;
                }
            }

            _dbContext.SaveChanges();

            TempData["SuccessMessage"] = "Permisos actualizados correctamente.";

            return RedirectToAction("Index", new { cargoId = cargoId });
        }

    }
}
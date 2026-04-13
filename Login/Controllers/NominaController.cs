using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Plataforma.Models;
using Plataforma.Services;

namespace Plataforma.Controllers
{
    [Authorize]
    public class NominaController : Controller
    {
        private readonly INominaService _nominaService;

        public NominaController(INominaService nominaService)
        {
            _nominaService = nominaService;
        }

        public async Task<IActionResult> Index()
        {
            var periodos = await _nominaService.ObtenerPeriodosAsync();
            ViewBag.UltimasLiquidaciones = await _nominaService.ObtenerUltimasLiquidacionesAsync();
            return View(periodos);
        }

        public async Task<IActionResult> Contratos()
        {
            var data = await _nominaService.ObtenerContratosAsync();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> CrearContrato()
        {
            var vm = await ConstruirContratoVmAsync();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearContrato(ContratoFormVm vm)
        {
            if (!ModelState.IsValid)
            {
                vm = await ConstruirContratoVmAsync(vm);
                return View(vm);
            }

            try
            {
                await _nominaService.CrearContratoAsync(vm);
                TempData["ok"] = "Contrato creado correctamente.";
                return RedirectToAction(nameof(Contratos));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                vm = await ConstruirContratoVmAsync(vm);
                return View(vm);
            }
        }

        public async Task<IActionResult> Periodos()
        {
            var data = await _nominaService.ObtenerPeriodosAsync();
            return View(data);
        }

        [HttpGet]
        public IActionResult CrearPeriodo()
        {
            return View(new PeriodoNominaFormVm
            {
                FechaInicio = DateTime.Today,
                FechaFin = DateTime.Today
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearPeriodo(PeriodoNominaFormVm vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                await _nominaService.CrearPeriodoAsync(vm);
                TempData["ok"] = "Período creado correctamente.";
                return RedirectToAction(nameof(Periodos));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
        }

        public async Task<IActionResult> Conceptos()
        {
            var data = await _nominaService.ObtenerConceptosAsync();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> CrearConcepto()
        {
            var vm = await ConstruirConceptoVmAsync();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearConcepto(ConceptoNominaFormVm vm)
        {
            if (!ModelState.IsValid)
            {
                vm = await ConstruirConceptoVmAsync(vm);
                return View(vm);
            }

            try
            {
                await _nominaService.CrearConceptoAsync(vm);
                TempData["ok"] = "Concepto creado correctamente.";
                return RedirectToAction(nameof(Conceptos));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                vm = await ConstruirConceptoVmAsync(vm);
                return View(vm);
            }
        }

        public async Task<IActionResult> Asignaciones(int? cedula)
        {
            var data = await _nominaService.ObtenerAsignacionesAsync(cedula);
            ViewBag.Cedula = cedula;
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> AsignarConcepto()
        {
            var vm = await ConstruirAsignacionVmAsync();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarConcepto(AsignarConceptoEmpleadoVm vm)
        {
            if (!ModelState.IsValid)
            {
                vm = await ConstruirAsignacionVmAsync(vm);
                return View(vm);
            }

            try
            {
                await _nominaService.AsignarConceptoAsync(vm);
                TempData["ok"] = "Concepto asignado correctamente.";
                return RedirectToAction(nameof(Asignaciones), new { cedula = vm.Cedula });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                vm = await ConstruirAsignacionVmAsync(vm);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerarNovedadesDesdeCierreCaja(int idPeriodo)
        {
            var usuario = User.Identity?.Name ?? "sistema";

            try
            {
                var resultado = await _nominaService.GenerarNovedadesDesdeCierreCajaAsync(idPeriodo, usuario);

                TempData["ok"] = $"Proceso ejecutado. " +
                                 $"Creadas: {resultado.NovedadesCreadas}, " +
                                 $"Actualizadas: {resultado.NovedadesActualizadas}, " +
                                 $"Base utilidad procesada: {resultado.BaseVentasProcesada:N2}.";
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Liquidaciones(int? idPeriodo, int? cedula)
        {
            var data = await _nominaService.ObtenerLiquidacionesAsync(idPeriodo, cedula);
            ViewBag.IdPeriodo = idPeriodo;
            ViewBag.Cedula = cedula;
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> GenerarLiquidacion()
        {
            var vm = await ConstruirGenerarLiquidacionVmAsync();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerarLiquidacion(GenerarLiquidacionVm vm)
        {
            if (!ModelState.IsValid)
            {
                vm = await ConstruirGenerarLiquidacionVmAsync(vm);
                return View(vm);
            }

            try
            {
                var usuario = User.Identity?.Name ?? "sistema";
                var liquidacion = await _nominaService.GenerarLiquidacionAsync(vm.Cedula, vm.IdPeriodo, usuario);

                TempData["ok"] = "Liquidación generada correctamente.";
                return RedirectToAction(nameof(DetalleLiquidacion), new { id = liquidacion.IdLiquidacion });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                vm = await ConstruirGenerarLiquidacionVmAsync(vm);
                return View(vm);
            }
        }

        public async Task<IActionResult> DetalleLiquidacion(int id)
        {
            var liquidacion = await _nominaService.ObtenerLiquidacionDetalleAsync(id);
            if (liquidacion == null)
                return NotFound();

            return View(liquidacion);
        }

        private async Task<ContratoFormVm> ConstruirContratoVmAsync(ContratoFormVm? vm = null)
        {
            vm ??= new ContratoFormVm();

            var empleados = await _nominaService.ObtenerEmpleadosAsync();
            var cargos = await _nominaService.ObtenerCargosAsync();

            vm.Empleados = empleados
                .Select(x => new SelectListItem
                {
                    Value = x.Cedula.ToString(),
                    Text = $"{x.Cedula} - {x.Nombre} {x.Apellido}"
                })
                .ToList();

            vm.Cargos = cargos
                .Select(x => new SelectListItem
                {
                    Value = x.Id_tipo.ToString(),
                    Text = $"{x.NombreCargo} - {(x.Area != null ? x.Area.NombreArea : "Sin área")}"
                })
                .ToList();

            vm.Empleados.Insert(0, new SelectListItem { Value = "0", Text = "Seleccione un empleado" });
            vm.Cargos.Insert(0, new SelectListItem { Value = "0", Text = "Seleccione un cargo" });

            return vm;
        }

        private async Task<ConceptoNominaFormVm> ConstruirConceptoVmAsync(ConceptoNominaFormVm? vm = null)
        {
            vm ??= new ConceptoNominaFormVm();

            var servicios = await _nominaService.ObtenerServiciosAsync();

            vm.Servicios = servicios
                .Select(x => new SelectListItem
                {
                    Value = x.IdServicio.ToString(),
                    Text = x.NombreServicio
                })
                .ToList();

            vm.Servicios.Insert(0, new SelectListItem { Value = "", Text = "Sin servicio asociado" });

            return vm;
        }

        private async Task<AsignarConceptoEmpleadoVm> ConstruirAsignacionVmAsync(AsignarConceptoEmpleadoVm? vm = null)
        {
            vm ??= new AsignarConceptoEmpleadoVm();

            var empleados = await _nominaService.ObtenerEmpleadosAsync();
            var conceptos = await _nominaService.ObtenerConceptosAsync();

            vm.Empleados = empleados
                .Select(x => new SelectListItem
                {
                    Value = x.Cedula.ToString(),
                    Text = $"{x.Cedula} - {x.Nombre} {x.Apellido}"
                })
                .ToList();

            vm.Conceptos = conceptos
                .Where(x => x.EsActivo)
                .Select(x => new SelectListItem
                {
                    Value = x.IdConcepto.ToString(),
                    Text = $"{x.Codigo} - {x.Nombre}"
                })
                .ToList();

            vm.Empleados.Insert(0, new SelectListItem { Value = "0", Text = "Seleccione un empleado" });
            vm.Conceptos.Insert(0, new SelectListItem { Value = "0", Text = "Seleccione un concepto" });

            return vm;
        }

        private async Task<GenerarLiquidacionVm> ConstruirGenerarLiquidacionVmAsync(GenerarLiquidacionVm? vm = null)
        {
            vm ??= new GenerarLiquidacionVm();

            var empleados = await _nominaService.ObtenerEmpleadosAsync();
            var periodos = await _nominaService.ObtenerPeriodosAsync();

            vm.Empleados = empleados
                .Select(x => new SelectListItem
                {
                    Value = x.Cedula.ToString(),
                    Text = $"{x.Cedula} - {x.Nombre} {x.Apellido}"
                })
                .ToList();

            vm.Periodos = periodos
                .Select(x => new SelectListItem
                {
                    Value = x.IdPeriodo.ToString(),
                    Text = $"{x.Descripcion} ({x.FechaInicio:yyyy-MM-dd} a {x.FechaFin:yyyy-MM-dd})"
                })
                .ToList();

            vm.Empleados.Insert(0, new SelectListItem { Value = "0", Text = "Seleccione un empleado" });
            vm.Periodos.Insert(0, new SelectListItem { Value = "0", Text = "Seleccione un período" });

            return vm;
        }

        public async Task<IActionResult> Areas()
        {
            var data = await _nominaService.ObtenerAreasAsync();
            return View(data);
        }

        [HttpGet]
        public IActionResult CrearArea()
        {
            return View(new AreaFormVm());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearArea(AreaFormVm vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                await _nominaService.CrearAreaAsync(vm);
                TempData["ok"] = "Área creada correctamente.";
                return RedirectToAction(nameof(Areas));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
        }

        public async Task<IActionResult> CargoAreas()
        {
            var cargos = await _nominaService.ObtenerCargosAsync();
            return View(cargos);
        }

        [HttpGet]
        public async Task<IActionResult> AsignarAreaCargo()
        {
            var vm = await ConstruirAsignarAreaCargoVmAsync();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarAreaCargo(AsignarAreaCargoVm vm)
        {
            if (!ModelState.IsValid)
            {
                vm = await ConstruirAsignarAreaCargoVmAsync(vm);
                return View(vm);
            }

            try
            {
                await _nominaService.AsignarAreaCargoAsync(vm);
                TempData["ok"] = "Área asignada al cargo correctamente.";
                return RedirectToAction(nameof(CargoAreas));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                vm = await ConstruirAsignarAreaCargoVmAsync(vm);
                return View(vm);
            }
        }

        private async Task<AsignarAreaCargoVm> ConstruirAsignarAreaCargoVmAsync(AsignarAreaCargoVm? vm = null)
        {
            vm ??= new AsignarAreaCargoVm();

            var cargos = await _nominaService.ObtenerCargosAsync();
            var areas = await _nominaService.ObtenerAreasAsync();

            vm.Cargos = cargos
                .Select(x => new SelectListItem
                {
                    Value = x.Id_tipo.ToString(),
                    Text = $"{x.NombreCargo}"
                })
                .ToList();

            vm.Areas = areas
                .Select(x => new SelectListItem
                {
                    Value = x.IdArea.ToString(),
                    Text = x.NombreArea
                })
                .ToList();

            vm.Cargos.Insert(0, new SelectListItem { Value = "0", Text = "Seleccione un cargo" });
            vm.Areas.Insert(0, new SelectListItem { Value = "0", Text = "Seleccione un área" });

            return vm;
        }
    }
}
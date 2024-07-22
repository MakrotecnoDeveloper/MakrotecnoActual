// Ubicación: Filters/RoleAuthorizationFilter.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Plataforma.Models; // Cambia esto al namespace adecuado
using Plataforma.Servicios.Contrato; // Cambia esto al namespace adecuado
using System.Linq;

public class RoleAuthorizationFilter : IAuthorizationFilter
{
    private readonly int _requiredIdCargo;
    private readonly IUsuarioService _usuarioService;

    public RoleAuthorizationFilter(int requiredIdCargo, IUsuarioService usuarioService)
    {
        _requiredIdCargo = requiredIdCargo;
        _usuarioService = usuarioService;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        var userCedula = user.FindFirst("Cedula")?.Value;

        if (!string.IsNullOrEmpty(userCedula))
        {
            int cedulaEmpleado;
            if (int.TryParse(userCedula, out cedulaEmpleado))
            {
                var sedeEmpleado = _usuarioService.ObtenerSedeEmpleadoPorCedula(cedulaEmpleado);

                if (sedeEmpleado == null || sedeEmpleado.id_cargo != _requiredIdCargo)
                {
                    context.Result = new RedirectToActionResult("Denied", "Errores", null);
                }
            }
        }
        else
        {
            context.Result = new RedirectToActionResult("Error", "Errores", new { mensaje = "Error a la hora de procesar los datos en la lista de usuarios" });
        }
    }
}

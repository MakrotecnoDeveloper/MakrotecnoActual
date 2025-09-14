using Plataforma.Models;
using System.Security.Claims;
namespace Plataforma.Servicios.Contrato
{
    public interface IOrdenServicioService
    {
        Task AgregarDispositivoAsync(Dispositivos dispositivo);
        Task<List<OrdenServicios>> ObtenerTodasAsync();
        Task<OrdenServicios?> ObtenerPorIdAsync(int id);
        Task<OrdenServicios> CrearAsync(OrdenServicios orden, string cedulaClaim);
        Task ActualizarAsync(OrdenServicios orden, int cedulaEmpleado);
        Task<List<HistOrdSer>> ObtenerOrdenPorIdAsync(int idOrden);
        Task<(OrdenServicios Orden, bool MostrarAgregarProductos)> ObtenerOrdenYPermisosAsync(int idOrden);
        Task CrearHistOrdenAsync(HistOrdSer nuevaHistOrden, string[] Cod_Producto, int cedula);
        Task<bool> ActualizarOrdenAsync(OrdenServicios orden, ClaimsPrincipal usuario);
        Task<List<Empleados>> ObtenerEmpleadosAsync();
        Task<OrdenServicios?> ActualizarOrdenTecnico(OrdenServicios model);
        Task<OrdenServicioRowDTO> GetOrdenRowAsync(int idOrden);
    }
}

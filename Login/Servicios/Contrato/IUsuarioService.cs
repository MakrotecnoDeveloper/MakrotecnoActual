using Plataforma.Models;
using System.Security.Claims;
namespace Plataforma.Servicios.Contrato
{
    public interface IUsuarioService
    {
        Task<List<Empleados>> ObtenerUsuarios();
        Task<List<Empleados>> ObtenerTodos();
        Task<Empleados> ObtenerPorCedula(int cedula);
        Task InsertarEmpleado(Empleados empleado);
        Task ActualizarEmpleado(Empleados empleado);
        int ObtenerRolPermisos(int cedula);
        RolPermisoDTO? ObtenerNombreRolPermisos(int rolEmpleado);
        int TraerUltimoIDPdv(int cedulaEmpleado);
        bool ServValidarDisponSede(int cedula);
        LogsLogin? InsertarLogLogin(int cedulaEmpleado, string correoEmpleado, int estado, int idPDV);
        Empleados? GetUsuarios(int cedula, string password);
        //Task<Empleado> SaveUsuario(Empleado modelo);
		List<Infopdv> FunValidarPDV(int cedula);
        List<TipoCargo> ObtenerCargos();
        object ValidarExistenciaEmpresa(string nit);
        List<Empresas> ObtenerEmpresas();
        TipoCargo? ValidarCargo(string nombreCargo);
        IEnumerable<TipoCargo> InsertarCargos(string nombreCargo, string descripcionCargo, string id_empresa);
        List<ActividadesEconomicas> ObtenerActividadesEconomicas();
        void InsertarEmpresa(
        string nit,
        string nombreEmpresa,
        string pais,
        string calle,
        string carrera,
        string ciudad,
        string departamento,
        string indicativo,
        string numero,
        string estado,
        int actividadEconomicaId
    );
        List<Sede> ObtenerSedes();
        Sede? ValidarExistenciaSede(string nombreSede);
        IEnumerable<Sede> InsertarSede(string id_empresa, string nombreSede, string ciudad, string direccion, string telefono);
        EmpleadoEmpresa? ValidarExisEmpleadoEmpresa(string id_empresa, int cedula);
        IEnumerable<EmpleadoEmpresa> InsertarEmpleadoEmpresa(string id_empresa, int cedula);
        EmpleadoSedeViewModel? EmpleadoSede();
        List<Sede> GetSedesByEmpresaId(string empresaId);
        Empleados? ValidarCedula(int cedula);
        string? ObtenerIdEmpresa(int cedula);
        List<Sede> ObtenerSedes(string idEmpresa);
        bool ObtenerSedePorEmpleado(int cedula);
        List<TipoCargo> ObtenerCargos(string idEmpresa);
        bool InsertarSedeEmpleado(int cedula, int idSede, int idCargo);
        Task<List<FacProUserViewModel>> TraerFactXDia(int cedula, int idPDVActual);
        Task<IEnumerable<ClientesPlataforma>> ObtenerCuentasProximas(int idPlataforma);
        List<Producto> ProductosAbarrotes();
        Task<bool> ActualizarProductoAsync(string id, string campo, string newVal);
        Task<bool> InsertProInventario(string codigoProducto, string nombreProducto, int cantidadProducto, decimal? valorNetoProductoFloat, decimal? valorVentaProductoFloat, int valorUnidadInt, string id_empresa, int categoria, int estado, string ubicacion, int IdProveedor);
        Task<bool> EliminarProductoXIdAsync(string id);
        Infopdv? SeleccionarNombrePDV(int selectedPDV);
        int? ValidarExistenteIdPDV(int idPDV, int cedula);
        Syncpdv AgregarEstadoPDV(int estadopdv, int idPDV, int cedula);
        bool InsertAddClient(int cedulaCliente, string nombreCliente, string empresaCliente, string ciudadCliente, string telefonoCliente, string correoCliente, string direccionCliente);
        List<Clientes> ServVisuaCliente();
        bool InsertAddProveedor(string nit, string razonSocial, string direccion, string celular, string correo);
        List<Proveedores> ServVisuaProveedor();
        List<Servicio> ServTraerServicios();
        Task<string> ObtenerIdEmpresaAsync(int cargoId);
        Task<bool> CrearPuntoVentaAsync(Infopdv infopdv);
        Task<Clientes?> ObtenerPorIdAsync(int id);
        Task<Proveedores?> ObtenerPorIdAsyncProveedor(int id);
        Task ActualizarProveedorAsync(Proveedores proveedores);
        Task ActualizarClienteAsync(Clientes clientes);
        Task<EmpleadoPdvViewModel> ObtenerDatosAsignacion(string cedulaUsuario);
        List<Agendamientos> AgendamientosServicios();
        bool GuardarEdicionServicio(Agendamientos model);
        Agendamientos ObtenerAgendamientoPorId(int id);
        bool AprobarAgendamiento(int id);
        bool EliminarAgendamiento(int id);
        Task<List<Producto>> ConsultarCatProductos(int id);
        Task<bool> RegistrarAgendamientoAsync(string codProducto, DateTime fecha, string NombreCliente, string CelularCliente);
        Task<bool> ExisteFechaAsync(string codProducto, DateTime fecha);
        Sede ObtenerSedePorId(int idSede);
        bool UsuarioTieneAccesoAPdv(int cedula, int pdvId);

    }
}

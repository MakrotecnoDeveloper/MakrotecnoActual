using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IUsuarioService
    {
        List<Empleado> ObtenerUsuarios();
        int ObtenerRolPermisos(int cedula);
        string? ObtenerNombreRolPermisos(int rolEmpleado);
        int TraerUltimoIDPdv(int cedulaEmpleado);
        bool ServValidarDisponSede(int cedula);
        LogsLogin? InsertarLogLogin(int cedulaEmpleado, string correoEmpleado, int estado, int idPDV);
        Empleado? GetUsuarios(int cedula, string password);
        Task<Empleado> SaveUsuario(Empleado modelo);
		List<Infopdv> FunValidarPDV(int cedula);
		bool ValidarEmpleado(int cedula);
        IEnumerable<Empleado> RegistrarEmpleado(int cedula, string nombre, string apellido, string genero, string correo, string rh, string celular, string contrasena);
        List<Empleado> BuscarUsuario(int id);
        void EditarEmpleado(Empleado empleado, int cedula, string nombre, string apellido, string genero, string correo, string rh, string celular, string contrasena);
        List<TipoCargo> ObtenerCargos();
        Empresas? ValidarExistenciaEmpresa(string nit);
        List<Empresas> ObtenerEmpresas();
        TipoCargo? ValidarCargo(string nombreCargo);
        IEnumerable<TipoCargo> InsertarCargos(string nombreCargo, string descripcionCargo, string id_empresa);
        IEnumerable<Empresas> InsertarEmpresa(string nit, string nombreEmpresa, string pais, string calle, string carrera, string ciudad, string departamento, string indicativo, string numero);
        List<Sede> ObtenerSedes();
        Sede? ValidarExistenciaSede(string nombreSede);
        IEnumerable<Sede> InsertarSede(string id_empresa, string nombreSede, string ciudad, string direccion, string telefono);
        EmpleadoEmpresa? ValidarExisEmpleadoEmpresa(string id_empresa, int cedula);
        IEnumerable<EmpleadoEmpresa> InsertarEmpleadoEmpresa(string id_empresa, int cedula);
        EmpleadoSedeViewModel? EmpleadoSede();
        List<Sede> GetSedesByEmpresaId(string empresaId);
        Empleado? ValidarCedula(int cedula);
        string? ObtenerIdEmpresa(int cedula);
        List<Sede> ObtenerSedes(string idEmpresa);
        bool ObtenerSedePorEmpleado(int cedula);
        List<TipoCargo> ObtenerCargos(string idEmpresa);
        bool InsertarSedeEmpleado(int cedula, int idSede, int idCargo);
        List<FacProUserViewModel> TraerFactXDia(int cedula, int traerIdNombrePDVActual);
        Task<IEnumerable<ClientesPlataforma>> ObtenerCuentasProximas(int idPlataforma);
        List<Producto> ProductosAbarrotes();
        Task<bool> ActualizarProductoAsync(string id, string campo, string newVal);
        Task<bool> InsertProInventario(string nombreProducto, int cantidadProducto, float valorNetoProductoFloat, float valorVentaProductoFloat, int valorUnidadInt, string id_empresa, string categoria, int estado, string ubicacion);
        Task<bool> EliminarProductoXIdAsync(string id);
        Infopdv? SeleccionarNombrePDV(int selectedPDV);
        int? ValidarExistenteIdPDV(int idPDV, int cedula);
        Syncpdv AgregarEstadoPDV(int estadopdv, int idPDV, int cedula);
        bool InsertAddClient(int cedulaCliente, string nombreCliente, string empresaCliente, string ciudadCliente, string telefonoCliente);
        List<Cliente> ServVisuaCliente();
        bool InsertAddProveedor(string nit, string razonSocial, string direccion, string celular, string correo);
        List<Proveedores> ServVisuaProveedor();
        List<Servicio> ServTraerServicios();
        Task<List<MenuOption>> GetMenuOptionsAsync(int cargoId, string idEmpresa);
        Task<string> ObtenerIdEmpresaAsync(int cargoId);
    }
}

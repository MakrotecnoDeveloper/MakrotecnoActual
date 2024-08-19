//establece un contrato que define las operaciones necesarias para interactuar con usuarios en una aplicación
using Login.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using System.Security.Claims;
namespace Plataforma.Servicios.Contrato
{
    public interface IUsuarioService
    {
        List<Empleado> ObtenerUsuarios();
        int ObtenerRolPermisos(int cedula);
        string ObtenerNombreRolPermisos(int rolEmpleado);
        Sedeempleado ObtenerSedeEmpleadoPorCedula(int cedulaEmpleado);
        Task<logsLogin> InsertarLogLogin(int cedulaEmpleado, string correoEmpleado, int estado);
        Task<Empleado> GetUsuarios(string correo, string password);
        //el segundo metodo guarda usuarios
        Task<Empleado> SaveUsuario(Empleado modelo);
        bool validarEmpleado(int cedula);
        IEnumerable<Empleado> RegistrarEmpleado(int cedula, string nombre, string apellido, string genero, string correo, string rh, string celular, string contrasena);
        List<Empleado> BuscarUsuario(int id);
        void EditarEmpleado(int cedula, string nombre, string apellido, string genero, string correo, string rh, string celular, string contrasena);
        List<TipoCargo> ObtenerCargos();
        List<Empresas> ObtenerEmpresas();
        IEnumerable<TipoCargo> InsertarCargos(string nombreCargo, string descripcionCargo, string id_empresa);
        IEnumerable<Empresas> InsertarEmpresa(string nit, string nombreEmpresa, string pais, string calle, string carrera, string ciudad, string departamento, string indicativo, string numero);
        List<Sede> ObtenerSedes();
        IEnumerable<Sede> InsertarSede(string id_empresa, string nombreSede, string ciudad, string direccion, string telefono);
        IEnumerable<EmpleadoEmpresa> InsertarEmpleadoEmpresa(string id_empresa, int cedula);
        EmpleadoSedeViewModel? EmpleadoSede();
        List<Sede> GetSedesByEmpresaId(string empresaId);
        Empleado ValidarCedula(int cedula);
        string? ObtenerIdEmpresa(int cedula);
        List<Sede> ObtenerSedes(string idEmpresa);
        bool ObtenerSedePorEmpleado(int cedula);
        List<TipoCargo> ObtenerCargos(string idEmpresa);
        bool InsertarSedeEmpleado(int cedula, int idSede, int idCargo);
        List<FacProuserViewModel> TraerFactXDia(Claim cedulaClaim);
        Task<IEnumerable<ClientesPlataforma>> ObtenerCuentasProximas(int idPlataforma);
    }
}

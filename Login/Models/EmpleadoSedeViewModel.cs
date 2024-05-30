using Plataforma.Models;

namespace Login.Models
{
    public class EmpleadoSedeViewModel
    {
        public List<Empleado> Empleados { get; set; }
        public List<Empresas> Empresas { get; set; }
        public List<Sede> Sedes { get; set; }
        public List<EmpleadoConSedeYEmpresa> EmpleadoConSedeYEmpresas { get; set; }
    }
}

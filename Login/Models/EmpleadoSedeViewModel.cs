namespace Plataforma.Models;
public class EmpleadoSedeViewModel
{
    public List<Empleados>? Empleados { get; set; }
        public List<Empresas>? Empresas { get; set; }
        public List<Sede>? Sedes { get; set; }
        public List<EmpleadoConSedeYEmpresa>? EmpleadoConSedeYEmpresas { get; set; }
    }

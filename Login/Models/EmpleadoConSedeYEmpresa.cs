namespace Plataforma.Models;
public class EmpleadoConSedeYEmpresa
{
        public Empleados? Empleado { get; set; }
        public Sede? Sede { get; set; }
        public Empresas? Empresa { get; set; }
        public TipoCargo? TipoCargo { get; set; }
}

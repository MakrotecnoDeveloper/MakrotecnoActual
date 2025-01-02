namespace Plataforma.Models;
public class EmpleadoConSedeYEmpresa
{
        public Empleado? Empleado { get; set; }
        public Sede? Sede { get; set; }
        public Empresas? Empresa { get; set; }
        public TipoCargo? TipoCargo { get; set; }
}

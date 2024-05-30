using Plataforma.Models;

namespace Login.Models
{
    public class EmpleadoEmpresaViewModel
    {
        public string id_empresa {  get; set; }
        public int id_empleadoE {  get; set; }
        public int cedula { get; set; }
        public string NombreEmpresa { get; set; }
    }
}
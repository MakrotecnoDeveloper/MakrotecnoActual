namespace Plataforma.Models.Dto.Pedido
{
    public class ContextoAccesoDto
    {
        public int Cedula { get; set; }
        public string EmpresaId { get; set; }
        public int SedeId { get; set; }
        public int PdvId { get; set; }
        public string NombreRol { get; set; }

        public bool EsAdministradorLider =>
            string.Equals(NombreRol, "Administrador", StringComparison.OrdinalIgnoreCase);
    }
}

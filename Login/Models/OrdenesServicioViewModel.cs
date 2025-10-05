namespace Plataforma.Models
{
    public class OrdenesServicioViewModel
    {
        public List<TipoDispositivos> TipoDispositivos { get; set; }
        public List<Dispositivos> Dispositivos { get; set; }
        public List<Clientes> Clientes { get; set; }
        public List<OrdenServicios> OrdenServicios { get; set; }
        public List<Proveedores> Proveedores { get; set; }
        public int IdOrden { get; set; }
        public string Estado { get; set; }
        public bool MostrarAgregarProductos { get; set; }
        public OrdenServicios orden { get; set; }
        public List<MetodoPagos> MetodoPagos { get; set; }
        public List<Empleados>? Empleados { get; set; }
        public List<Sedeempleado>? Sedeempleados { get;set; }

    }
}

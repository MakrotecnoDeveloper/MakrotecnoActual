//establece un contrato que define las operaciones necesarias para interactuar con usuarios en una aplicación
using Login.Models;
using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IProductoService
    {
        Task<bool> AgregarProductoAsync(string id_empresa, string codigo, string descripcion, float valor_neto, float valor_unitario, int stock, string categorias);
        List<Producto> ObtenerProductos();
        List<Producto> ObtenerProductosInventarioWeb();
        List<Producto> BuscarProductos(string searchTerm, string categoriaTerm);
        List<Producto> SinStock(string searchTerm, string categoriaTerm);
        List<Producto> BuscarProSinStock(string searchTerm, string categoriaTerm);
        Task<bool> AgregarStockAsync(string idProducto, int cantidad);
        IEnumerable<Producto> EditarStock(string id, int cantidad, int opcion);
        void EditarProducto(string codigo, string nombreProducto, float valorNeto, float valorVenta, int valorUnidad, int cantidad, string categoria, string idEmpresa, int estado);
        void EliminarProducto(string id);
        void inserPlataformaService(int idPlataforma, string descripcion, int valorventa, int valorneto, DateTime fechaInipago, DateTime fechaFinpago, int cantidad, string correo, string contrasena, int cedula, int estado);
        List<Plataformas> traerPlataformasExistentes();
        List<Plataformasuscripcion> SuscripcionesActivas();
        Task<List<Plataformasuscripcion>> ObtenerSuscripcionesActivas(int plataformaId);
        Task<List<ClientePlataformaDTO>> ObtenerDatosSuscripcion(int suscripcionId);
        Task<List<ClientePlataformaDTO>> ObtenerDatosPlataforma(int suscripcionId);
        Task<bool> EliminarClienteAsync(int idClientePlataforma);
        void servicioInsertarVentClientPlataforma(string nombrecliente, string celularcliente, string correo, string contrasena, int idPltfSuscripcion, int cantidad, string ppm, DateTime feciniplat, DateTime fecfinplat, int valorventa, int valorneto, int cedula, int estado, string clave);
        Task ActualizarCliente(int id, int estado, int idCliente);
        List<Producto> traerProductosXCategoria(string categoria);
        Task<List<string>> BuscarProductosAsync(string consulta);
        Task<string> GenerarRespuestaAsync(string consulta, List<string> productos);
        ProveedorProductosViewModel TraerProveedorProductos(int cedula);
        bool HistoricoCompra(HistoricoCompras historicoCompra);
        List<Factura> ObtenerFacturasPorFechaYUsuario(DateTime fecha, int cedula);
        DetallesFacturaViewModel ObtenerDetallesFactura(int codFactura);
    }
}

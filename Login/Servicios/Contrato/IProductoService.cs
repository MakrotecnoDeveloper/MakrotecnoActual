using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IProductoService
    {
        Task<bool> AgregarProductoAsync(string id_empresa, string codigo, string descripcion, float valor_neto, float valor_unitario, decimal stock, int categorias, int id_proveedor);
        List<CategoriaProductos> ObtenerCategorias();
        List<Producto> ObtenerProductos();
        List<Producto> ObtenerProductosPorCategoria(int idCategoria);
        List<CategoriaProductos> ObtenerCategoriaProductos(int idServicio);
        Task<List<CategoriaProductos>> ObtenerCategoriasPorServicio(int idServicio);
        List<Producto> BuscarProductos(string searchTerm, int categoriaTerm);
        List<Producto> SinStock(string searchTerm, int categoriaTerm);
        List<Producto> BuscarProSinStock(string searchTerm, int categoriaTerm);
        Task<bool> AgregarStockAsync(string idProducto, int cantidad);
        IEnumerable<Producto> EditarStock(string id, int cantidad, int opcion);
        void EditarProducto(string codigo, float valorNeto, float valorVenta, int valorUnidad, int cantidad);
        void EliminarProducto(string id);
        void InserPlataformaService(int idPlataforma, string descripcion, int valorventa, int valorneto, DateTime fechaInipago, DateTime fechaFinpago, int cantidad, string correo, string contrasena, int cedula, int estado);
        List<Plataformas> TraerPlataformasExistentes();
        List<Plataformasuscripcion> SuscripcionesActivas();
        Task<List<Plataformasuscripcion>> ObtenerSuscripcionesActivas(int plataformaId);
        Task<List<ClientePlataformaDTO>> ObtenerDatosSuscripcion(int suscripcionId);
        Task<List<ClientePlataformaDTO>> ObtenerDatosPlataforma(int suscripcionId);
        Task<bool> EliminarClienteAsync(int idClientePlataforma);
        void ServicioInsertarVentClientPlataforma(string nombrecliente, string celularcliente, string correo, string contrasena, int idPltfSuscripcion, int cantidad, string ppm, DateTime feciniplat, DateTime fecfinplat, int valorventa, int valorneto, int cedula, int estado, string clave);
        Task ActualizarCliente(int id, int estado, int idCliente);
        List<Producto> TraerProductosXCategoria(int categoria);
        //ProveedorProductosViewModel TraerProveedorProductos(int cedula);
        Task<bool> CrearCategoriaAsync(CategoriaProductos categoria);
        Task<List<Servicio>> ObtenerServiciosAsync();
        Task<List<Servicio>> ObtenerServicios();
        Task<List<Proveedores>> ObtenerProveedores();
        Task<int?> SeleccionarServicio(Producto p);
        Task<Servicio> CrearServicio(Servicio servicio);
        Task<decimal> ObtenerTotalStockAsync(int? sedeId);
        Task<int> ObtenerSkusConStockAsync(int? sedeId);
        Task<PagedResult<ProductoStockVm>> ObtenerStockAsync(
            int? sedeId, string? q, int page, int pageSize,
            string? sortBy = null, bool desc = false);
        Task<(decimal totalUnidades, int skusConStock)> ResumenAsync(int? sedeId, string? q);

        Task AplicarMovimientoAsync(int productoId, int sedeId, decimal delta, string? motivo = null);
        Task<(int insertados, int omitidos)> InsertarLoteAsync(IEnumerable<ProductoInsertDto> lote);
        Task<List<Servicio>> GetServiciosAsync();
        Task<List<CategoriaProductos>> GetCategoriasPorServicioAsync(int servicioId);
        Task<List<Proveedores>> GetProveedoresAsync();
        InventarioSede AsignarProductoSede(string producto, int sede, int cantidad, int valorUnitario, string cedulaClaim);
        bool ValidarSedeAsignacionProducto(int idSede);
        bool ValidarProductoAsignacion(string producto);
        bool ValidarCantidadProducto(string producto, decimal cantidad);
        List<Producto> TraerProductosInactivos();
    }
}

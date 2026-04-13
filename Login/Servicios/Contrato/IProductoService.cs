using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IProductoService
    {
        Task<bool> AgregarProductoAsync(string id_empresa, string codigo, string descripcion, decimal? valor_neto, decimal? valor_unitario, decimal? valor_unidad, int unidadMedida, decimal stock, int categorias, int id_proveedor, string? rutaImagen, string? autenticidadProducto, string? condicionProducto);
        List<CategoriaProductos> ObtenerCategorias();
        List<Producto> ObtenerProductos();
        List<Producto> ObtenerProductosPorCategoria(int idCategoria);
        List<CategoriaProductos> ObtenerCategoriaProductos(int idServicio);
        List<ProductoTiendaDTO> ObtenerProductosPorServicioYSede(int idServicio, int sedeId);
        Task<List<CategoriaProductos>> ObtenerCategoriasPorServicio(int idServicio);
        List<Sede> ObtenerSedes();
        ProductosCategoriaViewModel BuscarProductoXImagen(string searchTerm, int categoriaTerm, int sedeId);
        VisualizarProductoPublicoViewModel? ObtenerProductoPublico(string codigo);
        List<Producto> BuscarProductos(string searchTerm);
        Task<bool> AgregarStockAsync(string idProducto, int cantidad);
        IEnumerable<Producto> EditarStock(string id, int cantidad, int opcion);
        void EditarProducto(string codigo, string nombreProducto, decimal? valorNeto, decimal? valorVenta, int valorUnidad, int cantidad, int categorias, int id_proveedor, string? imagen);
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
        Task<List<Empresas>> TraerEmpresas();
        Task<List<UnidadMedida>> TraerUnidadesMedida();
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
        InventarioSede AsignarProductoSede(string producto, int sede, int cantidad, int valorUnitario, string cedulaClaim, int valorNeto);
        bool ValidarSedeAsignacionProducto(int idSede);
        bool ValidarProductoAsignacion(string producto);
        bool ValidarCantidadProducto(string producto, decimal cantidad);
        bool ValidarProductoSede(string producto, int idSede);
        List<Producto> TraerProductosInactivos();
        Task<List<Producto>> BuscarProductosPorCodigo(string codigo);
        List<CategoriaWebEstadoViewModel> ObtenerCategoriasWeb();
        void ActualizarEstadoWebCategorias(List<CategoriaWebEstadoViewModel> categorias);
    }
}

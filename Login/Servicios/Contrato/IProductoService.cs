//establece un contrato que define las operaciones necesarias para interactuar con usuarios en una aplicación
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IProductoService
    {
        Task<bool> AgregarProductoAsync(string id_empresa, string codigo, string descripcion, float valor_neto, float valor_unitario, int stock, string categorias);
        List<Producto> ObtenerProductos();
        List<Producto> BuscarProductos(string searchTerm, string categoriaTerm);
        List<Producto> SinStock(string searchTerm, string categoriaTerm);
        List<Producto> BuscarProSinStock(string searchTerm, string categoriaTerm);
        Task<bool> AgregarStockAsync(string idProducto, int cantidad);
        IEnumerable<Producto> EditarStock(string id, int cantidad, int opcion);
        void EditarProducto(string codigo, string nombreProducto, float valorNeto, float valorVenta, int cantidad, string categoria, string idEmpresa);
        void EliminarProducto(string id);
        void inserPlataformaService(string plataforma, string descripcion, int valorventa, int valorneto, DateTime fechaInipago, DateTime fechaFinpago, int cantidad, string correo, string contrasena, int cedula, int estado);
        List<Plataformas> traerPlataformasExistentes();
        List<ClientePlataformaDTO> TraerCtaClientPlatfExistentes();
        void servicioInsertarVentClientPlataforma(string nombrecliente, string celularcliente, string correo, string contrasena, int idplataforma, int cantidad, string ppm, DateTime feciniplat, DateTime fecfinplat, int valorventa, int valorneto, int cedula, int estado, string clave);
        Task ActualizarCliente(int estado, int id);
    }
}

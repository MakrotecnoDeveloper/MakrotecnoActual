using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface ITercerosService
    {
        Task<Clientes> CrearClienteAsync(Clientes cliente);
        Task<List<Clientes>> ObtenerClientes();
        Task<List<MetodoPagos>> ObtenerMetodosPago();
        Task<List<Proveedores>> ObtenerProveedoresAsync();
    }
}
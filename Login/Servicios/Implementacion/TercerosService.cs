using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;

namespace Plataforma.Servicios.Implementacion
{
    public class TercerosService : ITercerosService
    {
        private readonly BaseAdmContext _dbContext;
        public TercerosService(BaseAdmContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Clientes> CrearClienteAsync(Clientes cliente)
        {
            _dbContext.Cliente.Add(cliente);
            await _dbContext.SaveChangesAsync();
            return cliente;
        }
        public async Task<List<Clientes>> ObtenerClientes()
        {
            return await _dbContext.Cliente.ToListAsync();
        }
        public async Task<List<MetodoPagos>> ObtenerMetodosPago()
        {
            return await _dbContext.MetodoPagos.ToListAsync();
        }
        public async Task<List<Proveedores>> ObtenerProveedoresAsync()
        {
            return await _dbContext.Proveedores.ToListAsync();
        }
    }
}
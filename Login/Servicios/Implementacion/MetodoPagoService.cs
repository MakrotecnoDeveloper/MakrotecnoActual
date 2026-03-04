using DocumentFormat.OpenXml.Bibliography;
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using System.Text.Json;
namespace Plataforma.Servicios.Implementacion
{
    public class MetodoPagoService : IMetodoPagoService
    {
        private readonly BaseAdmContext _dbContext;
        public MetodoPagoService(BaseAdmContext dbContext)
        {
            _dbContext = dbContext;
        }
        // --- Creación de CXC ---
        public async Task<CxcVentas> CrearCuentaPorCobrarAsync(int idVenta, int idCliente, decimal total)
        {
            var cxc = new CxcVentas
            {
                IdVenta = idVenta,
                IdCliente = idCliente,
                Total = total,
                SaldoPendiente = total,
                EstadoCxc = "Pendiente"
            };

            _dbContext.CxcVentas.Add(cxc);
            await _dbContext.SaveChangesAsync();
            return cxc;
        }

        // --- Registro de pagos ---
        public async Task<bool> RegistrarPagoCxcAsync(int idCxc, decimal monto, string observacion = null)
        {
            var cxc = await _dbContext.CxcVentas.FirstOrDefaultAsync(c => c.IdCxc == idCxc);
            if (cxc == null) return false;

            var pago = new CxcPagos
            {
                IdCxc = idCxc,
                MontoPago = monto,
                Observacion = observacion
            };

            _dbContext.CxcPagos.Add(pago);
            cxc.SaldoPendiente -= monto;

            if (cxc.SaldoPendiente <= 0)
            {
                cxc.SaldoPendiente = 0;
                cxc.EstadoCxc = "Pagada";

                // También actualizamos el estado de la factura
                var factura = await _dbContext.Factura.FirstOrDefaultAsync(f => f.IdVenta == cxc.IdVenta);
                if (factura != null)
                    factura.EstadoFactura = "Pagada";
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        // --- Consulta del estado de la CXC ---
        public async Task<CxcVentas> ObtenerCxcPorVentaAsync(int idVenta)
        {
            return await _dbContext.CxcVentas
                .Include(c => c.Pagos)
                .FirstOrDefaultAsync(c => c.IdVenta == idVenta);
        }
    }
}

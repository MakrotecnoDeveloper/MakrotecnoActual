using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plataforma.Servicios.Implementacion
{
    public class FacturaService : IFacturaService
    {
        private readonly BaseAdmContext _dbContext;
        private readonly ILogger<ProductoService> _logger;
        public FacturaService(BaseAdmContext dbContext, IConfiguration configuration, ILogger<ProductoService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }


        public List<Factura> ObtenerFacturasPorFechaYUsuario(DateTime fecha, int cedula)
        {
            return _dbContext.Factura
                .Where(f => f.FechaVenta.Date == fecha.Date && f.Cedula == cedula && f.TipoFactura == "Compra")
                .ToList();
        }
        // Método para obtener los detalles de la factura seleccionada
        public DetallesFacturaViewModel ObtenerDetallesFactura(int codFactura)
        {
            var factura = _dbContext.Factura
                .Where(f => f.Cod_factura == codFactura)
                .FirstOrDefault();

            var compras = _dbContext.HistoricoCompras
                .Where(h => h.cod_factura == codFactura)
                .ToList();

            if (factura == null || compras.Count == 0)
            {
                return null;
            }

            var model = new DetallesFacturaViewModel
            {
                Factura = factura,
                Compras = compras
            };

            return model;
        }
    }
}

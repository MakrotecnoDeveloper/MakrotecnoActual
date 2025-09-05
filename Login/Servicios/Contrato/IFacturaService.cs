using Plataforma.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plataforma.Servicios.Contrato
{
    public interface IFacturaService
    {
        List<Factura> ObtenerFacturasPorFechaYUsuario(DateTime fecha, int cedula);
        DetallesFacturaViewModel ObtenerDetallesFactura(int codFactura);
    }
}

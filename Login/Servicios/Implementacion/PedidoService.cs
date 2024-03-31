using Microsoft.AspNetCore.Mvc;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Plataforma.Servicios.Implementacion
{
    public class PedidoService : IPedidoService
    {
        private readonly BaseAdmContext _dbContext;
        public PedidoService(BaseAdmContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<Factura> ObtenerFacturas()
        {
            return _dbContext.Factura.ToList();
        }
        public void ActualizarEstadoFacturas()
        {
            var facturasCompletadas = _dbContext.Factura
                .Where(f => f.estado == "Pagado" && f.fechaVenta.AddDays(7) <= DateTime.Now)
                .ToList();

            foreach (var factura in facturasCompletadas)
            {
                factura.estado = "Cerrado";
            }

            _dbContext.SaveChanges();
        }
        public IEnumerable<Factura> CrearFactura(int cod_factura, int cedula_cliente, int cedula_empleado, DateTime fechaVenta, string estado)
        {
            var facturaExistente = _dbContext.Factura.FirstOrDefault(p => p.cod_factura == cod_factura);
            if (facturaExistente != null)
            {
                // Si ya existe un empleado con la misma cédula, puedes manejarlo de acuerdo a tus requerimientos, por ejemplo, lanzar una excepción, devolver un mensaje de error, etc.
                // Aquí estoy lanzando una excepción como ejemplo.
                Console.WriteLine("Ya existe una factura con el mismo codigo");
            }

            // Crear una nueva instancia de Empleado
            var nuevaFactura = new Factura
            {
                cedula_cliente = cedula_cliente,
                cedula = cedula_empleado,
                fechaVenta = fechaVenta,
                estado = estado
            };

            // Agregar el nuevo empleado al contexto de la base de datos
            _dbContext.Factura.Add(nuevaFactura);

            // Guardar los cambios en la base de datos
            _dbContext.SaveChanges();

            // Retornar todos los empleados después de agregar el nuevo empleado
            return _dbContext.Factura.ToList();
        }
    }
}

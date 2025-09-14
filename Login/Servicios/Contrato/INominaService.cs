using Plataforma.Models;

namespace Plataforma.Servicios.Contrato
{
    public interface INominaService
    {
        Task CrearContratoAsync(int Cedula, string TipoContrato, int SalarioBase, DateTime FechaInicio, DateTime FechaFin, bool Estado);
        List<ConceptoServicioVM> GetConceptos(string? json);
        List<string> ObtenerServicios();
        Task CrearConceptoNominaAsync(string Nombre, string Tipo, string ServicioAsociado, decimal Porcentaje, decimal ValorFijo);
        Task AsociarConceptoAsync(int Cedula, int IdConcepto);
        Task<LiquidacionNomina> LiquidarPeriodoAsync(int cedulaEmpleado, DateTime fechaInicio, DateTime fechaFin);
    }
}

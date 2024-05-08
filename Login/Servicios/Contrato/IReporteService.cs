//establece un contrato que define las operaciones necesarias para interactuar con usuarios en una aplicación
using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
namespace Plataforma.Servicios.Contrato
{
    public interface IReporteService
    {
        Task<List<ReporteItem>> GenerarReporte(DateTime fechaInicio, DateTime fechaFin, string tipoReporte);
    }
}
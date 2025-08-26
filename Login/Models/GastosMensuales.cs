using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("GastosMensuales", Schema = "dbo")]
    public class GastosMensuales
    {
        public int IdGasto { get; set; }
        public string Nombre { get; set; } = string.Empty; // título / descripción corta
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }                // día del gasto
        public string? Categoria { get; set; }             // libre o FK si ya la tienes
        public FrecuenciaGasto Frecuencia { get; set; }    // semanal/quincenal/mensual
        public string? Notas { get; set; }                 // opcional
    }

    public enum FrecuenciaGasto : byte
    {
        Semanal = 1,
        Quincenal = 2,
        Mensual = 3
    }
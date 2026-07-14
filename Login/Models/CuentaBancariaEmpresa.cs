using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;

[Table("CuentasBancariasEmpresa", Schema = "dbo")]
public class CuentaBancariaEmpresa
{
    [Key]
    public int IdCuentaBancariaEmpresa { get; set; }

    [Required]
    [StringLength(50)]
    public string IdEmpresa { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string TitularCuenta { get; set; } = string.Empty;

    [StringLength(50)]
    public string? DocumentoTitular { get; set; }

    [Required]
    [StringLength(100)]
    public string Banco { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string TipoCuenta { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string NumeroCuenta { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Convenio { get; set; }

    [StringLength(500)]
    public string? Observacion { get; set; }

    public bool EsPrincipal { get; set; }

    public bool Estado { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.Now;
}
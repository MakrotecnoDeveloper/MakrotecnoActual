using System.ComponentModel.DataAnnotations;

namespace Plataforma.ViewModels.CuentasBancarias;

public class CuentaBancariaEmpresaVm
{
    public int IdCuentaBancariaEmpresa { get; set; }

    [Required(ErrorMessage = "El titular de la cuenta es obligatorio.")]
    [Display(Name = "Titular de la cuenta")]
    public string TitularCuenta { get; set; } = string.Empty;

    [Display(Name = "Documento titular")]
    public string? DocumentoTitular { get; set; }

    [Required(ErrorMessage = "El banco es obligatorio.")]
    public string Banco { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de cuenta es obligatorio.")]
    [Display(Name = "Tipo de cuenta")]
    public string TipoCuenta { get; set; } = string.Empty;

    [Required(ErrorMessage = "El número de cuenta es obligatorio.")]
    [Display(Name = "Número de cuenta")]
    public string NumeroCuenta { get; set; } = string.Empty;

    public string? Convenio { get; set; }

    public string? Observacion { get; set; }

    [Display(Name = "Cuenta principal")]
    public bool EsPrincipal { get; set; }

    public bool Estado { get; set; } = true;

    public DateTime FechaCreacion { get; set; }
}
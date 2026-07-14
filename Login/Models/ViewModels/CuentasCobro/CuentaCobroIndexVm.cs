namespace Plataforma.ViewModels.CuentasCobro;

public class CuentaCobroIndexVm
{
    public int IdCuentaCobro { get; set; }
    public string NumeroCuentaCobro { get; set; } = string.Empty;
    public string NumeroFactura { get; set; } = string.Empty;
    public string NombreCliente { get; set; } = string.Empty;
    public DateTime FechaCuentaCobro { get; set; }
    public decimal ValorNetoPagar { get; set; }
    public string EstadoCuentaCobro { get; set; } = string.Empty;
}
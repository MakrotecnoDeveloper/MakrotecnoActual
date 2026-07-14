using Plataforma.Models;

namespace Plataforma.ViewModels.CuentasCobro;

public class CuentaCobroDetalleVm
{
    public int IdCuentaCobro { get; set; }
    public string NumeroCuentaCobro { get; set; } = string.Empty;

    public int IdFactura { get; set; }
    public string NumeroFactura { get; set; } = string.Empty;

    public DateTime FechaCuentaCobro { get; set; }
    public string? Ciudad { get; set; }

    public string CodigoDocumento { get; set; } = string.Empty;
    public string VersionDocumento { get; set; } = string.Empty;
    public DateTime? FechaAprobacionDocumento { get; set; }

    public string? NombreCliente { get; set; }
    public string? DocumentoCliente { get; set; }
    public string? DireccionCliente { get; set; }
    public string? TelefonoCliente { get; set; }
    public string? CorreoCliente { get; set; }
    public string? BancoCliente { get; set; }

    public string? DetalleServicio { get; set; }

    public decimal ValorTotalServicio { get; set; }
    public decimal RetencionFuente { get; set; }
    public decimal OtrasDeducciones { get; set; }
    public decimal ValorNetoPagar { get; set; }

    public string? Declaracion { get; set; }
    public string? CedulaContratista { get; set; }
    public string? CiudadExpedicionContratista { get; set; }
    public string? FirmaPath { get; set; }

    public string EstadoCuentaCobro { get; set; } = string.Empty;

    public Empresas? Empresa { get; set; }
    public CuentaBancariaEmpresa? CuentaBancaria { get; set; }

    public List<CuentaCobroDetalleItemVm> Detalles { get; set; } = new();
}
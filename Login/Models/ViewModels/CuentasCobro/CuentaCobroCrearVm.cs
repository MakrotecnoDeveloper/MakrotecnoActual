using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Plataforma.ViewModels.CuentasCobro;

public class CuentaCobroCrearVm
{
    public int IdFactura { get; set; }
    public int IdVenta { get; set; }
    public int IdCliente { get; set; }

    public string NumeroFactura { get; set; } = string.Empty;
    public string NumeroCuentaCobro { get; set; } = string.Empty;

    [Required]
    public int IdCuentaBancariaEmpresa { get; set; }

    public DateTime FechaCuentaCobro { get; set; } = DateTime.Now;

    public string? Ciudad { get; set; }

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

    public string CodigoDocumento { get; set; } = "DCCC-000-01";
    public string VersionDocumento { get; set; } = "1";
    public DateTime? FechaAprobacionDocumento { get; set; }

    public string? Declaracion { get; set; }

    public string? CedulaContratista { get; set; }
    public string? CiudadExpedicionContratista { get; set; }

    public List<SelectListItem> CuentasBancarias { get; set; } = new();

    public List<CuentaCobroDetalleItemVm> Detalles { get; set; } = new();
}
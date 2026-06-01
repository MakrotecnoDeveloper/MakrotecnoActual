using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MakroTecno.ViewModels.Compras
{
    public class FacturaCompraViewModel
    {
        public int IdFacturaCompra { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un proveedor.")]
        public int IdProveedor { get; set; }

        public string? PrefijoFactura { get; set; }

        [Required(ErrorMessage = "El número de factura es obligatorio.")]
        public string NumeroFactura { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de compra es obligatoria.")]
        public DateTime FechaCompra { get; set; } = DateTime.Now;

        public string? Observacion { get; set; }

        public string? RutaArchivoActual { get; set; }

        public string? NombreArchivoOriginal { get; set; }

        public IFormFile? ArchivoFactura { get; set; }

        public List<SelectListItem> Proveedores { get; set; } = new();
    }
}
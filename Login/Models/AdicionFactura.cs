using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("AdicionFactura", Schema = "dbo")]

public class AdicionFactura
    {
            [Key]
            public int IdAdicion { get; set; }

            public int IdFactura { get; set; }

            public decimal Valor { get; set; }

            public string Descripcion { get; set; }

            public DateTime Fecha { get; set; }

            public int Cedula { get; set; }

            [ForeignKey("IdFactura")]
            public virtual Factura Factura { get; set; }
    }

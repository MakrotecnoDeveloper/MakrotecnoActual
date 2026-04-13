using Plataforma.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
    public class UnidadMedida
    {
        [Key]
        public int IdUnidad { get; set; }

        public string Nombre { get; set; }
        public string Abreviatura { get; set; }
        public string Tipo { get; set; } // peso / unidad
        public decimal FactorBase { get; set; }

        public ICollection<Producto> Productos { get; set; }
    }

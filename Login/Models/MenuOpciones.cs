using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;
    public class MenuOpciones
    {
        [Key]
        public int IdOpcion { get; set; }

        public int IdModulo { get; set; }

        public string Header { get; set; }      // MODULO ADMINISTRATIVO, MODULO PRODUCTOS, etc.
        public string Grupo { get; set; }       // Parametrizacion Empresa, Compras, etc.

        public string IconoHeader { get; set; } // fas fa-building
        public string IconoGrupo { get; set; }  // fas fa-building
        public string IconoOpcion { get; set; } // far fa-circle

        public string Controller { get; set; }  // Usuario, Producto, Compras...
        public string Action { get; set; }      // Cargos, Empresas, InsertarCompra...

        public int OrdenHeader { get; set; }
        public int OrdenGrupo { get; set; }
        public int OrdenOpcion { get; set; }

        // Relación con Modulos
        public Modulos Modulo { get; set; }
}


public class OpcionMenuDto
{
    public string Titulo { get; set; }      // Cargos, Empresas, Registrar Compra, etc.
    public string Controller { get; set; }
    public string Action { get; set; }
    public string Icono { get; set; }       // far fa-circle
    public int OrdenOpcion { get; set; }
}

public class GrupoMenuDto
{
    public string NombreGrupo { get; set; } // Parametrizacion Empresa, Compras, etc.
    public string IconoGrupo { get; set; }
    public int OrdenGrupo { get; set; }
    public List<OpcionMenuDto> Opciones { get; set; } = new();
}

public class ModuloMenuDto
{
    public string Header { get; set; }      // MODULO ADMINISTRATIVO, MODULO PRODUCTOS, etc.
    public string IconoHeader { get; set; }
    public int OrdenHeader { get; set; }
    public List<GrupoMenuDto> Grupos { get; set; } = new();
}

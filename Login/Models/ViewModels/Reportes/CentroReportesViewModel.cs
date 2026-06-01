using Microsoft.AspNetCore.Mvc.Rendering;

namespace Plataforma.Models.ViewModels.Reportes
{
    public class CentroReportesViewModel
    {
        public CentroReportesFiltroViewModel Filtros { get; set; } = new();

        public List<SelectListItem> TiposReporte { get; set; } = new();
        public List<SelectListItem> Sedes { get; set; } = new();
        public List<SelectListItem> Pdvs { get; set; } = new();
        public List<SelectListItem> Vendedores { get; set; } = new();
        public List<SelectListItem> MetodosPago { get; set; } = new();

        public List<string> Columnas { get; set; } = new();

        public List<CentroReportesFilaViewModel> Filas { get; set; } = new();

        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
        public int RegistrosPorPagina { get; set; } = 20;

        public decimal TotalGeneral { get; set; }
        public string? TituloReporte { get; set; }
        public string? Mensaje { get; set; }
    }

    public class CentroReportesFiltroViewModel
    {
        public string? TipoReporte { get; set; }

        public DateTime? FechaInicial { get; set; }
        public DateTime? FechaFinal { get; set; }

        public string? Concepto { get; set; }

        public int? IdSede { get; set; }
        public int? InfopdvId { get; set; }

        public int? CedulaVendedor { get; set; }

        public string? MetodoPago { get; set; }

        public decimal? StockMinimo { get; set; }
    }

    public class CentroReportesFilaViewModel
    {
        public Dictionary<string, string> Valores { get; set; } = new();
    }

    public static class TiposReporteOperativo
    {
        public const string DetalleVentaProducto = "DETALLE_VENTA_PRODUCTO";
        public const string VentasMetodoPago = "VENTAS_METODO_PAGO";
        public const string CierresCaja = "CIERRES_CAJA";
        public const string DiferenciasCaja = "DIFERENCIAS_CAJA";
        public const string InventarioGeneral = "INVENTARIO_GENERAL";
        public const string StockBajo = "STOCK_BAJO";
        public const string KardexVentas = "KARDEX_VENTAS";
        public const string ProductosVendidos = "PRODUCTOS_VENDIDOS";
    }

    //Reportes Avanzados
    public static class TiposReporteAvanzado
    {
        // 1. Reportes adicionales de ventas
        public const string ProductosMasVendidos = "PRODUCTOS_MAS_VENDIDOS";
        public const string ProductosSinMovimiento = "PRODUCTOS_SIN_MOVIMIENTO";
        public const string VentasAnuladas = "VENTAS_ANULADAS";
        public const string VentasPorHora = "VENTAS_POR_HORA";
        public const string TicketPromedio = "TICKET_PROMEDIO";

        // 2. Reportes adicionales de inventario
        public const string InventarioValorizado = "INVENTARIO_VALORIZADO";
        public const string ProductosSinStock = "PRODUCTOS_SIN_STOCK";
        public const string ProductosStockNegativo = "PRODUCTOS_STOCK_NEGATIVO";
        public const string RotacionInventario = "ROTACION_INVENTARIO";

        // 3. Reportes de caja y control
        public const string ComparativoVentasCierre = "COMPARATIVO_VENTAS_CIERRE";
        public const string CierresPendientes = "CIERRES_PENDIENTES";
        public const string CierresPorEmpleado = "CIERRES_POR_EMPLEADO";
        public const string GastosRegistradosCierre = "GASTOS_REGISTRADOS_CIERRE";

        // 4. Reportes de utilidad
        public const string UtilidadPorProducto = "UTILIDAD_POR_PRODUCTO";
        public const string UtilidadPorVendedor = "UTILIDAD_POR_VENDEDOR";
        public const string UtilidadPorPdv = "UTILIDAD_POR_PDV";
        public const string UtilidadPorDia = "UTILIDAD_POR_DIA";
    }
}
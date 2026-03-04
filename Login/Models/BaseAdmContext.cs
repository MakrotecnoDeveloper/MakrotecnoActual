using DocumentFormat.OpenXml.Bibliography;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;

namespace Plataforma.Models;

public partial class BaseAdmContext : DbContext
{
    public BaseAdmContext(DbContextOptions<BaseAdmContext> options)
        : base(options)
    {
    }

    public DbSet<Empleados> Empleado { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Factura> Factura { get; set; }
    public DbSet<Pedidos> Pedidos { get; set; }
    public DbSet<Clientes> Clientes { get; set; }
    public DbSet<Ventas> Ventas { get; set; }
    public DbSet<Ganancias> Ganancias { get; set; }
    public DbSet<TipoCargo> TipoCargo { get; set; }
    public DbSet<Empresas> Empresas { get; set; }
    public DbSet<Sede> Sede { get; set; }
    public DbSet<EmpleadoEmpresa> EmpleadoEmpresa { get; set; }
    public DbSet<Sedeempleado> Sedeempleado { get; set; }
    public DbSet<LogsLogin> LogsLogin { get; set; }
    public DbSet<Plataformasuscripcion> Plataformasuscripcion { get; set; }
    public DbSet<ClientesPlataforma> ClientesPlataforma { get; set; }
    public DbSet<Infopdv> Infopdv { get; set; }
    public DbSet<Syncpdv> Syncpdv { get; set; }
    public DbSet<Plataformas> Plataformas { get; set; }
    public DbSet<Proveedores> Proveedores { get; set; }
    public DbSet<Compras> Compras { get; set; }
    public DbSet<DetalleCompra> DetalleCompras { get; set; }
    public DbSet<Servicio> Servicio { get; set; }
    public DbSet<CategoriaProductos> CategoriaProductos { get; set; }
    public DbSet<MenuOpciones> MenuOpciones { get; set; }
    public DbSet<Dispositivos> Dispositivos { get; set; }
    public DbSet<OrdenServicios> OrdenServicios { get; set; }
    public DbSet<TipoDispositivos> TipoDispositivos { get; set; }
    public DbSet<HistOrdSer> HistOrdServ { get; set; }
    public DbSet<GestionRealizada> GestionRealizadas { get; set; }
    public DbSet<CierreCaja> CierreCajas { get; set; }
    public DbSet<VistaGananciaDiaria> VistaGananciaDiarias { get; set; }
    public DbSet<AdicionFactura> AdicionFacturas { get; set; }
    public DbSet<MetodoPagos> MetodoPagos { get; set; }
    public DbSet<GastosMensuales> GastosMensuales { get; set; }
    public DbSet<InventarioSede> InventarioSedes => Set<InventarioSede>();
    public DbSet<Contratos> Contratos { get; set; }
    public DbSet<ConceptoNomina> ConceptosNomina { get; set; }
    public DbSet<DetalleConceptosEmpleado> DetallesConceptosEmpleado { get; set; }
    public DbSet<LiquidacionNomina> LiquidacionesNomina { get; set; }
    public DbSet<DetalleLiquidacion> DetallesLiquidacion { get; set; }
    public DbSet<CxcVentas> CxcVentas { get; set; }
    public DbSet<CxcPagos> CxcPagos { get; set; }
    public DbSet<Agendamientos> Agendamientos { get; set; }
    public DbSet<Planes> Planes { get; set; }
    public DbSet<ActividadesEconomicas> ActividadesEconomicas { get; set; }
    public DbSet<LicenciasEmpresa> LicenciasEmpresa { get; set; }
    public DbSet<Modulos> Modulos { get; set; }
    public DbSet<PlanesModulos> PlanesModulos { get; set; }
    public DbSet<ModulosActividadEconomica> ModulosActividadEconomica { get; set; }
    public DbSet<CargoModuloPermiso> CargoModuloPermiso { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Llaves Primarias
        modelBuilder.Entity<Empleados>().HasKey(e => e.Cedula);
        modelBuilder.Entity<Producto>().HasKey(e => e.Cod_Producto);
        modelBuilder.Entity<Factura>().HasKey(e => e.IdFactura);
        modelBuilder.Entity<Clientes>().HasKey(c => c.IdCliente);
        modelBuilder.Entity<Pedidos>().HasKey(cp => cp.IdPedido);
        modelBuilder.Entity<Ventas>().HasKey(cv => cv.IdVenta);
        modelBuilder.Entity<Ganancias>().HasKey(id => id.IdGanancia);
        modelBuilder.Entity<TipoCargo>().HasKey(it => it.Id_tipo);
        modelBuilder.Entity<Empresas>().HasKey(ie => ie.Id_empresa);
        modelBuilder.Entity<Sede>().HasKey(sede => sede.Id_sede);
        modelBuilder.Entity<EmpleadoEmpresa>().HasKey(ie => ie.Id_empleadoE);
        modelBuilder.Entity<Sedeempleado>().HasKey(ise => ise.Id_sedeEmpleado);
        modelBuilder.Entity<LogsLogin>().HasKey(ise => ise.Id_log);
        modelBuilder.Entity<Plataformasuscripcion>().HasKey(ip => ip.IdPltfSuscripcion);
        modelBuilder.Entity<ClientesPlataforma>().HasKey(icp => icp.IdCliPltf);
        modelBuilder.Entity<Infopdv>().HasKey(iv => iv.InfopdvId);
        modelBuilder.Entity<Syncpdv>().HasKey(ds => ds.Idsync);
        modelBuilder.Entity<Plataformas>().HasKey(iptlf => iptlf.IdPlataforma);
        modelBuilder.Entity<Proveedores>().HasKey(prvd => prvd.IdProveedor);
        modelBuilder.Entity<Compras>().HasKey(prvd => prvd.IdCompra);
        modelBuilder.Entity<DetalleCompra>().HasKey(iddc => iddc.IdDetalleCompra);
        modelBuilder.Entity<Servicio>().HasKey(idser => idser.IdServicio);
        modelBuilder.Entity<CategoriaProductos>().HasKey(idcapro => idcapro.IdCateProducto);
        modelBuilder.Entity<MenuOpciones>().HasKey(idmo => idmo.IdOpcion);
        modelBuilder.Entity<Dispositivos>().HasKey(id => id.IdDispositivo);
        modelBuilder.Entity<OrdenServicios>().HasKey(io => io.IdOrden);
        modelBuilder.Entity<TipoDispositivos>().HasKey(iss => iss.TipoDispositivo);
        modelBuilder.Entity<HistOrdSer>().HasKey(idp => idp.IdDiagProb);
        modelBuilder.Entity<GestionRealizada>().HasKey(igr => igr.IdGR);
        modelBuilder.Entity<CierreCaja>().HasKey(id => id.IdFlujoCaja);
        modelBuilder.Entity<AdicionFactura>().HasKey(id => id.IdAdicion);
        modelBuilder.Entity<MetodoPagos>().HasKey(id => id.IdMetodo);
        modelBuilder.Entity<GastosMensuales>().HasKey(id => id.IdGasto);
        modelBuilder.Entity<InventarioSede>().HasKey(id => id.IdInventario);
        modelBuilder.Entity<Agendamientos>().HasKey(idag => idag.Id);
        modelBuilder.Entity<Planes>().HasKey(pl => pl.IdPlan);
        modelBuilder.Entity<ActividadesEconomicas>().HasKey(ac => ac.IdActividad);
        modelBuilder.Entity<LicenciasEmpresa>().HasKey(le => le.IdLicencia);
        modelBuilder.Entity<Modulos>().HasKey(mdl => mdl.IdModulo);
        modelBuilder.Entity<CargoModuloPermiso>().HasKey(id => id.Id);
        //Llaves foraneas
        modelBuilder.Entity<Factura>().HasOne(f => f.Venta).WithMany().HasForeignKey(f => f.IdVenta);
        modelBuilder.Entity<Ventas>().HasOne<Empleados>().WithMany().HasForeignKey(f => f.Cedula);
        modelBuilder.Entity<Ventas>().HasOne<Clientes>().WithMany().HasForeignKey(f => f.IdCliente);
        modelBuilder.Entity<Pedidos>().HasOne(p => p.Venta).WithMany(v => v.Pedidos).HasForeignKey(p => p.IdVenta);
        modelBuilder.Entity<Pedidos>().HasOne<Producto>().WithMany().HasForeignKey(f => f.Codigo);
        modelBuilder.Entity<Pedidos>().HasOne<Infopdv>().WithMany().HasForeignKey(f => f.InfopdvId);
        modelBuilder.Entity<TipoCargo>().HasOne<Empresas>().WithMany().HasForeignKey(f => f.Id_empresa);
        modelBuilder.Entity<Sede>().HasOne<Empresas>().WithMany().HasForeignKey(f => f.Id_empresa);
        modelBuilder.Entity<EmpleadoEmpresa>().HasOne<Empleados>().WithMany().HasForeignKey(f => f.Cedula);
        modelBuilder.Entity<EmpleadoEmpresa>().HasOne<Empresas>().WithMany().HasForeignKey(f => f.Id_empresa);
        modelBuilder.Entity<Sedeempleado>().HasOne<Sede>().WithMany().HasForeignKey(f => f.Id_sede);
        modelBuilder.Entity<Sedeempleado>().HasOne<Empleados>().WithMany().HasForeignKey(f => f.Cedula);
        modelBuilder.Entity<Sedeempleado>().HasOne<TipoCargo>().WithMany().HasForeignKey(f => f.Id_cargo);
        modelBuilder.Entity<Plataformasuscripcion>().HasOne<Empleados>().WithMany().HasForeignKey(f => f.CedulaEmpleado);
        modelBuilder.Entity<Plataformasuscripcion>().HasOne<Plataformas>().WithMany().HasForeignKey(f => f.IdPlataforma);
        modelBuilder.Entity<LogsLogin>().HasOne<Infopdv>().WithMany().HasForeignKey(f => f.InfopdvId);
        modelBuilder.Entity<ClientesPlataforma>().HasOne<Plataformasuscripcion>().WithMany().HasForeignKey(f => f.IdPltfSuscripcion);
        modelBuilder.Entity<Infopdv>().HasOne<Sede>().WithMany().HasForeignKey(f => f.Id_Sede);
        modelBuilder.Entity<Syncpdv>().HasOne<Infopdv>().WithMany().HasForeignKey(f => f.InfopdvId);
        modelBuilder.Entity<Syncpdv>().HasOne<Empleados>().WithMany().HasForeignKey(f => f.Cedula);
        //modelBuilder.Entity<GananciaPedido>().HasOne<Pedidos>().WithMany().HasForeignKey(f => f.Cod_pedido);
        modelBuilder.Entity<DetalleCompra>().HasOne(d => d.Compra).WithMany(c => c.Detalles).HasForeignKey(d => d.IdCompra).HasConstraintName("FK_DetalleCompra_Compras");
        //modelBuilder.Entity<DetalleCompra>().HasOne<Producto>().WithMany().HasForeignKey(f => f.CodProducto);
        modelBuilder.Entity<Compras>().HasOne<Proveedores>().WithMany().HasForeignKey(f => f.IdProveedor);
        modelBuilder.Entity<CategoriaProductos>().HasOne<Servicio>().WithMany().HasForeignKey(f => f.IdServicio);
        modelBuilder.Entity<Producto>().HasOne<CategoriaProductos>().WithMany().HasForeignKey(f => f.IdCatepro);
        modelBuilder.Entity<Producto>().HasOne<Proveedores>().WithMany().HasForeignKey(f => f.idProveedor);
        modelBuilder.Entity<Dispositivos>().HasOne(d => d.Cliente).WithMany().HasForeignKey(d => d.IdCliente).HasPrincipalKey(c => c.IdCliente);
        modelBuilder.Entity<OrdenServicios>().HasOne(d => d.Dispositivo).WithMany().HasForeignKey(d => d.IdDispositivo).HasPrincipalKey(c => c.IdDispositivo);
        modelBuilder.Entity<OrdenServicios>().HasOne<Empleados>().WithMany().HasForeignKey(f => f.Cedula);
        modelBuilder.Entity<Dispositivos>().HasOne<TipoDispositivos>().WithMany().HasForeignKey(f => f.TipoDispositivo);
        modelBuilder.Entity<HistOrdSer>().HasOne<OrdenServicios>().WithMany().HasForeignKey(f => f.IdOrden);
        modelBuilder.Entity<HistOrdSer>().HasOne<Producto>().WithMany().HasForeignKey(f => f.Cod_Producto);
        modelBuilder.Entity<HistOrdSer>().HasOne<Empleados>().WithMany().HasForeignKey(f => f.Cedula);
        modelBuilder.Entity<GestionRealizada>().HasOne<OrdenServicios>().WithMany().HasForeignKey(f => f.IdOrden);
        modelBuilder.Entity<CierreCaja>().HasOne<Empleados>().WithMany().HasForeignKey(f => f.Cedula);
        modelBuilder.Entity<Ganancias>().HasOne<Empleados>().WithMany().HasForeignKey(f => f.Cedula);
        modelBuilder.Entity<AdicionFactura>().HasOne(a => a.Factura).WithMany(f => f.Adiciones).HasForeignKey(a => a.IdFactura).HasConstraintName("FK_AdicionFactura_Factura");
        modelBuilder.Entity<Agendamientos>().HasOne<Producto>().WithMany().HasForeignKey(f => f.Cod_Producto);
        modelBuilder.Entity<CargoModuloPermiso>().HasOne<Empresas>().WithMany().HasForeignKey(f => f.EmpresaId);
        modelBuilder.Entity<CargoModuloPermiso>().HasOne<TipoCargo>().WithMany().HasForeignKey(f => f.CargoId);
        modelBuilder.Entity<CargoModuloPermiso>().HasOne<Modulos>().WithMany().HasForeignKey(f => f.ModuloId);


        modelBuilder.Entity<GestionRealizada>()
            .Property(g => g.CostoTotal)
            .HasPrecision(10, 2);

        modelBuilder.Entity<OrdenServicios>()
            .HasOne<Empleados>()
            .WithMany()
            .HasForeignKey(o => o.Cedula)
            .IsRequired(false); // ✅ permite NULL


        modelBuilder.Entity<Pedidos>()
            .Property(p => p.Stock)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Producto>()
            .Property(p => p.CantidadProducto)
            .HasPrecision(10, 2);

        modelBuilder.Entity<VistaGananciaDiaria>()
            .HasNoKey()
            .ToView("VistaGananciaDiaria");

        modelBuilder.Entity<OrdenServicios>()
        .ToTable(t =>
        {
            t.HasCheckConstraint(
                "CK_OrdenServicios_Estado",
                "Estado IN ('Ingresada','Ejecucion','Pendiente','Finalizada','Rechazada')"
            );
        });
        modelBuilder.Entity<GastosMensuales>(eb =>
        {
            // Nombre tal cual en SQL Server
            eb.ToTable(tb => tb.HasTrigger("TR_GastosMensuales_SetUpdated"));
        });

        // Ejemplo: relaciones
        modelBuilder.Entity<Contratos>()
            .HasOne(c => c.Empleado)
            .WithMany(e => e.Contratos)
            .HasForeignKey(c => c.Cedula);

        modelBuilder.Entity<DetalleLiquidacion>()
            .HasOne(dl => dl.Liquidacion)
            .WithMany(l => l.Detalles)
            .HasForeignKey(dl => dl.IdLiquidacion);

        modelBuilder.Entity<LiquidacionNomina>()
            .HasOne(c => c.Empleado)
            .WithMany(e => e.Liquidaciones)
            .HasForeignKey(c => c.Cedula);

        //Licenciamiento y Modulos
        // Tabla puente PlanModulo
        modelBuilder.Entity<PlanesModulos>()
            .HasKey(pm => new { pm.IdPlan, pm.IdModulo });

        modelBuilder.Entity<PlanesModulos>()
            .HasOne(pm => pm.Plan)
            .WithMany(p => p.PlanesModulos)
            .HasForeignKey(pm => pm.IdPlan);

        modelBuilder.Entity<PlanesModulos>()
            .HasOne(pm => pm.Modulo)
            .WithMany(m => m.PlanesModulos)
            .HasForeignKey(pm => pm.IdModulo);

        // Tabla puente ModuloActividadEconomica
        modelBuilder.Entity<ModulosActividadEconomica>()
            .HasKey(ma => new { ma.IdActividad, ma.IdModulo });

        modelBuilder.Entity<ModulosActividadEconomica>()
            .HasOne(ma => ma.ActividadEconomica)
            .WithMany(a => a.ModulosActividad)
            .HasForeignKey(ma => ma.IdActividad);

        modelBuilder.Entity<ModulosActividadEconomica>()
            .HasOne(ma => ma.Modulo)
            .WithMany(m => m.ModulosActividad)
            .HasForeignKey(ma => ma.IdModulo);

        // Relación Empresa → Actividad (opcional)
        modelBuilder.Entity<Empresas>()
            .HasOne(e => e.ActividadEconomica)
            .WithMany(a => a.Empresas)
            .HasForeignKey(e => e.ActividadEconomicaId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relación Licencia → Empresa / Plan
        modelBuilder.Entity<LicenciasEmpresa>()
            .HasOne(l => l.Empresa)
            .WithMany(e => e.Licencias)
            .HasForeignKey(l => l.IdEmpresa);

        modelBuilder.Entity<LicenciasEmpresa>()
            .HasOne(l => l.Plan)
            .WithMany(p => p.Licencias)
            .HasForeignKey(l => l.IdPlan);

        modelBuilder.Entity<Modulos>()
           .HasMany(m => m.MenuOpciones)
           .WithOne(o => o.Modulo)
           .HasForeignKey(o => o.IdModulo);

        foreach (var property in modelBuilder.Model
        .GetEntityTypes()
        .SelectMany(t => t.GetProperties())
        .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetPrecision(10);
            property.SetScale(2);
        }
    }
}

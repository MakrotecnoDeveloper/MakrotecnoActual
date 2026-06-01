using MakroTecno.Models;
using Microsoft.EntityFrameworkCore;
using Plataforma.Data.Configurations;

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
    public DbSet<Contrato> Contratos => Set<Contrato>();
    public DbSet<ConceptoNomina> ConceptosNomina => Set<ConceptoNomina>();
    public DbSet<DetalleConceptoEmpleado> DetalleConceptosEmpleado => Set<DetalleConceptoEmpleado>();
    public DbSet<PeriodoNomina> PeriodosNomina => Set<PeriodoNomina>();
    public DbSet<NovedadNomina> NovedadesNomina => Set<NovedadNomina>();
    public DbSet<LiquidacionNomina> LiquidacionesNomina => Set<LiquidacionNomina>();
    public DbSet<DetalleLiquidacion> DetalleLiquidacion => Set<DetalleLiquidacion>();
    public DbSet<UnidadMedida> UnidadesMedida { get; set; }
    public DbSet<Area> Areas { get; set; }
    public DbSet<ModulosEmpresa> ModulosEmpresa { get; set; }
    public DbSet<ClienteStreaming> ClientesStreaming { get; set; }
    public DbSet<FacturaCompra> FacturaCompra { get; set; }
    /*Integraciones Rappi Inicio*/
    public DbSet<RappiTienda> RappiTienda { get; set; }
    public DbSet<RappiCategoriaMapeo> RappiCategoriaMapeo { get; set; }
    public DbSet<RappiProductoConfig> RappiProductoConfig { get; set; }
    public DbSet<RappiSincronizacion> RappiSincronizacion { get; set; }
    public DbSet<RappiSincronizacionDetalle> RappiSincronizacionDetalle { get; set; }
    /*Integraciones Rappi Fin*/

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
        modelBuilder.Entity<Area>().HasKey(id => id.IdArea);
        modelBuilder.Entity<ClienteStreaming>().HasKey(id => id.IdClienteStreaming);
        modelBuilder.Entity<FacturaCompra>().HasKey(id => id.IdFacturaCompra);
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
        modelBuilder.Entity<ClientesPlataforma>().HasOne(cp => cp.PlataformaSuscripcion).WithMany().HasForeignKey(f => f.IdPltfSuscripcion).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ClientesPlataforma>().HasOne(cp => cp.ClienteStreaming).WithMany(c => c.PlataformasCliente).HasForeignKey(cp => cp.IdClienteStreaming).OnDelete(DeleteBehavior.Restrict);
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
        modelBuilder.Entity<TipoCargo>().HasOne(tc => tc.Area).WithMany(a => a.TiposCargo).HasForeignKey(tc => tc.IdArea).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Pedidos>().HasOne(p => p.Producto).WithMany().HasForeignKey(p => p.Codigo).HasPrincipalKey(pr => pr.Cod_Producto);
        modelBuilder.Entity<ClientesPlataforma>().HasOne(cp => cp.ClienteStreaming).WithMany(c => c.PlataformasCliente).HasForeignKey(cp => cp.IdClienteStreaming);
        /*Integraciones Rappi Inicio*/
        modelBuilder.Entity<RappiProductoConfig>().HasOne(x => x.Producto).WithMany().HasForeignKey(x => x.ProductoId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<RappiCategoriaMapeo>().HasOne(x => x.CategoriaProducto).WithMany().HasForeignKey(x => x.IdCateProducto).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<RappiTienda>().HasOne(x => x.Sede).WithMany().HasForeignKey(x => x.SedeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<RappiSincronizacionDetalle>().HasOne(x => x.RappiSincronizacion).WithMany(x => x.Detalles).HasForeignKey(x => x.RappiSincronizacionId).OnDelete(DeleteBehavior.Cascade);
        /*Integraciones Rappi Fin*/

        /*Configuracion para Nomina*/
        modelBuilder.ApplyConfiguration(new ContratoConfiguration());
        modelBuilder.ApplyConfiguration(new ConceptoNominaConfiguration());
        modelBuilder.ApplyConfiguration(new DetalleConceptoEmpleadoConfiguration());
        modelBuilder.ApplyConfiguration(new PeriodoNominaConfiguration());
        modelBuilder.ApplyConfiguration(new NovedadNominaConfiguration());
        modelBuilder.ApplyConfiguration(new LiquidacionNominaConfiguration());
        modelBuilder.ApplyConfiguration(new DetalleLiquidacionConfiguration());

        /*Factura*/
        modelBuilder.Entity<Factura>(entity =>
        {
            entity.ToTable("Factura");

            entity.HasKey(e => e.IdFactura);

            entity.Property(e => e.NumeroFactura)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.SubTotal)
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.IVA)
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.Total)
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.EstadoFactura)
                .IsRequired()
                .HasMaxLength(30)
                .HasDefaultValue("Generada");

            entity.Property(e => e.EsElectronica)
                .HasDefaultValue(false);

            entity.Property(e => e.EstadoDian)
                .IsRequired()
                .HasMaxLength(30)
                .HasDefaultValue("NoAplica");

            entity.Property(e => e.Cufe)
                .HasMaxLength(150);

            entity.Property(e => e.TrackId)
                .HasMaxLength(150);

            entity.HasOne(e => e.Venta)
                .WithOne()
                .HasForeignKey<Factura>(e => e.IdVenta)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.IdVenta)
                .IsUnique();

            entity.HasIndex(e => e.NumeroFactura)
                .IsUnique();

            entity.HasIndex(e => e.EstadoDian);
        });

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

        modelBuilder.Entity<Producto>()
        .HasOne(p => p.Unidad)
        .WithMany(u => u.Productos)
        .HasForeignKey(p => p.IdUnidad)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CierreCaja>(entity =>
        {
            entity.ToTable("CierreCaja"); // cambia si en SQL se llama distinto

            entity.HasKey(e => e.IdFlujoCaja);

            entity.Property(e => e.Monto)
                .HasColumnType("decimal(10,2)");

            entity.Property(e => e.Efectivo)
                .HasColumnType("decimal(10,2)");

            entity.Property(e => e.Transferencia)
                .HasColumnType("decimal(10,2)");

            entity.Property(e => e.GastosEfectivo)
                .HasColumnType("decimal(10,2)");

            entity.Property(e => e.GastosTransferencia)
                .HasColumnType("decimal(10,2)");

            entity.Property(e => e.Diferencia)
                .HasColumnType("decimal(10,2)");

            entity.Property(e => e.IdEmpresa)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.NombreRol)
                .HasMaxLength(150);

            entity.Property(e => e.NombreEmpresa)
                .HasMaxLength(200);

            entity.Property(e => e.NombreSede)
                .HasMaxLength(200);

            entity.Property(e => e.NombrePdv)
                .HasMaxLength(200);

            entity.HasIndex(e => new { e.Fecha, e.Cedula, e.InfopdvId })
                .HasDatabaseName("IX_CierreCajas_Fecha_Cedula_InfopdvId");

            entity.HasIndex(e => e.IdEmpresa)
                .HasDatabaseName("IX_CierreCajas_IdEmpresa");

            entity.HasIndex(e => e.IdSede)
                .HasDatabaseName("IX_CierreCajas_IdSede");

            entity.HasIndex(e => e.InfopdvId)
                .HasDatabaseName("IX_CierreCajas_InfopdvId");

            entity.HasOne(e => e.Empleado)
                .WithMany()
                .HasForeignKey(e => e.Cedula)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Empresa)
                .WithMany()
                .HasForeignKey(e => e.IdEmpresa)
                .HasPrincipalKey(e => e.Id_empresa)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Sede)
                .WithMany()
                .HasForeignKey(e => e.IdSede)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Pdv)
                .WithMany()
                .HasForeignKey(e => e.InfopdvId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CategoriaProductos>()
        .HasOne(c => c.Servicio)
        .WithMany(s => s.CategoriaProductos)
        .HasForeignKey(c => c.IdServicio);

        foreach (var property in modelBuilder.Model
        .GetEntityTypes()
        .SelectMany(t => t.GetProperties())
        .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetPrecision(10);
            property.SetScale(2);
        }

        /*Evitar que mas de un proveedor tengan la misma factura*/
         modelBuilder.Entity<FacturaCompra>()
        .HasOne(f => f.Proveedor)
        .WithMany(p => p.FacturasCompra)
        .HasForeignKey(f => f.IdProveedor)
        .OnDelete(DeleteBehavior.Restrict);
    }
}

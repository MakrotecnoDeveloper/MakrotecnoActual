using Microsoft.EntityFrameworkCore;
using MK_ERP.Models;

namespace MK_ERP.Data;

public partial class PanMel1Context : DbContext
{
    public PanMel1Context()
    {
    }

    public PanMel1Context(DbContextOptions<PanMel1Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Categoriaproducto> Categoriaproductos { get; set; }

    public virtual DbSet<CierreCaja> CierreCajas { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Clientesplataforma> Clientesplataformas { get; set; }

    public virtual DbSet<Compra> Compras { get; set; }

    public virtual DbSet<Detallecompra> Detallecompras { get; set; }

    public virtual DbSet<Diagnosticoproblema> Diagnosticoproblemas { get; set; }

    public virtual DbSet<Dispositivo> Dispositivos { get; set; }

    public virtual DbSet<Empleado> Empleados { get; set; }

    public virtual DbSet<Empleadoempresa> Empleadoempresas { get; set; }

    public virtual DbSet<Empresa> Empresas { get; set; }

    public virtual DbSet<Factura> Facturas { get; set; }

    public virtual DbSet<FlujoCaja> FlujoCajas { get; set; }

    public virtual DbSet<Ganancia> Ganancias { get; set; }

    public virtual DbSet<Gestionrealizadum> Gestionrealizada { get; set; }

    public virtual DbSet<Infopdv> Infopdvs { get; set; }

    public virtual DbSet<Logslogin> Logslogins { get; set; }

    public virtual DbSet<MkMenu> MkMenus { get; set; }

    public virtual DbSet<MkPermisosMenu> MkPermisosMenus { get; set; }

    public virtual DbSet<Ordenservicio> Ordenservicios { get; set; }

    public virtual DbSet<Pedido> Pedidos { get; set; }

    public virtual DbSet<PlataformaStrm> Plataformas { get; set; }

    public virtual DbSet<Plataformasuscripcion> Plataformasuscripcions { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Proveedore> Proveedores { get; set; }

    public virtual DbSet<Sede> Sedes { get; set; }

    public virtual DbSet<Sedeempleado> Sedeempleados { get; set; }

    public virtual DbSet<Seguimientoservicio> Seguimientoservicios { get; set; }

    public virtual DbSet<Servicio> Servicios { get; set; }

    public virtual DbSet<Syncpdv> Syncpdvs { get; set; }

    public virtual DbSet<Tipocargo> Tipocargos { get; set; }

    public virtual DbSet<Venta> Ventas { get; set; }

    public virtual DbSet<VistaGananciaDiarium> VistaGananciaDiaria { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=DEVELOPERIST\\DEVELOPERIST;User =DeveloperIST ;Database=PAN_MEL1; TrustServerCertificate=True; Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoriaproducto>(entity =>
        {
            entity.HasKey(e => e.IdCateProducto).HasName("PK_categoriaproductos_IdCateProducto");

            entity.ToTable("categoriaproductos");

            entity.HasIndex(e => e.Idservicio, "fk_idservicio");

            entity.Property(e => e.Descripcion).HasMaxLength(100);
            entity.Property(e => e.Idservicio).HasDefaultValueSql("(NULL)");

            entity.HasOne(d => d.IdservicioNavigation).WithMany(p => p.Categoriaproductos)
                .HasForeignKey(d => d.Idservicio)
                .HasConstraintName("FK_CategoriaProductos_Servicio");
        });

        modelBuilder.Entity<CierreCaja>(entity =>
        {
            entity.HasKey(e => e.IdCierreCaja);

            entity.ToTable("CierreCaja");

            entity.Property(e => e.IdCierreCaja).ValueGeneratedNever();
            entity.Property(e => e.Diferencia).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.EfectivoReal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.TotalVentas).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.CedulaNavigation).WithMany(p => p.CierreCajas)
                .HasForeignKey(d => d.Cedula)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CierreCaja_Empleado");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.CedulaCliente).HasName("PK_cliente_cedulaCliente");

            entity.ToTable("cliente");

            entity.Property(e => e.CedulaCliente)
                .ValueGeneratedNever()
                .HasColumnName("cedulaCliente");
            entity.Property(e => e.CiudadCliente)
                .HasMaxLength(30)
                .HasColumnName("ciudadCliente");
            entity.Property(e => e.EmpresaCliente)
                .HasMaxLength(50)
                .HasColumnName("empresaCliente");
            entity.Property(e => e.NombreCliente)
                .HasMaxLength(50)
                .HasColumnName("nombreCliente");
            entity.Property(e => e.TelefonoCliente)
                .HasMaxLength(30)
                .HasColumnName("telefonoCliente");
        });

        modelBuilder.Entity<Clientesplataforma>(entity =>
        {
            entity.HasKey(e => e.IdCliPltf).HasName("PK_clientesplataforma_idCliPltf");

            entity.ToTable("clientesplataforma");

            entity.HasIndex(e => e.CedulaEmpleado, "cedulaEmpleado");

            entity.HasIndex(e => e.IdPltfSuscripcion, "idPlataforma");

            entity.Property(e => e.IdCliPltf).HasColumnName("idCliPltf");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.CedulaEmpleado).HasColumnName("cedulaEmpleado");
            entity.Property(e => e.CelularCliente)
                .HasMaxLength(100)
                .HasColumnName("celularCliente");
            entity.Property(e => e.Clave)
                .HasMaxLength(100)
                .HasColumnName("clave");
            entity.Property(e => e.ClavePerfil)
                .HasMaxLength(100)
                .HasColumnName("clavePerfil");
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .HasColumnName("correo");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.FechaFinPago)
                .HasPrecision(0)
                .HasColumnName("fechaFinPago");
            entity.Property(e => e.FechaIniPago)
                .HasPrecision(0)
                .HasColumnName("fechaIniPago");
            entity.Property(e => e.IdPltfSuscripcion)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("idPltfSuscripcion");
            entity.Property(e => e.NombreCliente)
                .HasMaxLength(100)
                .HasColumnName("nombreCliente");
            entity.Property(e => e.Ppm)
                .HasMaxLength(100)
                .HasColumnName("ppm");
            entity.Property(e => e.ValorNeto).HasColumnName("valorNeto");
            entity.Property(e => e.ValorVenta).HasColumnName("valorVenta");

            entity.HasOne(d => d.CedulaEmpleadoNavigation).WithMany(p => p.Clientesplataformas)
                .HasForeignKey(d => d.CedulaEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClientesPlataforma_Empleado");

            entity.HasOne(d => d.IdPltfSuscripcionNavigation).WithMany(p => p.Clientesplataformas)
                .HasForeignKey(d => d.IdPltfSuscripcion)
                .HasConstraintName("FK_ClientesPlataforma_PlataformaSuscripcion");
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.HasKey(e => e.IdCompra).HasName("PK_historicocompras_idHC");

            entity.ToTable("compras");

            entity.HasIndex(e => e.CodFacturaExterno, "fk_codfactura");

            entity.Property(e => e.CodFacturaExterno)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)");
            entity.Property(e => e.Estado).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.FechaCompra)
                .HasPrecision(0)
                .HasDefaultValueSql("(NULL)");
            entity.Property(e => e.ValorTotal)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.Compras)
                .HasForeignKey(d => d.IdProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Compras_Proveedores");
        });

        modelBuilder.Entity<Detallecompra>(entity =>
        {
            entity.HasKey(e => e.IdDetalleCompra);

            entity.ToTable("detallecompra");

            entity.Property(e => e.Cantidad).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CodProducto).HasMaxLength(255);
            entity.Property(e => e.ValorNeto).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ValorTotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ValorVenta).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.CodProductoNavigation).WithMany(p => p.Detallecompras)
                .HasForeignKey(d => d.CodProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleCompra_Productos");

            entity.HasOne(d => d.IdCompraNavigation).WithMany(p => p.Detallecompras)
                .HasForeignKey(d => d.IdCompra)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleCompra_Compra");
        });

        modelBuilder.Entity<Diagnosticoproblema>(entity =>
        {
            entity.HasKey(e => e.IdDiagProb).HasName("PK__diagnost__E22B446BDB62D92E");

            entity.ToTable("diagnosticoproblema");

            entity.Property(e => e.Cedula).HasColumnName("cedula");
            entity.Property(e => e.Costo).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CostoInterno).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Estado).HasMaxLength(50);
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.ProblemaDetectado).HasMaxLength(255);
            entity.Property(e => e.Solucion)
                .HasMaxLength(255)
                .IsFixedLength();

            entity.HasOne(d => d.CedulaNavigation).WithMany(p => p.Diagnosticoproblemas)
                .HasForeignKey(d => d.Cedula)
                .HasConstraintName("EmpleadoFK");

            entity.HasOne(d => d.IdOrdenNavigation).WithMany(p => p.Diagnosticoproblemas)
                .HasForeignKey(d => d.IdOrden)
                .HasConstraintName("OrdenFK");
        });

        modelBuilder.Entity<Dispositivo>(entity =>
        {
            entity.HasKey(e => e.IdDispositivo).HasName("PK__disposit__B1EDB8E458D95D32");

            entity.ToTable("dispositivo");

            entity.Property(e => e.CedulaCliente).HasColumnName("cedulaCliente");
            entity.Property(e => e.Imei)
                .HasMaxLength(100)
                .HasColumnName("IMEI");
            entity.Property(e => e.Marca).HasMaxLength(50);
            entity.Property(e => e.Modelo).HasMaxLength(50);

            entity.HasOne(d => d.CedulaClienteNavigation).WithMany(p => p.Dispositivos)
                .HasForeignKey(d => d.CedulaCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("CedulaFK");
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.Cedula).HasName("PK_empleado_cedula");

            entity.ToTable("empleado");

            entity.Property(e => e.Cedula)
                .ValueGeneratedNever()
                .HasColumnName("cedula");
            entity.Property(e => e.Apellido)
                .HasMaxLength(30)
                .HasColumnName("apellido");
            entity.Property(e => e.Celular)
                .HasMaxLength(30)
                .HasColumnName("celular");
            entity.Property(e => e.Contrasena)
                .HasMaxLength(100)
                .HasColumnName("contrasena");
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .HasColumnName("correo");
            entity.Property(e => e.Genero)
                .HasMaxLength(3)
                .HasColumnName("genero");
            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .HasColumnName("nombre");
            entity.Property(e => e.Rh)
                .HasMaxLength(5)
                .HasColumnName("rh");
        });

        modelBuilder.Entity<Empleadoempresa>(entity =>
        {
            entity.HasKey(e => e.IdEmpleadoE).HasName("PK_empleadoempresa_id_empleadoE");

            entity.ToTable("empleadoempresa");

            entity.HasIndex(e => e.Cedula, "cedula");

            entity.HasIndex(e => e.IdEmpresa, "id_empresa");

            entity.Property(e => e.IdEmpleadoE).HasColumnName("id_empleadoE");
            entity.Property(e => e.Cedula).HasColumnName("cedula");
            entity.Property(e => e.IdEmpresa)
                .HasMaxLength(50)
                .HasColumnName("id_empresa");

            entity.HasOne(d => d.CedulaNavigation).WithMany(p => p.Empleadoempresas)
                .HasForeignKey(d => d.Cedula)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmpleadoEmpresa_Empleado");

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.Empleadoempresas)
                .HasForeignKey(d => d.IdEmpresa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmpleadoEmpresa_Empresas");
        });

        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.HasKey(e => e.IdEmpresa).HasName("PK_empresas_id_empresa");

            entity.ToTable("empresas");

            entity.Property(e => e.IdEmpresa)
                .HasMaxLength(50)
                .HasColumnName("id_empresa");
            entity.Property(e => e.Direccion)
                .HasMaxLength(100)
                .HasColumnName("direccion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .HasColumnName("nombre");
            entity.Property(e => e.Pais)
                .HasMaxLength(30)
                .HasColumnName("pais");
            entity.Property(e => e.Telefono)
                .HasMaxLength(30)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.HasKey(e => e.IdFactura).HasName("PK_factura_cod_factura");

            entity.ToTable("factura");

            entity.HasIndex(e => e.IdVenta, "cedula");

            entity.HasIndex(e => e.NumeroFactura, "cedula_cliente");

            entity.Property(e => e.EstadoFactura).HasMaxLength(50);
            entity.Property(e => e.Iva)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("IVA");
            entity.Property(e => e.SubTotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdVentaNavigation).WithMany(p => p.Facturas)
                .HasForeignKey(d => d.IdVenta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Factura_Venta");
        });

        modelBuilder.Entity<FlujoCaja>(entity =>
        {
            entity.HasKey(e => e.IdFlujoCaja);

            entity.ToTable("FlujoCaja");

            entity.Property(e => e.Concepto).HasMaxLength(255);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.Monto).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TipoMovimiento).HasMaxLength(50);

            entity.HasOne(d => d.CedulaNavigation).WithMany(p => p.FlujoCajas)
                .HasForeignKey(d => d.Cedula)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FlujoCaja_Empleado");
        });

        modelBuilder.Entity<Ganancia>(entity =>
        {
            entity.HasKey(e => e.IdGanancia).HasName("PK_ganancias_id_ganancias");

            entity.ToTable("ganancias");

            entity.Property(e => e.Costos).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime");
            entity.Property(e => e.Ingresos).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Utilidad).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.CedulaNavigation).WithMany(p => p.Ganancia)
                .HasForeignKey(d => d.Cedula)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ganancias_Empleado");
        });

        modelBuilder.Entity<Gestionrealizadum>(entity =>
        {
            entity.HasKey(e => e.IdGr).HasName("PK__gestionr__B773F21AF35AFECD");

            entity.ToTable("gestionrealizada");

            entity.Property(e => e.IdGr).HasColumnName("IdGR");
            entity.Property(e => e.CostoTotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Estado).HasMaxLength(50);
            entity.Property(e => e.FechaEntrega).HasColumnType("datetime");

            entity.HasOne(d => d.IdOrdenNavigation).WithMany(p => p.Gestionrealizada)
                .HasForeignKey(d => d.IdOrden)
                .HasConstraintName("OrdenGestionFK");
        });

        modelBuilder.Entity<Infopdv>(entity =>
        {
            entity.HasKey(e => e.InfopdvId).HasName("PK_infopdv_InfopdvId");

            entity.ToTable("infopdv");

            entity.HasIndex(e => e.IdSede, "infopdv_sede_FK");

            entity.Property(e => e.IdSede)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("id_sede");
            entity.Property(e => e.NombreInfoPdv)
                .HasMaxLength(255)
                .HasColumnName("NombreInfoPDV");

            entity.HasOne(d => d.IdSedeNavigation).WithMany(p => p.Infopdvs)
                .HasForeignKey(d => d.IdSede)
                .HasConstraintName("FK_InfoPDV_Sedes");
        });

        modelBuilder.Entity<Logslogin>(entity =>
        {
            entity.HasKey(e => e.IdLog).HasName("PK_logslogin_id_log");

            entity.ToTable("logslogin");

            entity.HasIndex(e => e.Cedula, "cedula");

            entity.HasIndex(e => e.InfopdvId, "logslogin_infopdv_FK");

            entity.Property(e => e.IdLog).HasColumnName("id_log");
            entity.Property(e => e.Cedula).HasColumnName("cedula");
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .HasColumnName("correo");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.Fecha)
                .HasPrecision(0)
                .HasColumnName("fecha");

            entity.HasOne(d => d.CedulaNavigation).WithMany(p => p.Logslogins)
                .HasForeignKey(d => d.Cedula)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LogsLogin_Empleado");

            entity.HasOne(d => d.Infopdv).WithMany(p => p.Logslogins)
                .HasForeignKey(d => d.InfopdvId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LogsLogin_InfoPDV");
        });

        modelBuilder.Entity<MkMenu>(entity =>
        {
            entity.ToTable("MkMenu");

            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<MkPermisosMenu>(entity =>
        {
            entity.ToTable("MkPermisosMenu");

            entity.Property(e => e.IdMenu).HasColumnName("Id_Menu");
            entity.Property(e => e.IdTipoCargo).HasColumnName("Id_TipoCargo");

            entity.HasOne(d => d.IdMenuNavigation).WithMany(p => p.MkPermisosMenus)
                .HasForeignKey(d => d.IdMenu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PermisosMenu_MkMenu");

            entity.HasOne(d => d.IdTipoCargoNavigation).WithMany(p => p.MkPermisosMenus)
                .HasForeignKey(d => d.IdTipoCargo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PermisosMenu_TipoCargo");
        });

        modelBuilder.Entity<Ordenservicio>(entity =>
        {
            entity.HasKey(e => e.IdOrden).HasName("PK__ordenser__C38F300D658EB7C0");

            entity.ToTable("ordenservicio");

            entity.Property(e => e.Estado).HasMaxLength(50);
            entity.Property(e => e.FechaIngreso).HasColumnType("datetime");
            entity.Property(e => e.ProblemaReportado).HasMaxLength(255);

            entity.HasOne(d => d.IdDispositivoNavigation).WithMany(p => p.Ordenservicios)
                .HasForeignKey(d => d.IdDispositivo)
                .HasConstraintName("DispositivoFK");
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.IdPedido).HasName("PK_pedidos_cod_pedido");

            entity.ToTable("pedidos");

            entity.HasIndex(e => e.IdVenta, "cod_factura");

            entity.HasIndex(e => e.Codigo, "cod_producto");

            entity.HasIndex(e => e.InfopdvId, "fk_pedidos_infopdv");

            entity.Property(e => e.Codigo).HasMaxLength(255);
            entity.Property(e => e.FechaRegistro)
                .HasPrecision(0)
                .HasDefaultValueSql("(NULL)");
            entity.Property(e => e.Stock)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Vneto).HasColumnName("VNeto");
            entity.Property(e => e.Vventa).HasColumnName("VVenta");

            entity.HasOne(d => d.CodigoNavigation).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.Codigo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pedidos_Productos");

            entity.HasOne(d => d.IdVentaNavigation).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.IdVenta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pedidos_Venta");

            entity.HasOne(d => d.Infopdv).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.InfopdvId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pedidos_InfoPDV");
        });

        modelBuilder.Entity<PlataformaStrm>(entity =>
        {
            entity.HasKey(e => e.IdPlataforma).HasName("PK_plataformas_idPlataforma");

            entity.ToTable("plataformas");

            entity.Property(e => e.IdPlataforma).HasColumnName("idPlataforma");
            entity.Property(e => e.NombrePltf)
                .HasMaxLength(50)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("nombrePltf");
        });

        modelBuilder.Entity<Plataformasuscripcion>(entity =>
        {
            entity.HasKey(e => e.IdPltfSuscripcion).HasName("PK_plataformasuscripcion_idPltfSuscripcion");

            entity.ToTable("plataformasuscripcion");

            entity.HasIndex(e => e.CedulaEmpleado, "cedulaEmpleado");

            entity.HasIndex(e => e.IdPlataforma, "fk_pltf_suscripcion");

            entity.Property(e => e.IdPltfSuscripcion).HasColumnName("idPltfSuscripcion");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.CedulaEmpleado).HasColumnName("cedulaEmpleado");
            entity.Property(e => e.Contrasena)
                .HasMaxLength(100)
                .HasColumnName("contrasena");
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .HasColumnName("correo");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.FechaFinPago)
                .HasPrecision(0)
                .HasColumnName("fechaFinPago");
            entity.Property(e => e.FechaIniPago)
                .HasPrecision(0)
                .HasColumnName("fechaIniPago");
            entity.Property(e => e.IdPlataforma)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("idPlataforma");
            entity.Property(e => e.ValorNeto).HasColumnName("valorNeto");
            entity.Property(e => e.ValorVenta).HasColumnName("valorVenta");

            entity.HasOne(d => d.CedulaEmpleadoNavigation).WithMany(p => p.Plataformasuscripcions)
                .HasForeignKey(d => d.CedulaEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PlataformaSuscripcion_Empleado");

            entity.HasOne(d => d.IdPlataformaNavigation).WithMany(p => p.Plataformasuscripcions)
                .HasForeignKey(d => d.IdPlataforma)
                .HasConstraintName("FK_PlataformaSuscripcion_Plataforma");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.CodProducto).HasName("PK_productos_cod_producto");

            entity.ToTable("productos");

            entity.HasIndex(e => e.IdCatePro, "fk_categoria_producto");

            entity.HasIndex(e => e.IdEmpresa, "id_empresa");

            entity.Property(e => e.CodProducto)
                .HasMaxLength(255)
                .HasColumnName("cod_producto");
            entity.Property(e => e.CantidadProducto)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("cantidadProducto");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.IdCatePro)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("idCatePro");
            entity.Property(e => e.IdEmpresa)
                .HasMaxLength(50)
                .HasColumnName("id_empresa");
            entity.Property(e => e.NombreProducto).HasColumnName("nombreProducto");
            entity.Property(e => e.ValorNetoProducto).HasColumnName("valorNetoProducto");
            entity.Property(e => e.ValorUnidad).HasColumnName("valorUnidad");
            entity.Property(e => e.ValorVentaProducto).HasColumnName("valorVentaProducto");

            entity.HasOne(d => d.IdCateProNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdCatePro)
                .HasConstraintName("FK_Productos_CategoriaProducto");

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdEmpresa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Productos_Empresas");
        });

        modelBuilder.Entity<Proveedore>(entity =>
        {
            entity.HasKey(e => e.IdProveedor).HasName("PK_proveedores_idProveedor");

            entity.ToTable("proveedores");

            entity.Property(e => e.IdProveedor).HasColumnName("idProveedor");
            entity.Property(e => e.Celular)
                .HasMaxLength(20)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("celular");
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("correo");
            entity.Property(e => e.Direccion)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("direccion");
            entity.Property(e => e.Nit)
                .HasMaxLength(50)
                .HasColumnName("nit");
            entity.Property(e => e.RazonSocial)
                .HasMaxLength(255)
                .HasColumnName("razonSocial");
        });

        modelBuilder.Entity<Sede>(entity =>
        {
            entity.HasKey(e => e.IdSede).HasName("PK_sede_id_sede");

            entity.ToTable("sede");

            entity.HasIndex(e => e.IdEmpresa, "id_empresa");

            entity.Property(e => e.IdSede).HasColumnName("id_sede");
            entity.Property(e => e.Ciudad)
                .HasMaxLength(25)
                .HasColumnName("ciudad");
            entity.Property(e => e.Direccion)
                .HasMaxLength(100)
                .HasColumnName("direccion");
            entity.Property(e => e.IdEmpresa)
                .HasMaxLength(50)
                .HasColumnName("id_empresa");
            entity.Property(e => e.NombreSede)
                .HasMaxLength(30)
                .HasColumnName("nombreSede");
            entity.Property(e => e.Telefono)
                .HasMaxLength(30)
                .HasColumnName("telefono");

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.Sedes)
                .HasForeignKey(d => d.IdEmpresa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sede_Empresas");
        });

        modelBuilder.Entity<Sedeempleado>(entity =>
        {
            entity.HasKey(e => e.IdSedeEmpleado).HasName("PK_sedeempleado_id_sedeEmpleado");

            entity.ToTable("sedeempleado");

            entity.HasIndex(e => e.Cedula, "cedula");

            entity.HasIndex(e => e.IdCargo, "id_cargo");

            entity.HasIndex(e => e.IdSede, "id_sede");

            entity.Property(e => e.IdSedeEmpleado).HasColumnName("id_sedeEmpleado");
            entity.Property(e => e.Cedula).HasColumnName("cedula");
            entity.Property(e => e.IdCargo).HasColumnName("id_cargo");
            entity.Property(e => e.IdSede).HasColumnName("id_sede");

            entity.HasOne(d => d.CedulaNavigation).WithMany(p => p.Sedeempleados)
                .HasForeignKey(d => d.Cedula)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SedeEmpleado_Empleado");

            entity.HasOne(d => d.IdCargoNavigation).WithMany(p => p.Sedeempleados)
                .HasForeignKey(d => d.IdCargo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SedeEmpleado_TipoCargo");

            entity.HasOne(d => d.IdSedeNavigation).WithMany(p => p.Sedeempleados)
                .HasForeignKey(d => d.IdSede)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SedeEmpleado_Sede");
        });

        modelBuilder.Entity<Seguimientoservicio>(entity =>
        {
            entity.HasKey(e => e.IdSeguiServ).HasName("PK__seguimie__661068BF2E5C6B5D");

            entity.ToTable("seguimientoservicio");

            entity.Property(e => e.Estado).HasMaxLength(50);
            entity.Property(e => e.FechaSeguimiento).HasColumnType("datetime");
            entity.Property(e => e.RespuestaCliente).HasMaxLength(255);

            entity.HasOne(d => d.IdOrdenNavigation).WithMany(p => p.Seguimientoservicios)
                .HasForeignKey(d => d.IdOrden)
                .HasConstraintName("OrdenSeguimientoFK");
        });

        modelBuilder.Entity<Servicio>(entity =>
        {
            entity.HasKey(e => e.IdServicio).HasName("PK_servicio_IdServicio");

            entity.ToTable("servicio");

            entity.Property(e => e.DescripcionServicio)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("descripcionServicio");
            entity.Property(e => e.NombreServicio)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("nombreServicio");
        });

        modelBuilder.Entity<Syncpdv>(entity =>
        {
            entity.HasKey(e => e.Idsync).HasName("PK_syncpdv_Idsync");

            entity.ToTable("syncpdv");

            entity.HasIndex(e => e.Cedula, "syncpdv_empleado_FK");

            entity.HasIndex(e => e.InfopdvId, "syncpdv_infopdv_FK");

            entity.Property(e => e.Cedula).HasColumnName("cedula");
            entity.Property(e => e.Estado)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("estado");
            entity.Property(e => e.FechaEstado)
                .HasPrecision(0)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("fechaEstado");

            entity.HasOne(d => d.CedulaNavigation).WithMany(p => p.Syncpdvs)
                .HasForeignKey(d => d.Cedula)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SyncPDV_Empleado");

            entity.HasOne(d => d.Infopdv).WithMany(p => p.Syncpdvs)
                .HasForeignKey(d => d.InfopdvId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SyncPDV_InfoPDV");
        });

        modelBuilder.Entity<Tipocargo>(entity =>
        {
            entity.HasKey(e => e.IdTipo).HasName("PK_tipocargo_id_tipo");

            entity.ToTable("tipocargo");

            entity.HasIndex(e => e.IdEmpresa, "id_empresa");

            entity.Property(e => e.IdTipo).HasColumnName("id_tipo");
            entity.Property(e => e.DescripcionCargo).HasColumnName("descripcionCargo");
            entity.Property(e => e.IdEmpresa)
                .HasMaxLength(50)
                .HasColumnName("id_empresa");
            entity.Property(e => e.NombreCargo)
                .HasMaxLength(50)
                .HasColumnName("nombreCargo");

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.Tipocargos)
                .HasForeignKey(d => d.IdEmpresa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TipoCargo_Empresas");
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => e.IdVenta).HasName("PK_ventas_id_venta");

            entity.ToTable("ventas");

            entity.Property(e => e.EstadoVenta).HasMaxLength(50);
            entity.Property(e => e.FechaVenta)
                .HasPrecision(0)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("fechaVenta");
            entity.Property(e => e.MetodoPago).HasMaxLength(50);
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.CedulaNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.Cedula)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ventas_Empleado");

            entity.HasOne(d => d.CedulaClienteNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.CedulaCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ventas_Cliente");
        });

        modelBuilder.Entity<VistaGananciaDiarium>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VistaGananciaDiaria");

            entity.Property(e => e.TotalCostos).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalIngresos).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalUtilidad).HasColumnType("decimal(18, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

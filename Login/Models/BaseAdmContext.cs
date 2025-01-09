using Microsoft.EntityFrameworkCore;

namespace Plataforma.Models;

public partial class BaseAdmContext : DbContext
{
    public BaseAdmContext(DbContextOptions<BaseAdmContext> options)
        : base(options)
    {
    }

    public DbSet<Empleado> Empleado { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Factura> Factura { get; set; }
    public DbSet<Pedidos> Pedidos { get; set; }
    public DbSet<Cliente> Cliente { get; set; }
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
    public DbSet<GananciaPedido> GananciaPedido { get; set; }
    public DbSet<Plataformas> Plataformas { get; set; }
    public DbSet<Proveedores> Proveedores { get; set; }
    public DbSet<HistoricoCompras> HistoricoCompras { get; set; }
    public DbSet<Servicio> Servicio { get; set; }
    public DbSet<CategoriaProductos> CategoriaProductos { get; set; }
    public DbSet<MenuOption> MenuOption { get; set; }
    public DbSet<Menu> Menu { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Llaves Primarias
        modelBuilder.Entity<Empleado>().HasKey(e => e.Cedula);
        modelBuilder.Entity<Producto>().HasKey(e => e.Cod_Producto);
        modelBuilder.Entity<Factura>().HasKey(e => e.Cod_factura);
        modelBuilder.Entity<Cliente>().HasKey(c => c.CedulaCliente);
        modelBuilder.Entity<Pedidos>().HasKey(cp => cp.Cod_pedido);
        modelBuilder.Entity<Ventas>().HasKey(cv => cv.Id_venta);
        modelBuilder.Entity<Ganancias>().HasKey(id => id.Id_ganancias);
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
        modelBuilder.Entity<GananciaPedido>().HasKey(ds => ds.IdGP);
        modelBuilder.Entity<Plataformas>().HasKey(iptlf => iptlf.IdPlataforma);
        modelBuilder.Entity<Proveedores>().HasKey(prvd => prvd.IdProveedor);
        modelBuilder.Entity<HistoricoCompras>().HasKey(prvd => prvd.IdHC);
        modelBuilder.Entity<Servicio>().HasKey(idser => idser.IdServicio);
        modelBuilder.Entity<CategoriaProductos>().HasKey(idcapro => idcapro.IdCateProducto);
        modelBuilder.Entity<MenuOption>().HasKey(idmo => idmo.IdMenuOption);
        modelBuilder.Entity<Menu>().HasKey(idm => idm.IdMenu);
        //Llaves foraneas
        modelBuilder.Entity<Factura>().HasOne<Cliente>().WithMany().HasForeignKey(f => f.Cedula_cliente);
        modelBuilder.Entity<Factura>().HasOne<Empleado>().WithMany().HasForeignKey(f => f.Cedula);
        modelBuilder.Entity<Pedidos>().HasOne<Factura>().WithMany().HasForeignKey(f => f.Cod_factura);
        modelBuilder.Entity<Pedidos>().HasOne<Producto>().WithMany().HasForeignKey(f => f.Cod_producto);
        modelBuilder.Entity<Pedidos>().HasOne<Infopdv>().WithMany().HasForeignKey(f => f.InfopdvId);
        modelBuilder.Entity<TipoCargo>().HasOne<Empresas>().WithMany().HasForeignKey(f => f.Id_empresa);
        modelBuilder.Entity<Sede>().HasOne<Empresas>().WithMany().HasForeignKey(f => f.Id_empresa);
        modelBuilder.Entity<EmpleadoEmpresa>().HasOne<Empleado>().WithMany().HasForeignKey(f => f.Cedula);
        modelBuilder.Entity<EmpleadoEmpresa>().HasOne<Empresas>().WithMany().HasForeignKey(f => f.Id_empresa);
        modelBuilder.Entity<Sedeempleado>().HasOne<Sede>().WithMany().HasForeignKey(f => f.Id_sede);
        modelBuilder.Entity<Sedeempleado>().HasOne<Empleado>().WithMany().HasForeignKey(f => f.Cedula);
        modelBuilder.Entity<Sedeempleado>().HasOne<TipoCargo>().WithMany().HasForeignKey(f => f.Id_cargo);
        modelBuilder.Entity<Plataformasuscripcion>().HasOne<Empleado>().WithMany().HasForeignKey(f => f.CedulaEmpleado);
        modelBuilder.Entity<Plataformasuscripcion>().HasOne<Plataformas>().WithMany().HasForeignKey(f => f.IdPlataforma);
        modelBuilder.Entity<LogsLogin>().HasOne<Infopdv>().WithMany().HasForeignKey(f => f.InfopdvId);
        modelBuilder.Entity<ClientesPlataforma>().HasOne<Plataformasuscripcion>().WithMany().HasForeignKey(f => f.IdPltfSuscripcion);
        modelBuilder.Entity<Infopdv>().HasOne<Empresas>().WithMany().HasForeignKey(f => f.Id_Empresa);
        modelBuilder.Entity<Infopdv>().HasOne<Sede>().WithMany().HasForeignKey(f => f.Id_Sede);
        modelBuilder.Entity<Syncpdv>().HasOne<Infopdv>().WithMany().HasForeignKey(f => f.InfopdvId);
        modelBuilder.Entity<Syncpdv>().HasOne<Empleado>().WithMany().HasForeignKey(f => f.Cedula);
        modelBuilder.Entity<GananciaPedido>().HasOne<Pedidos>().WithMany().HasForeignKey(f => f.Cod_pedido);
        modelBuilder.Entity<HistoricoCompras>().HasOne<Producto>().WithMany().HasForeignKey(f => f.Cod_Producto);
        modelBuilder.Entity<HistoricoCompras>().HasOne<Factura>().WithMany().HasForeignKey(f => f.cod_factura);
        modelBuilder.Entity<CategoriaProductos>().HasOne<Servicio>().WithMany().HasForeignKey(f => f.IdServicio);
        modelBuilder.Entity<MenuOption>().HasOne<Empresas>().WithMany().HasForeignKey(f => f.Id_Empresa);
        modelBuilder.Entity<MenuOption>().HasOne<TipoCargo>().WithMany().HasForeignKey(f => f.Id_Tipo);
        modelBuilder.Entity<MenuOption>().HasOne<Menu>().WithMany().HasForeignKey(f => f.IdMenu);
    }
}

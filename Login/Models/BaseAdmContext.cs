using Login.Models;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Llaves Primarias
        modelBuilder.Entity<Empleado>().HasKey(e => e.Cedula);
        modelBuilder.Entity<Producto>().HasKey(e => e.Cod_Producto);
        modelBuilder.Entity<Factura>().HasKey(e => e.cod_factura);
        modelBuilder.Entity<Cliente>().HasKey(c => c.cedulaCliente);
        modelBuilder.Entity<Pedidos>().HasKey(cp => cp.cod_pedido);
        modelBuilder.Entity<Ventas>().HasKey(cv => cv.id_venta);
        modelBuilder.Entity<Ganancias>().HasKey(id => id.id_ganancias);
        modelBuilder.Entity<TipoCargo>().HasKey(it => it.id_tipo);
        modelBuilder.Entity<Empresas>().HasKey(ie => ie.id_empresa);
        modelBuilder.Entity<Sede>().HasKey(sede => sede.id_sede);
        modelBuilder.Entity<EmpleadoEmpresa>().HasKey(ie => ie.id_empleadoE);
        modelBuilder.Entity<Sedeempleado>().HasKey(ise => ise.id_sedeEmpleado);
        modelBuilder.Entity<LogsLogin>().HasKey(ise => ise.Id_log);
        modelBuilder.Entity<Plataformasuscripcion>().HasKey(ip => ip.idPltfSuscripcion);
        modelBuilder.Entity<ClientesPlataforma>().HasKey(icp => icp.idCliPltf);
        modelBuilder.Entity<Infopdv>().HasKey(iv => iv.InfopdvId);
        modelBuilder.Entity<Syncpdv>().HasKey(ds => ds.Idsync);
        modelBuilder.Entity<GananciaPedido>().HasKey(ds => ds.idGP);
        modelBuilder.Entity<Plataformas>().HasKey(iptlf => iptlf.IdPlataforma);
        //Llaves foraneas
        modelBuilder.Entity<Factura>().HasOne<Cliente>().WithMany().HasForeignKey(f => f.cedula_cliente);
        modelBuilder.Entity<Factura>().HasOne<Empleado>().WithMany().HasForeignKey(f => f.cedula);
        modelBuilder.Entity<Pedidos>().HasOne<Factura>().WithMany().HasForeignKey(f => f.cod_factura);
        modelBuilder.Entity<Pedidos>().HasOne<Producto>().WithMany().HasForeignKey(f => f.cod_producto);
        modelBuilder.Entity<Pedidos>().HasOne<Infopdv>().WithMany().HasForeignKey(f => f.InfopdvId);
        modelBuilder.Entity<TipoCargo>().HasOne<Empresas>().WithMany().HasForeignKey(f => f.id_empresa);
        modelBuilder.Entity<Sede>().HasOne<Empresas>().WithMany().HasForeignKey(f => f.id_empresa);
        modelBuilder.Entity<EmpleadoEmpresa>().HasOne<Empleado>().WithMany().HasForeignKey(f => f.cedula);
        modelBuilder.Entity<EmpleadoEmpresa>().HasOne<Empresas>().WithMany().HasForeignKey(f => f.id_empresa);
        modelBuilder.Entity<Sedeempleado>().HasOne<Sede>().WithMany().HasForeignKey(f => f.id_sede);
        modelBuilder.Entity<Sedeempleado>().HasOne<Empleado>().WithMany().HasForeignKey(f => f.cedula);
        modelBuilder.Entity<Sedeempleado>().HasOne<TipoCargo>().WithMany().HasForeignKey(f => f.id_cargo);
        modelBuilder.Entity<Plataformasuscripcion>().HasOne<Empleado>().WithMany().HasForeignKey(f => f.cedulaEmpleado);
        modelBuilder.Entity<Plataformasuscripcion>().HasOne<Plataformas>().WithMany().HasForeignKey(f => f.idPlataforma);
        modelBuilder.Entity<LogsLogin>().HasOne<Infopdv>().WithMany().HasForeignKey(f => f.InfopdvId);
        modelBuilder.Entity<ClientesPlataforma>().HasOne<Plataformasuscripcion>().WithMany().HasForeignKey(f => f.idPltfSuscripcion);
        modelBuilder.Entity<Infopdv>().HasOne<Empresas>().WithMany().HasForeignKey(f => f.Id_Empresa);
        modelBuilder.Entity<Infopdv>().HasOne<Sede>().WithMany().HasForeignKey(f => f.Id_Sede);
        modelBuilder.Entity<Syncpdv>().HasOne<Infopdv>().WithMany().HasForeignKey(f => f.InfopdvId);
        modelBuilder.Entity<Syncpdv>().HasOne<Empleado>().WithMany().HasForeignKey(f => f.Cedula);
        modelBuilder.Entity<GananciaPedido>().HasOne<Pedidos>().WithMany().HasForeignKey(f => f.cod_pedido);
    }
}

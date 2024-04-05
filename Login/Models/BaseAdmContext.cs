using System;
using System.Collections.Generic;
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
    public DbSet<Pedido> Pedido { get; set; }
    public DbSet<Cliente> Cliente { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Llaves Primarias
        modelBuilder.Entity<Empleado>().HasKey(e => e.Cedula);
        modelBuilder.Entity<Producto>().HasKey(e => e.Cod_Producto);
        modelBuilder.Entity<Factura>().HasKey(e => e.cod_factura);
        modelBuilder.Entity<Cliente>().HasKey(c => c.cedulaCliente);
        modelBuilder.Entity<Pedido>().HasKey(cp => cp.cod_pedido);
        //Llaves foraneas
        modelBuilder.Entity<Factura>().HasOne<Cliente>().WithMany().HasForeignKey(f => f.cedula_cliente);
        modelBuilder.Entity<Factura>().HasOne<Empleado>().WithMany().HasForeignKey(f => f.cedula);
        modelBuilder.Entity<Pedido>().HasOne<Factura>().WithMany().HasForeignKey(f => f.cod_factura);
        modelBuilder.Entity<Pedido>().HasOne<Producto>().WithMany().HasForeignKey(f => f.cod_producto);
    }
}

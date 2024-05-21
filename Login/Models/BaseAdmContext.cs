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
    public DbSet<Pedidos> Pedidos { get; set; }
    public DbSet<Cliente> Cliente { get; set; }
    public DbSet<Ventas> Ventas { get; set; }
    public DbSet<Ganancias> Ganancias { get; set; }
    public DbSet<TipoCargo> TipoCargo { get; set; }
    public DbSet<Empresas> Empresas { get; set; }
    public DbSet<Sede> Sede { get; set; }
    public DbSet<EmpleadoEmpresa> EmpleadoEmpresa { get; set; }

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
        //Llaves foraneas
        modelBuilder.Entity<Factura>().HasOne<Cliente>().WithMany().HasForeignKey(f => f.cedula_cliente);
        modelBuilder.Entity<Factura>().HasOne<Empleado>().WithMany().HasForeignKey(f => f.cedula);
        modelBuilder.Entity<Pedidos>().HasOne<Factura>().WithMany().HasForeignKey(f => f.cod_factura);
        modelBuilder.Entity<Pedidos>().HasOne<Producto>().WithMany().HasForeignKey(f => f.cod_producto);
        modelBuilder.Entity<Ventas>().HasOne<Factura>().WithMany().HasForeignKey(f => f.cod_factura);
        modelBuilder.Entity<Ganancias>().HasOne<Ventas>().WithMany().HasForeignKey(f => f.id_venta);
        modelBuilder.Entity<TipoCargo>().HasOne<Empresas>().WithMany().HasForeignKey(f => f.id_empresa);
        modelBuilder.Entity<Sede>().HasOne<Empresas>().WithMany().HasForeignKey(f => f.id_empresa);
        modelBuilder.Entity<EmpleadoEmpresa>().HasOne<Empleado>().WithMany().HasForeignKey(f => f.cedula);
        modelBuilder.Entity<EmpleadoEmpresa>().HasOne<Empresas>().WithMany().HasForeignKey(f => f.id_empresa);
    }
}

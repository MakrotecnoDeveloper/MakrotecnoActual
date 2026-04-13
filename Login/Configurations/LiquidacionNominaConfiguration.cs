using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plataforma.Models;

namespace Plataforma.Data.Configurations
{
    public class LiquidacionNominaConfiguration : IEntityTypeConfiguration<LiquidacionNomina>
    {
        public void Configure(EntityTypeBuilder<LiquidacionNomina> entity)
        {
            entity.ToTable("LiquidacionesNomina", t =>
            {
                t.HasCheckConstraint("CK_LiquidacionesNomina_Estado", "[Estado] IN ('Borrador', 'Confirmada', 'Pagada', 'Anulada')");
                t.HasCheckConstraint("CK_LiquidacionesNomina_Totales", "[TotalDevengado] >= 0 AND [TotalDeducciones] >= 0 AND [NetoPagar] >= 0");
            });

            entity.HasKey(e => e.IdLiquidacion);

            entity.Property(e => e.Cedula).HasMaxLength(20).IsRequired();
            entity.Property(e => e.TotalDevengado).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            entity.Property(e => e.TotalDeducciones).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            entity.Property(e => e.NetoPagar).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            entity.Property(e => e.Estado).HasMaxLength(20).HasDefaultValue("Borrador");
            entity.Property(e => e.UsuarioLiquida).HasMaxLength(100);
            entity.Property(e => e.Observaciones).HasMaxLength(500);
            entity.Property(e => e.FechaLiquidacion).HasDefaultValueSql("CAST(GETDATE() AS DATE)");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("SYSDATETIME()");

            entity.HasOne(e => e.Empleado)
                .WithMany(e => e.LiquidacionesNomina)
                .HasForeignKey(e => e.Cedula)
                .HasPrincipalKey(e => e.Cedula)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Contrato)
                .WithMany(e => e.LiquidacionesNomina)
                .HasForeignKey(e => e.IdContrato)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.PeriodoNomina)
                .WithMany(e => e.LiquidacionesNomina)
                .HasForeignKey(e => e.IdPeriodo)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.Cedula)
                .HasDatabaseName("IX_LiquidacionesNomina_Cedula");

            entity.HasIndex(e => new { e.Cedula, e.IdPeriodo })
                .IsUnique()
                .HasDatabaseName("UX_LiquidacionesNomina_Cedula_IdPeriodo");
        }
    }
}
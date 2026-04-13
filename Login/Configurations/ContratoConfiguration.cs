using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plataforma.Models;

namespace Plataforma.Data.Configurations
{
    public class ContratoConfiguration : IEntityTypeConfiguration<Contrato>
    {
        public void Configure(EntityTypeBuilder<Contrato> entity)
        {
            entity.ToTable("Contratos", t =>
            {
                t.HasCheckConstraint("CK_Contratos_Estado", "[Estado] IN ('Activo', 'Suspendido', 'Finalizado', 'Liquidado')");
                t.HasCheckConstraint("CK_Contratos_Fechas", "[FechaFin] IS NULL OR [FechaFin] >= [FechaInicio]");
                t.HasCheckConstraint("CK_Contratos_SalarioBase", "[SalarioBase] >= 0");
            });

            entity.HasKey(e => e.IdContrato);

            entity.Property(e => e.Cedula).IsRequired();
            entity.Property(e => e.TipoContrato).HasMaxLength(30).IsRequired();
            entity.Property(e => e.Cargo).HasMaxLength(100);
            entity.Property(e => e.Area).HasMaxLength(100);
            entity.Property(e => e.TipoSalario).HasMaxLength(30).HasDefaultValue("Fijo");
            entity.Property(e => e.SalarioBase).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Estado).HasMaxLength(20).HasDefaultValue("Activo");
            entity.Property(e => e.Observaciones).HasMaxLength(500);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("SYSDATETIME()");

            entity.HasOne(e => e.Empleado)
                .WithMany(e => e.Contratos)
                .HasForeignKey(e => e.Cedula)
                .HasPrincipalKey(e => e.Cedula)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.Cedula)
                .HasDatabaseName("IX_Contratos_Cedula");

            entity.HasIndex(e => e.Cedula)
                .HasFilter("[Estado] = 'Activo'")
                .IsUnique()
                .HasDatabaseName("UX_Contratos_Cedula_Activo");
        }
    }
}
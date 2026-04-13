using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plataforma.Models;

namespace Plataforma.Data.Configurations
{
    public class DetalleConceptoEmpleadoConfiguration : IEntityTypeConfiguration<DetalleConceptoEmpleado>
    {
        public void Configure(EntityTypeBuilder<DetalleConceptoEmpleado> entity)
        {
            entity.ToTable("DetalleConceptosEmpleado", t =>
            {
                t.HasCheckConstraint("CK_DetalleConceptosEmpleado_Fechas", "[FechaFin] IS NULL OR [FechaFin] >= [FechaInicio]");
                t.HasCheckConstraint("CK_DetalleConceptosEmpleado_Porcentaje", "[PorcentajePersonalizado] IS NULL OR [PorcentajePersonalizado] >= 0");
                t.HasCheckConstraint("CK_DetalleConceptosEmpleado_ValorFijo", "[ValorFijoPersonalizado] IS NULL OR [ValorFijoPersonalizado] >= 0");
            });

            entity.HasKey(e => e.IdDetalle);

            entity.Property(e => e.Cedula).IsRequired();
            entity.Property(e => e.PorcentajePersonalizado).HasColumnType("decimal(9,4)");
            entity.Property(e => e.ValorFijoPersonalizado).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Observacion).HasMaxLength(300);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("SYSDATETIME()");
            entity.Property(e => e.FechaInicio).HasDefaultValueSql("CAST(GETDATE() AS DATE)");

            entity.HasOne(e => e.Empleado)
                .WithMany(e => e.DetalleConceptosEmpleado)
                .HasForeignKey(e => e.Cedula)
                .HasPrincipalKey(e => e.Cedula)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ConceptoNomina)
                .WithMany(e => e.DetalleConceptosEmpleado)
                .HasForeignKey(e => e.IdConcepto)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.Cedula)
                .HasDatabaseName("IX_DetalleConceptosEmpleado_Cedula");

            entity.HasIndex(e => new { e.Cedula, e.IdConcepto })
                .HasFilter("[Estado] = 1")
                .IsUnique()
                .HasDatabaseName("UX_DetalleConceptosEmpleado_Cedula_IdConcepto_Activo");
        }
    }
}
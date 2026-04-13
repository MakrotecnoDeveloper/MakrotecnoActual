using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plataforma.Models;

namespace Plataforma.Data.Configurations
{
    public class PeriodoNominaConfiguration : IEntityTypeConfiguration<PeriodoNomina>
    {
        public void Configure(EntityTypeBuilder<PeriodoNomina> entity)
        {
            entity.ToTable("PeriodosNomina", t =>
            {
                t.HasCheckConstraint("CK_PeriodosNomina_TipoPeriodo", "[TipoPeriodo] IN ('Mensual', 'Quincenal', 'Semanal')");
                t.HasCheckConstraint("CK_PeriodosNomina_Estado", "[Estado] IN ('Abierto', 'Calculado', 'Cerrado', 'Pagado')");
                t.HasCheckConstraint("CK_PeriodosNomina_Fechas", "[FechaFin] >= [FechaInicio]");
            });

            entity.HasKey(e => e.IdPeriodo);

            entity.Property(e => e.Descripcion).HasMaxLength(100).IsRequired();
            entity.Property(e => e.TipoPeriodo).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Estado).HasMaxLength(20).HasDefaultValue("Abierto");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("SYSDATETIME()");

            entity.HasIndex(e => new { e.FechaInicio, e.FechaFin })
                .IsUnique()
                .HasDatabaseName("UQ_PeriodosNomina_Rango");
        }
    }
}
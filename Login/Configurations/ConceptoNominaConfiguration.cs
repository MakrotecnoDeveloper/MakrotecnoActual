using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plataforma.Models;

namespace Plataforma.Data.Configurations
{
    public class ConceptoNominaConfiguration : IEntityTypeConfiguration<ConceptoNomina>
    {
        public void Configure(EntityTypeBuilder<ConceptoNomina> entity)
        {
            entity.ToTable("ConceptosNomina", t =>
            {
                t.HasCheckConstraint("CK_ConceptosNomina_Naturaleza", "[Naturaleza] IN ('Ingreso', 'Deduccion', 'AporteEmpresa', 'Provision')");
                t.HasCheckConstraint("CK_ConceptosNomina_ModoCalculo", "[ModoCalculo] IN ('Fijo', 'Porcentaje', 'Manual', 'Formula')");
                t.HasCheckConstraint("CK_ConceptosNomina_Porcentaje", "[Porcentaje] IS NULL OR [Porcentaje] >= 0");
                t.HasCheckConstraint("CK_ConceptosNomina_ValorFijo", "[ValorFijo] IS NULL OR [ValorFijo] >= 0");
            });

            entity.HasKey(e => e.IdConcepto);

            entity.Property(e => e.Codigo).HasMaxLength(30).IsRequired();
            entity.Property(e => e.Nombre).HasMaxLength(150).IsRequired();
            entity.Property(e => e.Naturaleza).HasMaxLength(20).IsRequired();
            entity.Property(e => e.ModoCalculo).HasMaxLength(20).IsRequired();
            entity.Property(e => e.BaseCalculo).HasMaxLength(50);
            entity.Property(e => e.Porcentaje).HasColumnType("decimal(9,4)");
            entity.Property(e => e.ValorFijo).HasColumnType("decimal(18,2)");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("SYSDATETIME()");

            entity.HasIndex(e => e.Codigo)
                .IsUnique()
                .HasDatabaseName("UQ_ConceptosNomina_Codigo");

            entity.HasOne(e => e.Servicio)
                .WithMany(s => s.ConceptosNomina)
                .HasForeignKey(e => e.IdServicio)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plataforma.Models;

namespace Plataforma.Data.Configurations
{
    public class NovedadNominaConfiguration : IEntityTypeConfiguration<NovedadNomina>
    {
        public void Configure(EntityTypeBuilder<NovedadNomina> entity)
        {
            entity.ToTable("NovedadesNomina", t =>
            {
                t.HasCheckConstraint("CK_NovedadesNomina_Cantidad", "[Cantidad] >= 0");
                t.HasCheckConstraint("CK_NovedadesNomina_Valor", "[Valor] >= 0");
                t.HasCheckConstraint("CK_NovedadesNomina_Porcentaje", "[PorcentajeAplicado] IS NULL OR [PorcentajeAplicado] >= 0");
                t.HasCheckConstraint("CK_NovedadesNomina_Estado", "[Estado] IN ('Pendiente', 'Aplicada', 'Anulada')");
            });

            entity.HasKey(e => e.IdNovedad);

            entity.Property(e => e.Cedula).IsRequired();
            entity.Property(e => e.Cantidad).HasColumnType("decimal(18,4)").HasDefaultValue(1m);
            entity.Property(e => e.BaseValor).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PorcentajeAplicado).HasColumnType("decimal(9,4)");
            entity.Property(e => e.Valor).HasColumnType("decimal(18,2)");
            entity.Property(e => e.DocumentoOrigen).HasMaxLength(100);
            entity.Property(e => e.Origen).HasMaxLength(30).HasDefaultValue("Manual");
            entity.Property(e => e.Estado).HasMaxLength(20).HasDefaultValue("Pendiente");
            entity.Property(e => e.Observacion).HasMaxLength(500);
            entity.Property(e => e.FechaNovedad).HasDefaultValueSql("CAST(GETDATE() AS DATE)");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("SYSDATETIME()");

            entity.HasOne(e => e.Empleado)
                .WithMany(e => e.NovedadesNomina)
                .HasForeignKey(e => e.Cedula)
                .HasPrincipalKey(e => e.Cedula)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ConceptoNomina)
                .WithMany(e => e.NovedadesNomina)
                .HasForeignKey(e => e.IdConcepto)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.PeriodoNomina)
                .WithMany(e => e.NovedadesNomina)
                .HasForeignKey(e => e.IdPeriodo)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.Cedula, e.IdPeriodo })
                .HasDatabaseName("IX_NovedadesNomina_Cedula_IdPeriodo");
        }
    }
}
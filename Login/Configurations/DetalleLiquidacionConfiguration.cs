using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plataforma.Models;

namespace Plataforma.Data.Configurations
{
    public class DetalleLiquidacionConfiguration : IEntityTypeConfiguration<DetalleLiquidacion>
    {
        public void Configure(EntityTypeBuilder<DetalleLiquidacion> entity)
        {
            entity.ToTable("DetalleLiquidacion", t =>
            {
                t.HasCheckConstraint("CK_DetalleLiquidacion_Naturaleza", "[Naturaleza] IN ('Ingreso', 'Deduccion', 'AporteEmpresa', 'Provision')");
                t.HasCheckConstraint("CK_DetalleLiquidacion_Cantidad", "[Cantidad] >= 0");
                t.HasCheckConstraint("CK_DetalleLiquidacion_Monto", "[Monto] >= 0");
                t.HasCheckConstraint("CK_DetalleLiquidacion_Porcentaje", "[PorcentajeAplicado] IS NULL OR [PorcentajeAplicado] >= 0");
                t.HasCheckConstraint("CK_DetalleLiquidacion_ValorUnitario", "[ValorUnitario] IS NULL OR [ValorUnitario] >= 0");
            });

            entity.HasKey(e => e.IdDetalle);

            entity.Property(e => e.NombreConcepto).HasMaxLength(150).IsRequired();
            entity.Property(e => e.Naturaleza).HasMaxLength(20).IsRequired();
            entity.Property(e => e.BaseAplicada).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Cantidad).HasColumnType("decimal(18,4)").HasDefaultValue(1m);
            entity.Property(e => e.PorcentajeAplicado).HasColumnType("decimal(9,4)");
            entity.Property(e => e.ValorUnitario).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Monto).HasColumnType("decimal(18,2)");
            entity.Property(e => e.OrigenDetalle).HasMaxLength(30);
            entity.Property(e => e.Observacion).HasMaxLength(300);

            entity.HasOne(e => e.LiquidacionNomina)
                .WithMany(e => e.DetalleLiquidacion)
                .HasForeignKey(e => e.IdLiquidacion)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ConceptoNomina)
                .WithMany(e => e.DetallesLiquidacion)
                .HasForeignKey(e => e.IdConcepto)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.IdLiquidacion)
                .HasDatabaseName("IX_DetalleLiquidacion_IdLiquidacion");
        }
    }
}

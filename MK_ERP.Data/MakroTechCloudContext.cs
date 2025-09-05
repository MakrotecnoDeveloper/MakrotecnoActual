using Microsoft.EntityFrameworkCore;
using MK_ERP.Models.Cloud_Mk;
namespace MK_ERP.Data;

public partial class MakroTechCloudContext : DbContext
{
    public MakroTechCloudContext()
    {
    }

    public MakroTechCloudContext(DbContextOptions<MakroTechCloudContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CloudUser> CloudUsers { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=DEVELOPERIST\\DEVELOPERIST; Database=MakroTechCloud; TrustServerCertificate=True; Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CloudUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CloudUse__3214EC07D74C6F86");

            entity.Property(e => e.BaseDatosCodigo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UsuarioCodigo)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

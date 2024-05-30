using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LabApp.Models;

public partial class LabAppDataBaseContext : DbContext
{
    public LabAppDataBaseContext()
    {
    }

    public LabAppDataBaseContext(DbContextOptions<LabAppDataBaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Room> Rooms { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLExpress; Database=LabAppDataBase; Trusted_Connection=True; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rooms__3214EC07EDD2A6A3");

            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("endDate");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("startDate");
            entity.Property(e => e.UserId)
                .HasMaxLength(450)
                .HasColumnName("userId");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

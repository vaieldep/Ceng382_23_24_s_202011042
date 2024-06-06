using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LabApp.Models;

public partial class FinalDataBaseContext : DbContext
{
    public FinalDataBaseContext()
    {
    }

    public FinalDataBaseContext(DbContextOptions<FinalDataBaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblReservation> TblReservations { get; set; }

    public virtual DbSet<TblRoom> TblRooms { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var builder = WebApplication.CreateBuilder();
        var connectionString = builder.Configuration.GetConnectionString ("MyConnection");
        optionsBuilder.UseSqlServer(connectionString);
    }    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblReservation>(entity =>
        {
            entity.ToTable("tbl_Reservations");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.RoomId).HasColumnName("roomId");
            entity.Property(e => e.Time).HasColumnName("time");
            entity.Property(e => e.UserId).HasColumnName("userId");
        });

        modelBuilder.Entity<TblRoom>(entity =>
        {
            entity.ToTable("tbl_Rooms");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.RoomName)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("roomName");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

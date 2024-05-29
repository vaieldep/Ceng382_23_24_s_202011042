using Microsoft.EntityFrameworkCore;
using WEB_LAB9.Models; // Room sınıfı için gerekli using direktifi

public class AppDbContext : DbContext
{
public AppDbContext(DbContextOptions<AppDbContext> options) :
base(options)
{
}
public DbSet<Room> Rooms { get; set; }
}

namespace WEB_LAB9.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }
    }
}
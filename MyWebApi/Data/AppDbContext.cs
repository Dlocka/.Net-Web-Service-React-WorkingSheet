using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Staff> Staffs { get; set; }
    public DbSet<WorkHour> WorkHours{ get; set; }
}

using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Staff> Staffs { get; set; }
    public DbSet<WorkHour> WorkHours { get; set; }
    public DbSet<Job> Jobs { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Foreign key setup (optional if EF infers it)
        modelBuilder.Entity<WorkHour>()
         .HasOne(w => w.Staff)
         .WithMany(s => s.WorkHours)
         .HasForeignKey(w => w.StaffId)
         .IsRequired();

        modelBuilder.Entity<WorkHour>()
            .HasOne(w => w.Job)
            .WithMany()
            .HasForeignKey(w => w.JobId);

        // Used to Update a Staff's work hours by date and time
        modelBuilder.Entity<WorkHour>()
            .HasIndex(w => new { w.StaffId, w.Date, w.StartTime });
        // Used to get a staff's worksheets by a week or month.
        modelBuilder.Entity<WorkHour>().HasIndex(w => new { w.Date, w.StaffId });
       
    }
    
}

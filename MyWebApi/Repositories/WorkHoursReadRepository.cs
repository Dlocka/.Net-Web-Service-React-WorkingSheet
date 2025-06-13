using Microsoft.EntityFrameworkCore;
public class WorkHoursReadRepository : IWorkHoursReadRepository
{
     private readonly AppDbContext _context;
     public WorkHoursReadRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<WorkHour>> GetWorkHoursByStaffIdAsync(int staffId)
    {
       return await _context.WorkHours
            .Where(wh => wh.StaffId == staffId)
            .ToListAsync();
    }
}
using Microsoft.EntityFrameworkCore;
public class WorkHoursReadRepository : IWorkHoursReadRepository
{
    private readonly AppDbContext _context;
    public WorkHoursReadRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<WorkHour>> CheckOverlapAsync(int staffId, List<WorkHourDto> dtos)
    {
        var result = new List<WorkHour>();

        foreach (var dto in dtos)
        {
            dto.StaffId = staffId;
            //verify the validation of input time
            if (dto.EndTime <= dto.StartTime)
            {
                throw new ArgumentException($"Invalid time range: EndTime must be after StartTime. Got StartTime: {dto.StartTime}, EndTime: {dto.EndTime}");
            }
            var overlaps = await _context.WorkHours
                .Where(w => w.StaffId == staffId &&
                            w.Date == dto.Date &&
                            w.StartTime < dto.EndTime &&
                            w.EndTime > dto.StartTime)
                .ToListAsync();

            result.AddRange(overlaps);
        }

        return result;
    }

    public async Task<double> GetRemainingWorkMinutesAsync(int staffId, int jobId, DateTime from, DateTime endDateTime)
{
    var now = from;

    if (endDateTime <= now)
        return 0; // If the end time is before 'now', there's no remaining time.

    var workHours = await _context.WorkHours
        .Where(wh => wh.StaffId == staffId && wh.JobId == jobId && wh.OnWork)
        .ToListAsync();

    double totalMinutes = 0;

    foreach (var wh in workHours)
    {
        var startDateTime = wh.Date.ToDateTime(wh.StartTime);
        var endDateTimeOfWork = wh.Date.ToDateTime(wh.EndTime);

        // Only consider work periods that overlap with [now, endDateTime]
        if (endDateTimeOfWork <= now || startDateTime >= endDateTime)
            continue;

        var effectiveStart = startDateTime < now ? now : startDateTime;
        var effectiveEnd = endDateTimeOfWork > endDateTime ? endDateTime : endDateTimeOfWork;

        totalMinutes += (effectiveEnd - effectiveStart).TotalMinutes;
    }

    return totalMinutes;
}


    public async Task<IEnumerable<WorkHour>> GetWorkHoursByStaffIdAsync(int staffId)
    {
        return await _context.WorkHours
             .Where(wh => wh.StaffId == staffId)
             .Include(wh => wh.Job)
             .Include(wh => wh.Staff)
             .ToListAsync();
    }
    
    public async Task<IEnumerable<WorkHour>> GetWorkHoursInRange(int staffId, DateOnly startDate, DateOnly endDate)
    {
        return await _context.WorkHours
            .Where(wh => wh.StaffId == staffId && wh.Date >= startDate && wh.Date <= endDate)
            .ToListAsync();
    }

    public async Task<List<WorkHour>> GetWorkHoursInRangeAsync(int staffId, DateOnly start, DateOnly end)
    {
        return await _context.WorkHours
            .Where(wh => wh.StaffId == staffId && wh.Date >= start && wh.Date <= end)
            .ToListAsync();
    }
}
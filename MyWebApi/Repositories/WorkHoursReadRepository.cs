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

    public async Task<List<OverworkRecordDto>> GetOverworkDaysAsync(double thresholdHours = 10)
    {
   // First, execute the GroupBy and select necessary raw data into memory
    // This part runs on the database as much as possible for SQLite
    var rawGroupedData = await _context.WorkHours
        .Include(w => w.Staff) // This include is still important for Staff.Name
        .GroupBy(w => new { w.StaffId, w.Date, w.Staff.Name })
        .Select(g => new
        {
            // Select the key components and the raw StartTime/EndTime for each group.
            // We use Max/Min here to get the boundaries for the day from potentially multiple entries.
            // SQLite might still struggle with TimeOnly itself in this initial Select,
            // so we might need to project it as string or another compatible type if it's stored that way.
            // Assuming your WorkHour.StartTime and EndTime are mapped to TimeOnly properly by EF Core
            // for the database query part, this should be fine.
            StaffId = g.Key.StaffId, // Keep StaffId if you need it for debugging/tracing, not strictly required for final DTO
            Date = g.Key.Date,
            StaffName = g.Key.Name,
            MinStartTime = g.Min(w => w.StartTime), // Get the earliest start time for the day
            MaxEndTime = g.Max(w => w.EndTime)     // Get the latest end time for the day
        })
        .ToListAsync(); // <-- Execute this part of the query on the database asynchronously

    // Now, perform the TimeOnly.ToTimeSpan() calculation and filtering in-memory
    var overworkRecords = rawGroupedData
        .Select(x => new
        {
            x.Date,
            x.StaffName,
            // Perform the TimeOnly to TimeSpan conversion and calculation in-memory
            TotalMinutes = (x.MaxEndTime.ToTimeSpan() - x.MinStartTime.ToTimeSpan()).TotalMinutes
        })
        .Where(x => x.TotalMinutes > thresholdHours * 60)
        .Select(x => new OverworkRecordDto
        {
            Date = x.Date,
            StaffName = x.StaffName
        })
        .ToList(); // <-- Use ToList() because it's an in-memory IEnumerable<T> now

    return overworkRecords;
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
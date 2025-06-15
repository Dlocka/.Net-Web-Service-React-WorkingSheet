using Microsoft.EntityFrameworkCore;
using System.Text.Json;

public class WorkHoursWriteRepository : IWorkHoursWriteRepository
{
    private readonly AppDbContext _context;

    public WorkHoursWriteRepository(AppDbContext context)
    {
        _context = context;
    }


   


    public async Task<int> SetWorkHoursAsync(int staffId, List<WorkHourDto> workHourDtos)
    {
        foreach (var dto in workHourDtos)
        {
            dto.StaffId = staffId;

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

            foreach (var existing in overlaps)
            {
                // Full overlap: new range fully covers old — remove it
                if (dto.StartTime <= existing.StartTime && dto.EndTime >= existing.EndTime)
                {
                    _context.WorkHours.Remove(existing);
                }
                // Partial overlap at the start: existing starts before and ends inside new
                else if (existing.StartTime < dto.StartTime && existing.EndTime > dto.StartTime && existing.EndTime <= dto.EndTime)
                {
                    existing.EndTime = dto.StartTime;
                    _context.Entry(existing).State = EntityState.Modified;
                }
                // Partial overlap at the end: existing starts inside and ends after new
                else if (existing.StartTime >= dto.StartTime && existing.StartTime < dto.EndTime && existing.EndTime > dto.EndTime)
                {
                    existing.StartTime = dto.EndTime;
                    _context.Entry(existing).State = EntityState.Modified;
                }
                // New block is inside existing one → split into two
                else if (existing.StartTime < dto.StartTime && existing.EndTime > dto.EndTime)
                {
                    // Modify the existing one to end before new block
                    var secondHalf = new WorkHour
                    {
                        StaffId = existing.StaffId,
                        Date = existing.Date,
                        StartTime = dto.EndTime,
                        EndTime = existing.EndTime,
                        TaskDescription = existing.TaskDescription,
                        JobId = existing.JobId,
                        OnWork = existing.OnWork
                    };

                    existing.EndTime = dto.StartTime;

                    _context.Entry(existing).State = EntityState.Modified;
                    _context.WorkHours.Add(secondHalf);
                }
            }

            // Add the new block
            var newWorkHour = new WorkHour
            {
                StaffId = dto.StaffId,
                Date = dto.Date,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                TaskDescription = dto.TaskDescription,
                JobId = dto.JobId,
                OnWork = dto.OnWork
            };

            _context.WorkHours.Add(newWorkHour);
        }

        return await _context.SaveChangesAsync();
    }

    public async Task<int> DeleteWorkHoursByIdsAsync(List<int> ids)
    {
        var workHours = await _context.WorkHours
            .Where(w => ids.Contains(w.WorkHourId))
            .ToListAsync();

        _context.WorkHours.RemoveRange(workHours);
        await _context.SaveChangesAsync();

        return workHours.Count;
    }

    public async Task<(int attempted, int deleted, int ignored)> DeleteWorkHoursByFieldsAsync(List<WorkHourDto> workHourDtos)
{
    if (workHourDtos == null || workHourDtos.Count == 0)
        throw new ArgumentException("Input list cannot be null or empty.");

    int deletedCount = 0;
    int attempted = workHourDtos.Count;

    foreach (var dto in workHourDtos)
    {
        // 1. Completely overlaped, delete all this period
        var fullOverlaps = await _context.WorkHours
            .Where(w => w.StaffId == dto.StaffId &&
                        w.Date == dto.Date &&
                        w.StartTime >= dto.StartTime &&
                        w.EndTime <= dto.EndTime)
            .ToListAsync();

        if (fullOverlaps.Any())
        {
            _context.WorkHours.RemoveRange(fullOverlaps);
            deletedCount += fullOverlaps.Count;
        }

        // 2. cut down the middle part, transforming to 2 parts
        var middleCuts = await _context.WorkHours
            .Where(w => w.StaffId == dto.StaffId &&
                        w.Date == dto.Date &&
                        w.StartTime < dto.StartTime &&
                        w.EndTime > dto.EndTime)
            .ToListAsync();

        foreach (var existing in middleCuts)
        {
            var firstPart = new WorkHour
            {
                StaffId = existing.StaffId,
                Date = existing.Date,
                StartTime = existing.StartTime,
                EndTime = dto.StartTime,
                TaskDescription = existing.TaskDescription,
                JobId = existing.JobId,
                OnWork = existing.OnWork
            };

            var secondPart = new WorkHour
            {
                StaffId = existing.StaffId,
                Date = existing.Date,
                StartTime = dto.EndTime,
                EndTime = existing.EndTime,
                TaskDescription = existing.TaskDescription,
                JobId = existing.JobId,
                OnWork = existing.OnWork
            };

            _context.WorkHours.Remove(existing);
            _context.WorkHours.AddRange(firstPart, secondPart);
            deletedCount++;
        }

        // 3. Overlap at top, move the start time
        var headOverlaps = await _context.WorkHours
            .Where(w => w.StaffId == dto.StaffId &&
                        w.Date == dto.Date &&
                        w.StartTime < dto.EndTime &&
                        w.StartTime >= dto.StartTime &&
                        w.EndTime > dto.EndTime)
            .ToListAsync();

        foreach (var existing in headOverlaps)
        {
            existing.StartTime = dto.EndTime;
            _context.Entry(existing).State = EntityState.Modified;
            deletedCount++;
        }

        // 4. Overlap at bottom，change the EndTime
        var tailOverlaps = await _context.WorkHours
            .Where(w => w.StaffId == dto.StaffId &&
                        w.Date == dto.Date &&
                        w.StartTime < dto.StartTime &&
                        w.EndTime > dto.StartTime &&
                        w.EndTime <= dto.EndTime)
            .ToListAsync();

        foreach (var existing in tailOverlaps)
        {
            existing.EndTime = dto.StartTime;
            _context.Entry(existing).State = EntityState.Modified;
            deletedCount++;
        }
    }

    await _context.SaveChangesAsync();
    int ignored = attempted - deletedCount;
    return (attempted, deletedCount, ignored);
}

}
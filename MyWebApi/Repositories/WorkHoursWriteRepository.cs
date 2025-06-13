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

            // Validate: Start < End
            if (dto.EndTime <= dto.StartTime)
            {
                throw new ArgumentException($"Invalid time range: EndTime must be after StartTime. Got StartTime: {dto.StartTime}, EndTime: {dto.EndTime}");
            }

                

            // Get all existing overlaps on same day
            var overlaps = await _context.WorkHours
                .Where(w => w.StaffId == staffId && w.Date == dto.Date &&
                            w.StartTime < dto.EndTime && w.EndTime > dto.StartTime)
                .ToListAsync();

            // Remove overlapping blocks
            if (overlaps.Any())
            {
                _context.WorkHours.RemoveRange(overlaps);
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
    // public async Task<int> SetWorkHoursAsync(int staffId, List<WorkHourDto> workHourDtos)
    // {
    //     foreach (var dto in workHourDtos)
    //     {
    //         dto.StaffId = staffId;

    //         var existing = await _context.WorkHours.FirstOrDefaultAsync(w =>
    //             w.StaffId == staffId &&
    //             w.Date == dto.Date &&
    //             w.StartTime == dto.StartTime);

    //         if (existing == null)
    //         {
    //             var newWorkHour = new WorkHour
    //             {
    //                 StaffId = dto.StaffId,
    //                 Date = dto.Date,
    //                 StartTime = dto.StartTime,
    //                 TaskDescription = dto.TaskDescription,
    //                 JobId = dto.JobId,
    //                 OnWork = dto.OnWork
    //             };
    //             _context.WorkHours.Add(newWorkHour);
    //         }
    //         else
    //         {
    //             existing.TaskDescription = dto.TaskDescription;
    //             existing.JobId = dto.JobId;
    //             existing.OnWork = dto.OnWork;
    //         }
    //     }

    //     await _context.SaveChangesAsync();
    //     return workHourDtos.Count;
    // }

    public async Task<int> DeleteWorkHoursByIdsAsync(List<int> ids)
    {
        var workHours = await _context.WorkHours
            .Where(w => ids.Contains(w.WorkHourId))
            .ToListAsync();

        _context.WorkHours.RemoveRange(workHours);
        await _context.SaveChangesAsync();

        return workHours.Count;
    }

    public async Task<(int attempted, int updated, int ignored)> SoftDeleteByFieldsAsync(JsonElement json)
    {
        if (json.ValueKind != JsonValueKind.Array)
            throw new ArgumentException("Expected a JSON array.");

        int updatedCount = 0;
        int attempted = json.GetArrayLength();

        foreach (var element in json.EnumerateArray())
        {
            int staffId = element.GetProperty("staffId").GetInt32();
            DateOnly date = DateOnly.Parse(element.GetProperty("date").GetString());
            TimeOnly startTime = TimeOnly.Parse(element.GetProperty("startTime").GetString());

            var hours = await _context.WorkHours.Where(w =>
                w.StaffId == staffId &&
                w.Date == date &&
                w.StartTime == startTime &&
                w.OnWork == true).ToListAsync();

            foreach (var hour in hours)
            {
                hour.OnWork = false;
                _context.Entry(hour).State = EntityState.Modified;
                updatedCount++;
            }
        }

        await _context.SaveChangesAsync();
        return (attempted, updatedCount, attempted - updatedCount);
    }

}
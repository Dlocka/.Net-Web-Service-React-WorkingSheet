using Microsoft.EntityFrameworkCore;

public static class General
{
    //To get all work hours having any overlap with given work hours.
    public static async Task<IEnumerable<WorkHour>> GetOverlaps(int staffId, List<WorkHourDto> dtos, AppDbContext _context)
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
}
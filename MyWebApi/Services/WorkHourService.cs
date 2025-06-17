
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
public class WorkHoursService : IWorkHoursService
{
    private readonly IWorkHoursWriteRepository _writer;
    private readonly IWorkHoursReadRepository _reader;

    public WorkHoursService(
        IWorkHoursWriteRepository writer,
        IWorkHoursReadRepository reader)
    {
        _writer = writer;
        _reader = reader;
    }

    public async Task<int> SetWorkHoursAsync(int staffId, List<WorkHourDto> dtos)
    {
        return await _writer.SetWorkHoursAsync(staffId, dtos);
    }

    public async Task<int> DeleteWorkHoursByIdsAsync(List<int> ids)
    {
        return await _writer.DeleteWorkHoursByIdsAsync(ids);
    }



    public async Task<IEnumerable<WorkHour>> GetWorkHoursByStaffIdAsync(int staffId)
    {
        return await _reader.GetWorkHoursByStaffIdAsync(staffId);
    }

    public async Task<(int attempted, int updated, int ignored)> DeleteWorkHoursByFieldsAsync(List<WorkHourDto> dtos)
    {
        var (attempted, updated, ignored) = await _writer.DeleteWorkHoursByFieldsAsync(dtos);
        return (attempted, updated, ignored);
    }

    public async Task<List<WorkHour>> CheckOverlapAsync(int staffId, List<WorkHourDto> dtos)
    {
        var overlaps = await _reader.CheckOverlapAsync(staffId, dtos);
        return overlaps;
    }

    public async Task<IEnumerable<WorkHour>> GetWorkHoursInRange(int staffId, DateOnly startDate, DateOnly endDate)
    {
        var WorkHours = await _reader.GetWorkHoursInRange(staffId, startDate, endDate);
        return WorkHours;
    }

    public Task<double> CalculateRemainingMinutesAsync(int staffId, DateTime now, DateTime end)
    {
        throw new NotImplementedException();
    }

    public async Task<double> GetRemainingWorkMinutesFromNowAsync(int staffId, int JobId, DateTime endDateTime)
    {
        return await _reader.GetRemainingWorkMinutesAsync(staffId, JobId,DateTime.Now, endDateTime);
    }
    public async Task<List<DateOnly>> GetOvertimeDaysAsync(int staffId, DateOnly start, DateOnly end)
    {
        var records = await _reader.GetWorkHoursInRangeAsync(staffId, start, end);

        return records
            .GroupBy(wh => wh.Date)
            .Select(group => new {
                Date = group.Key,
                TotalMinutes = group.Sum(wh =>
                    (wh.EndTime.ToTimeSpan() - wh.StartTime.ToTimeSpan()).TotalMinutes
                )
            })
            .Where(g => g.TotalMinutes > 600) // 10 hours
            .Select(g => g.Date)
            .ToList();
    }

    public async Task<List<OverworkRecordDto>> GetOverworkDaysAsync(double thresholdHours = 10)
    {
        return await _reader.GetOverworkDaysAsync();
    }
}

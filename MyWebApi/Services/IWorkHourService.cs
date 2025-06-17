using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
public interface IWorkHoursService
{
    Task<int> SetWorkHoursAsync(int staffId, List<WorkHourDto> dtos);
    Task<int> DeleteWorkHoursByIdsAsync(List<int> ids);
    Task<(int attempted, int updated, int ignored)> DeleteWorkHoursByFieldsAsync([FromBody] List<WorkHourDto> dtos);
    Task<IEnumerable<WorkHour>> GetWorkHoursByStaffIdAsync(int staffId);
    Task<List<WorkHour>> CheckOverlapAsync(int staffId, List<WorkHourDto> dtos);
    Task<IEnumerable<WorkHour>> GetWorkHoursInRange(int staffId, DateOnly startDate, DateOnly endDate);
    Task<double> CalculateRemainingMinutesAsync(int staffId, DateTime startDateTime, DateTime endDateTime);
    Task<double> GetRemainingWorkMinutesFromNowAsync(int staffId, int jobId, DateTime endDateTime);
    Task<List<DateOnly>> GetOvertimeDaysAsync(int staffId, DateOnly start, DateOnly end);
    Task<List<OverworkRecordDto>> GetOverworkDaysAsync(double thresholdHours = 10);
}
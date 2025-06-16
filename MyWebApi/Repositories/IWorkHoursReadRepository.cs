public interface IWorkHoursReadRepository
{
    Task<IEnumerable<WorkHour>> GetWorkHoursByStaffIdAsync(int staffId);
    Task<List<WorkHour>> CheckOverlapAsync(int staffId, List<WorkHourDto> dtos);
    Task<IEnumerable<WorkHour>> GetWorkHoursInRange(int staffId, DateOnly startDate, DateOnly endDate);
    Task<double> GetRemainingWorkMinutesAsync(int staffId, int jobId, DateTime from, DateTime endDateTime);
    Task<List<WorkHour>> GetWorkHoursInRangeAsync(int staffId, DateOnly start, DateOnly end);
    
}

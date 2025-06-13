public interface IWorkHoursReadRepository
{
    Task<IEnumerable<WorkHour>> GetWorkHoursByStaffIdAsync(int staffId);
    Task<List<WorkHour>> CheckOverlapAsync(int staffId, List<WorkHourDto> dtos);
    
}

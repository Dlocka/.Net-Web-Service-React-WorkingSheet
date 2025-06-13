public interface IWorkHoursReadRepository
{
    Task<IEnumerable<WorkHour>> GetWorkHoursByStaffIdAsync(int staffId);
}

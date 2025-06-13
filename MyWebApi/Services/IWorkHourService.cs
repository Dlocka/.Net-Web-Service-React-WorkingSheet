using System.Text.Json;
public interface IWorkHoursService
{
    Task<int> SetWorkHoursAsync(int staffId, List<WorkHourDto> dtos);
    Task<int> DeleteWorkHoursByIdsAsync(List<int> ids);
    Task<(int attempted, int updated, int ignored)> SoftDeleteByFieldsAsync(JsonElement json);
    Task<IEnumerable<WorkHour>> GetWorkHoursByStaffIdAsync(int staffId);
}
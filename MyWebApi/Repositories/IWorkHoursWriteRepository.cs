using System.Text.Json;

public interface IWorkHoursWriteRepository
{
    Task<int> SetWorkHoursAsync(int staffId, List<WorkHourDto> workHours);
    Task<int> DeleteWorkHoursByIdsAsync(List<int> ids);
    Task<(int attempted, int deleted, int ignored)> DeleteWorkHoursByFieldsAsync(List<WorkHourDto> workHourDtos);
 
}

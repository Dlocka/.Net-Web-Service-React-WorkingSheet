
using System.Text.Json;
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

    public async Task<(int attempted, int updated, int ignored)> SoftDeleteByFieldsAsync(JsonElement json)
    {
        return await _writer.SoftDeleteByFieldsAsync(json);
    }

    public async Task<IEnumerable<WorkHour>> GetWorkHoursByStaffIdAsync(int staffId)
    {
        return await _reader.GetWorkHoursByStaffIdAsync(staffId);
    }
}

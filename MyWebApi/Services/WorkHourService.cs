
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
}

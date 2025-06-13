using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("[controller]")]
public class WorkHourController : ControllerBase
{
    private readonly IWorkHoursService _workHoursService;

    public WorkHourController(IWorkHoursService workHoursService)
    {
        _workHoursService = workHoursService;
    }


    // GET: /WorkHour/staff/5
    [HttpGet("staff/{staffId}")]
    public async Task<ActionResult<IEnumerable<WorkHour>>> GetWorkHoursByStaff(int staffId)
    {
        var workHours = await _workHoursService.GetWorkHoursByStaffIdAsync(staffId);
        return Ok(workHours);
    }

    [HttpPost("hours_set_byStaff/{staffId}")]
    public async Task<IActionResult> SetWorkHours(int staffId, [FromBody] List<WorkHourDto> workHourDtos)
    {
        try
            {
                await _workHoursService.SetWorkHoursAsync(staffId, workHourDtos);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
    }

    [HttpDelete("hours_delete")]
    public async Task<IActionResult> DeleteWorkHours([FromBody] List<int> ids)
    {
        if (ids == null || ids.Count == 0)
            return BadRequest("No IDs provided.");

        int deletedCount = await _workHoursService.DeleteWorkHoursByIdsAsync(ids);
        if (deletedCount == 0)
            return NotFound("No matching work hours found.");

        return Ok(new { deleted = deletedCount });
    }

    [HttpDelete("hours_soft_delete_by_field")]
    public async Task<IActionResult> SoftDeleteByFields([FromBody] JsonElement json)
    {
        if (json.ValueKind != JsonValueKind.Array)
            return BadRequest("Expected a list.");

        var (attempted, updated, ignored) = await _workHoursService.SoftDeleteByFieldsAsync(json);
        return Ok(new { attempted, updated, ignored });
    }
}





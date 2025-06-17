using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("[controller]")]
public class WorkHourController : ControllerBase
{
    private readonly IWorkHoursService _workHoursService;
    private readonly IEmailService _emailService;
    public WorkHourController(IWorkHoursService workHoursService, IEmailService emailService)
    {
        _workHoursService = workHoursService;
        _emailService = emailService;
    }


    // GET: /WorkHour/staff/5
    [HttpGet("staff/{staffId}")]
    public async Task<ActionResult<IEnumerable<WorkHour>>> GetWorkHoursByStaff(int staffId)
    {
        var workHours = await _workHoursService.GetWorkHoursByStaffIdAsync(staffId);
        return Ok(workHours);
    }

    [HttpGet("get_workhours_byrange/{staffId}")]
    public async Task<IActionResult> GetWorkHoursInRange(int staffId, DateOnly startDate, DateOnly endDate)
    {
        // if not explicit the type, response body will return the entire object of 'var'ff, like 
        #region {
        // "result": [ /* ... array of work hours ... */ ],
        // "id": 8,
        // "exception": null,
        // "status": 5,
        // "isCanceled": false,
        // "isCompleted": true,
        // "isCompletedSuccessfully": true,
        // "creationOptions": 0,
        // "asyncState": null,
        // "isFaulted": false
        #endregion
        IEnumerable<WorkHour> WorkHours = await _workHoursService.GetWorkHoursInRange(staffId, startDate, endDate);
        return Ok(WorkHours);

    }

    [HttpGet("remaining_worktime")]
    public async Task<IActionResult> GetRemainingWorkTimeFromNow(int staffId, int JobId, string endDate, string endTime)
    {
        Console.WriteLine(endDate);
        Console.WriteLine(endTime);
        if (!DateOnly.TryParse(endDate, out var parsedDate) ||
                    !TimeOnly.TryParse(endTime, out var parsedTime))
        {
            return BadRequest("Invalid date or time format.");
        }

        var endDateTime = parsedDate.ToDateTime(parsedTime);

        if (DateTime.Now >= endDateTime)
            return BadRequest("End time must be in the future.");

        var remainingMinutes = await _workHoursService.GetRemainingWorkMinutesFromNowAsync(staffId, JobId, endDateTime);
        return Ok(new { remainingMinutes });
    }

    [HttpGet("get_overtime_days/{staffId}")]
    public async Task<IActionResult> GetOvertimeDays(int staffId, [FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate)
    {
        var result = await _workHoursService.GetOvertimeDaysAsync(staffId, startDate, endDate);
        return Ok(result);
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

    [HttpPost("check_overlap")]
    public async Task<IActionResult> CheckOverlap(int staffId, [FromBody] List<WorkHourDto> dtos)
    {
        try
        {
            var overlaps = await _workHoursService.CheckOverlapAsync(staffId, dtos);

            return Ok(new
            {
                hasConflict = overlaps.Any(),
                overlaps
            });
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

    [HttpDelete("hours_delete_by_fields")]
    public async Task<IActionResult> DeleteByFields([FromBody] List<WorkHourDto> dtos)
    {
        var result = await _workHoursService.DeleteWorkHoursByFieldsAsync(dtos);

        return Ok(new
        {
            attempted = result.attempted,
            deleted = result.updated,
            ignored = result.ignored
        });
    }
    
    [HttpPost("overwork-days/email")]
public async Task<IActionResult> GetOverworkDaysAndEmail([FromBody] OverworkEmailRequest request)
{
    var result = await _workHoursService.GetOverworkDaysAsync();

    if (!result.Any())
        return Ok(new { message = "No overwork records found." });

    // Format email content
        var body = "The following staff have worked over 10 hours:\n\n" +
               string.Join("\n", result.Select(r => $"{r.Date}: {r.StaffName}"));

    await _emailService.SendEmailAsync(request.Email, "Overwork Notification", body);
        Console.WriteLine("email service has worked");
    return Ok(new { message = "Email sent successfully." });

}
    
    
}






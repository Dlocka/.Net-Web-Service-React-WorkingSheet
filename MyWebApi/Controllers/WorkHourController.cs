using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
[ApiController]
[Route("[controller]")]
public class WorkHourController : ControllerBase
{
    private readonly AppDbContext _context;

    public WorkHourController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /WorkHour/staff/5
    [HttpGet("staff/{staffId}")]
    public async Task<ActionResult<IEnumerable<WorkHour>>> GetWorkHoursByStaff(int staffId)
    {
        var workHours = await _context.WorkHours
            .Where(wh => wh.StaffId == staffId)
            .ToListAsync();

        return Ok(workHours);
    }

    // // POST: /WorkHour
    // [HttpPost]
    // public async Task<ActionResult<WorkHour>> AddWorkHour(WorkHour workHour)
    // {

    //     if (!_context.Staffs.Any(s => s.StaffId == workHour.StaffId))
    //         return NotFound($"Staff with ID {workHour.StaffId} not found.");

    //     _context.WorkHours.Add(workHour);
    //     await _context.SaveChangesAsync();
    //     return CreatedAtAction(nameof(GetByStaff), new { staffId = workHour.StaffId }, workHour);
    // }

    [HttpPost("hours_set_byStaff/{staffid}")]
    public async Task<ActionResult> SetWorkHours(int staffId, [FromBody] List<WorkHour> workHours)
    {
        if (workHours == null || workHours.Count == 0)
        return BadRequest("Work hours list cannot be empty.");

    foreach (var workhour in workHours)
    {
        workhour.StaffId = staffId; // assign staffId from URL param

            // id 0 to represent the empty hour to be occupied now
            if (workhour.WorkHourId == 0)
            {
                // New record: Add it
                _context.WorkHours.Add(workhour);
            }
            else
            {
                // Existing record: Update it
                // Option A: Attach and mark modified
                _context.WorkHours.Attach(workhour);
                _context.Entry(workhour).State = EntityState.Modified;
            }
    }

    await _context.SaveChangesAsync();

    return Ok(new { count = workHours.Count });
    }
    // DELETE: /WorkHour/5
    // [HttpDelete("{id}")]
    // public async Task<IActionResult> DeleteWorkHour(int id)
    // {
    //     var workHour = await _context.WorkHours.FindAsync(id);
    //     if (workHour == null)
    //     {
    //         return NotFound();
    //     }

    //     _context.WorkHours.Remove(workHour);
    //     await _context.SaveChangesAsync();

    //     return NoContent();
    // }
    [HttpDelete("hours_delete")]
    public async Task<IActionResult> DeleteWorkHours([FromBody] List<int> ids)
    {
        if (ids == null || ids.Count == 0)
            return BadRequest("No IDs provided.");

        var workHours = await _context.WorkHours
            .Where(w => ids.Contains(w.WorkHourId))
            .ToListAsync();

        if (workHours.Count == 0)
            return NotFound("No matching work hours found.");

        _context.WorkHours.RemoveRange(workHours);
        await _context.SaveChangesAsync();

        return Ok(new { deleted = workHours.Count });
    }
}
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class StaffController : ControllerBase
{
    private readonly IStaffWriter _staffWriter;
    private readonly IStaffReader _staffReader;
    public StaffController(IStaffWriter staffWriter, IStaffReader staffReader)
    {
        _staffWriter = staffWriter;
        _staffReader = staffReader;
    }

    [HttpPost("create")]
    public IActionResult CreateStaff([FromBody] string name)
    {
        _staffWriter.AddStaff(name);
        return Ok(new { message = $"Staff {name} created." });
    }

    [HttpGet("{id}")]
    public IActionResult GetStaff(int id)
    {
        Console.WriteLine("StaffController:Lets get Staff");
        var staff = _staffReader.GetStaff(id); // or inject reader separately
        return staff == null ? NotFound() : Ok(staff);
    }

    [HttpGet()]
    public IActionResult GetAllStaff()
    {
        IEnumerable<Staff> staffList = _staffReader.GetAll();
        return staffList == null ? NotFound() : Ok(staffList);
    }

    [HttpDelete("{id}",Name = "DeleteStaffById")]
    public IActionResult DeleteStaff(int id)
    {
        bool deleted = _staffWriter.DeleteStaff(id);
    if (deleted)
    {
        return Ok(new { message = $"Staff with id {id} deleted." });
    }
    else
    {
        return NotFound(new { message = $"Staff with id {id} not found." });
    }
    }
}

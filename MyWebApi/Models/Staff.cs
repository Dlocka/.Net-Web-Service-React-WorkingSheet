public class Staff
{
    public int StaffId { get; set; }
    public required string Name { get; set; }

    public List<WorkHour> WorkHours { get; set; } = new();
}
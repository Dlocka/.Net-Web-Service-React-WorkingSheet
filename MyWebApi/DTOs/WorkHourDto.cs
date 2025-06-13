public class WorkHourDto
{
    public int StaffId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string? TaskDescription { get; set; }
    public int? JobId { get; set; }
    public bool OnWork { get; set; } = true;
}
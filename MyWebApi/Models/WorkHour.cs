public class WorkHour
{
    public int WorkHourId { get; set; }
    public int StaffId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string? TaskDescription { get; set; }

    public required Staff Staff { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class WorkHour
{
    public int WorkHourId { get; set; }
    public int StaffId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public string? TaskDescription { get; set; }
    public int? JobId { get; set; }

    public Job? Job{ get; set; }
    public bool OnWork { get; set; } = true;
    [ForeignKey("StaffId")]
    public required Staff Staff { get; set; }    
}
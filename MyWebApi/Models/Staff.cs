using System.Text.Json.Serialization;

public class Staff
{
    public int StaffId { get; set; }
    public required string Name { get; set; }

    [JsonIgnore]
    public List<WorkHour> WorkHours { get; set; } = new();
}
public interface IStaffWriter
{
    void AddStaff(string name);
    void UpdateStaff(Staff staff);
    bool DeleteStaff(int id);
}
public interface IStaffWriter
{
    void AddStaff(string name);
    void UpdateStaff(Staff staff);
    void DeleteStaff(int id);
}
public interface IStaffReader
{
    Staff? GetStaff(int id);

    public IEnumerable<Staff> GetAll();
 }

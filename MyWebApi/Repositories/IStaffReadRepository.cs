public interface IStaffReadRepository
{
    Staff GetById(int id);
    IEnumerable<Staff> GetAll();
}
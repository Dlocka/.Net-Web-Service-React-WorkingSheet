public interface IUserReadRepository
{
    Staff GetById(int id);
    IEnumerable<Staff> GetAll();
}
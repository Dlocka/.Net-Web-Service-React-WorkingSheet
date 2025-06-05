public interface IUserWriteRepository
{
    void Add(Staff staff);
    void Update(Staff staff);
    void Delete(int id);
}
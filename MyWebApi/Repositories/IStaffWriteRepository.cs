public interface IStaffWriteRepository
{
    void Add(Staff staff);
    void Update(Staff staff);
    void Delete(int id);
}
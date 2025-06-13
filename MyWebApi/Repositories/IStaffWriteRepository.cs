public interface IStaffWriteRepository
{
    void Add(Staff staff);
    void Update(Staff staff);
    bool Delete(int id);
}
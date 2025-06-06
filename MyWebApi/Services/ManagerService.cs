
public class ManagerService : IStaffWriter,IStaffReader
{
    private readonly IStaffReadRepository _StaffReadRepository;
    private readonly IStaffWriteRepository _StaffWriterRepository;
    public ManagerService(IStaffReadRepository StaffRepository,IStaffWriteRepository StaffWriteRepository)
    {
        _StaffReadRepository = StaffRepository;
        _StaffWriterRepository=StaffWriteRepository;
    }

    public void AddStaff(string name)
    {
        var staff = new Staff { Name = name };
        _StaffWriterRepository.Add(staff);
    }

    public void DeleteStaff(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Staff> GetAll()
    {
        return _StaffReadRepository.GetAll();
    }

    public Staff? GetStaff(int id)
    {
        Console.WriteLine("Manager Service: get stuff");
        return _StaffReadRepository.GetById(id);
    }

    public void UpdateStaff(Staff staff)
    {
        throw new NotImplementedException();
    }
}


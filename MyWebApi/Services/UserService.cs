public class UserService : IStaffWriter,IStaffReader
{
    private readonly IUserReadRepository _userReadRepository;
    private readonly IUserWriteRepository _userWriterRepository;
    public UserService(IUserReadRepository userRepository,IUserWriteRepository userWriteRepository)
    {
        _userReadRepository = userRepository;
        _userWriterRepository=userWriteRepository;
    }

    public void AddStaff(string name)
    {
        var staff = new Staff { Name = name };
        _userWriterRepository.Add(staff);
    }

    public void DeleteStaff(int id)
    {
        throw new NotImplementedException();
    }

    public Staff? GetStaff(int id)
    {
        return _userReadRepository.GetById(id);
    }

    public void UpdateStaff(Staff staff)
    {
        throw new NotImplementedException();
    }
}


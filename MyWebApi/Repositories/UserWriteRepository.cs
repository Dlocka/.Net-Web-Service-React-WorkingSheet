public class UserWriteRepository : IUserWriteRepository
{
    private readonly AppDbContext _context;

    public UserWriteRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Add(Staff staff)
    {
        _context.Staffs.Add(staff);
        _context.SaveChanges(); // persist
    }

    public void Update(Staff staff){

    }
    
    public void Delete(int id){
        
    }

}
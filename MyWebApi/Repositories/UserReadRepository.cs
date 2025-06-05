using Microsoft.EntityFrameworkCore;
public class UserReadRepository : IUserReadRepository
{
    private readonly AppDbContext _context;

    public UserReadRepository(AppDbContext context)
    {
        _context = context;
    }

    public Staff? GetById(int id)
    {
        return _context.Staffs.Find(id);
    }


    public IEnumerable<Staff> GetAll(){
        return _context.Staffs.AsNoTracking().ToList();
    }
}

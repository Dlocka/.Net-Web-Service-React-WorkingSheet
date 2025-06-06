using Microsoft.EntityFrameworkCore;
public class StaffReadRepository : IStaffReadRepository
{
    private readonly AppDbContext _context;

    public StaffReadRepository(AppDbContext context)
    {
        _context = context;
    }

    public Staff? GetById(int id)
    {
        Console.WriteLine("DAL: let's get Staff");
        return _context.Staffs.Find(id);
    }


    public IEnumerable<Staff> GetAll(){
        return _context.Staffs.AsNoTracking().ToList();
    }
}

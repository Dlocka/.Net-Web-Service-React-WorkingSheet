public class StaffWriteRepository : IStaffWriteRepository
{
    private readonly AppDbContext _context;

    public StaffWriteRepository(AppDbContext context)
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
    
    public bool Delete(int id){
        var staff = _context.Staffs.Find(id);
    if (staff == null)
        return false;

    _context.Staffs.Remove(staff);
    _context.SaveChanges();
    return true;
    }

}
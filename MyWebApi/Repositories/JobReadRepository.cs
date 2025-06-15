using Microsoft.EntityFrameworkCore;
public class JobReadRepository : IJobReadRepository
{
    private readonly AppDbContext _context;
    public JobReadRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Job>> GetAllJobs()
    {
        return await _context.Jobs.ToListAsync();
    }
}
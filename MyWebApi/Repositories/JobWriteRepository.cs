public class JobWriteRepository : IJobWriteRepository
{
    private readonly AppDbContext _context;

    public JobWriteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Job> CreateJobAsync(Job job)
    {
        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();
        return job;
    }
}
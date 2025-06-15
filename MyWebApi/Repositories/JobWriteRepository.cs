public class JobWriteRepository : IJobWriteRepository
{
    private readonly AppDbContext _context;

    public JobWriteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Job> CreateJobAsync(JobCreateDto jobDto)
    {
        var job = new Job
        {
            Name = jobDto.Name
            // Id is not set here; EF Core generates it on SaveChanges
        };

        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();

        return job;
    }
}
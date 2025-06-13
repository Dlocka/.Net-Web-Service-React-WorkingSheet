public class JobService : IJobService
{
    private readonly IJobWriteRepository _jobWriteRepository;

    public JobService(IJobWriteRepository jobWriteRepository)
    {
        _jobWriteRepository = jobWriteRepository;
    }

    public async Task<Job> CreateJobAsync(Job job)
    {
        // Here you can add any extra validation/business logic before creating
        return await _jobWriteRepository.CreateJobAsync(job);
    }
}
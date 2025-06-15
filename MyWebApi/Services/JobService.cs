
public class JobService : IJobService
{
    private readonly IJobWriteRepository _jobWriteRepository;
    private readonly IJobReadRepository _jobReadRepository;
    public JobService(IJobWriteRepository jobWriteRepository, IJobReadRepository jobReadRepository)
    {
        _jobWriteRepository = jobWriteRepository;
        _jobReadRepository = jobReadRepository;
    }

     public async Task<Job> CreateJobAsync(JobCreateDto createJobDto)
    {
       return await _jobWriteRepository.CreateJobAsync(createJobDto);
    }

    public async Task<IEnumerable<Job>> GetAllJobs()
    {
        return await _jobReadRepository.GetAllJobs();
    }
}
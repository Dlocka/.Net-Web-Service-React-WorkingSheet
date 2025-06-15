public interface IJobService
{
    Task<Job> CreateJobAsync(JobCreateDto createJobDto);
    Task<IEnumerable<Job>> GetAllJobs();
}
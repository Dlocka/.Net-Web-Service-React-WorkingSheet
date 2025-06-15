public interface IJobWriteRepository
{
    Task<Job> CreateJobAsync(JobCreateDto job);
}
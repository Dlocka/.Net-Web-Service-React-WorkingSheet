public interface IJobWriteRepository
{
    Task<Job> CreateJobAsync(Job job);
}
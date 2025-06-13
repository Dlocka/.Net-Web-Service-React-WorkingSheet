public interface IJobService
{
    Task<Job> CreateJobAsync(Job job);
}
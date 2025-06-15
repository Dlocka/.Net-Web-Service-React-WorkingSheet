public interface IJobReadRepository
{
 Task<IEnumerable<Job> > GetAllJobs();

}
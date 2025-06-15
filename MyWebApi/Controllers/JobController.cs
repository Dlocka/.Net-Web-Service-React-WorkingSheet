using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[Controller]")]
public class JobController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobController(IJobService jobService)
    {
        _jobService = jobService;
    }

    

    [HttpPost("create")] // Example route
    public async Task<IActionResult> CreateJob([FromBody] JobCreateDto createJobDto)
    {

        if (string.IsNullOrWhiteSpace(createJobDto.Name))
        {
            return BadRequest("Job name is required.");
        }
        // The service handles mapping CreateJobDto to Job entity and saving
        Job createdJob = await _jobService.CreateJobAsync(createJobDto);
        return Ok(createdJob);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetJobById(int id)
    {
        // Placeholder - implement this if you want
        return Ok();
    }
    [HttpGet("get_jobs_all")]

    public async Task<IActionResult> GetAllJobs()
    {

        IEnumerable<Job> jobs = await _jobService.GetAllJobs();

        // Return the jobs with an OK (200) status
        return Ok(jobs);
    }

}
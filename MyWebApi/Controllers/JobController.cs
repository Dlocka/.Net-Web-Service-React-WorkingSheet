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

    [HttpPost("create")]
    public async Task<IActionResult> CreateJob([FromBody] Job job)
    {
        if (string.IsNullOrWhiteSpace(job.Name))
        {
            return BadRequest("Job name is required.");
        }

        var createdJob = await _jobService.CreateJobAsync(job);
         return CreatedAtAction(nameof(GetJobById), new { id = createdJob.JobId }, createdJob);
        
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetJobById(int id)
    {
        // Placeholder - implement this if you want
        return Ok();
    }
}
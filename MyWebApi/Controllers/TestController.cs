using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult GetHello()
    {
        return Ok(new { message = "Hello from .Net Web Api Test" });
    }
    [HttpPut("putTest")]
    public IActionResult PutTest()
    {
        return Ok(new{message="This is PUT"});
    }
}
using Microsoft.AspNetCore.Mvc;

namespace MakeSport.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TestBaseController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> TestGet()
    {
        await Task.Delay(100);
        return Ok("Hello World");
    }
}
using Microsoft.AspNetCore.Mvc;

namespace NetClinic.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OopsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        throw new Exception("Expected: controller used to showcase what happens when an exception is thrown");
    }
}
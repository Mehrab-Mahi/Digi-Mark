using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.User;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    [HttpPost]
    public ActionResult SignUp()
    {
        return Ok("okay");
    }
}
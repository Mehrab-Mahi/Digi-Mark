using Domain.Models.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Auth;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost]
    public ActionResult Login(LoginModel loginModel)
    {
        return Ok("okay");
    }
}
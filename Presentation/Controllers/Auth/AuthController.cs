using Application.Interfaces;
using Domain.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [AllowAnonymous]
    [HttpPost]
    public ActionResult Login(LoginModel loginModel)
    {
        var response = _authService.Login(loginModel);
        return Ok(new {data = response});
    }
}
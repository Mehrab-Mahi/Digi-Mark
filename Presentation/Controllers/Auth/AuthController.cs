using Application.Interfaces;
using Domain.DTO.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost]
    public ActionResult Login(LoginDto login)
    {
        var response = _authService.Login(login);
        return Ok(new {data = response});
    }
}
using Application.Interfaces;
using Domain.DTO.User;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.User;

[ApiController]
[Route("[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpPost]
    public ActionResult SignUp(SignUpDto signUpDto)
    {
        var response = _userService.SignUp(signUpDto);
        return Ok(new {data = response});
    }
}
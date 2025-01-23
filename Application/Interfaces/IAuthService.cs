using Domain.Models.Auth;
using Domain.Models.Common;

namespace Application.Interfaces;

public interface IAuthService
{
    PayloadResponse Login(LoginModel loginModel);
}
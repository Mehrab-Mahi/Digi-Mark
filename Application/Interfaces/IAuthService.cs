using Domain.DTO.Auth;
using Domain.Models.Common;

namespace Application.Interfaces;

public interface IAuthService
{
    PayloadResponse Login(LoginDto login);
}
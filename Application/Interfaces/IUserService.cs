using Domain.DTO.User;
using Domain.Models.Common;

namespace Application.Interfaces;

public interface IUserService
{
    PayloadResponse SignUp(SignUpDto signUpDto);
}
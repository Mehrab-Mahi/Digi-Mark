using Application.Interfaces;
using Domain.DTO.Auth;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Models;
using Domain.Models.Common;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<User> _userRepository;
        private readonly AppSettings _appSettings;

        public AuthService(IRepository<User> userRepository,
            IOptions<AppSettings> appSettings)
        {
            _userRepository = userRepository;
            _appSettings = appSettings.Value;
        }

        public PayloadResponse Login(LoginDto loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.UserName) || string.IsNullOrEmpty(loginDto.Password))
            {
                return ReturnPayloadForEmptyValue(loginDto);
            }

            var user = _userRepository
                .GetConditional(u => u.UserName == loginDto.UserName)
                .FirstOrDefault();

            if (!IfUserExist(user))
            {
                return new PayloadResponse()
                {
                    IsSuccess = false,
                    PayloadType = "Authentication",
                    Message = $"User with '{loginDto.UserName}' username does not exist!"
                };
            }

            if (!user!.IsActive)
            {
                return new PayloadResponse()
                {
                    IsSuccess = false,
                    PayloadType = "Authentication",
                    Message = "User is not active!"
                };
            }

            var isVerified = VerifyPassword(loginDto.Password, user.Password);

            if (!isVerified)
            {
                return new PayloadResponse
                {
                    IsSuccess = false,
                    PayloadType = "Authentication",
                    Content = null,
                    Message = "Authentication unsuccessful. Password does not match!"
                };
            }

            var accessToken = GenerateToken(user);

            return new PayloadResponse()
            {
                IsSuccess = true,
                PayloadType = "Authentication",
                Content = accessToken,
                Message = "Authentication successful!"
            };
        }

        private string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.Name, ($"{user.FirstName} {user.MiddleName} {user.LastName}")),
                    new(type: "UserId", user.Id),
                    new(type: "IsSuperAdmin", user.IsSystemAdmin.ToString()),
                    new(type: "UserType", user.UserName),
                    new(type: "Email", user.Email)
                }),
                Expires = DateTime.UtcNow.AddMinutes(20),
                SigningCredentials = credentials
            };

            var tokenValue = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(tokenValue);

            return token;
        }

        private static bool VerifyPassword(string loginModelPassword, string userPassword)
        {
            return BCrypt.Net.BCrypt.Verify(loginModelPassword, userPassword);
        }

        private static PayloadResponse ReturnPayloadForEmptyValue(LoginDto loginModel)
        {
            var message = string.Empty;

            if (string.IsNullOrEmpty(loginModel.UserName))
            {
                message = "Please input the username!";
            }

            if (string.IsNullOrEmpty(loginModel.Password))
            {
                message = "Please input the password!";
            }

            return new PayloadResponse()
            {
                IsSuccess = false,
                Message = message
            };
        }

        private static bool IfUserExist(User? user)
        {
            return user is not null;
        }
    }
}

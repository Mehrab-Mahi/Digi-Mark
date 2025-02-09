using Application.Interfaces;
using Domain.DTO.User;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Models.Common;

namespace Application.Services
{
    public class UserService(IRepository<User> userRepository) : IUserService
    {
        private readonly IRepository<User> _userRepository = userRepository;

        public PayloadResponse SignUp(SignUpDto signUpDto)
        {
            var response = IfRequiredFieldsFilledUp(signUpDto);

            if (!response.IsSuccess)
            {
                return response;
            }

            var user = new User()
            {
                FirstName = signUpDto.FirstName,
                LastName = signUpDto.LastName,
                MiddleName = signUpDto.MiddleName,
                UserName = signUpDto.UserName,
                Email = signUpDto.Email,
                Password = GeneratePassword(signUpDto.Password),
                IsActive = true
            };

            _userRepository.Insert(user);
            _userRepository.SaveChanges();

            return new PayloadResponse()
            {
                IsSuccess = true,
                Message = "Sign up has been successful!",
                PayloadType = "User signup"
            };
        }

        private static string GeneratePassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        private PayloadResponse IfRequiredFieldsFilledUp(SignUpDto signUpDto)
        {
            var message = string.Empty;

            if (string.IsNullOrEmpty(signUpDto.Email))
            {
                message = "Please enter your email!";
            }
            else if (string.IsNullOrEmpty(signUpDto.UserName))
            {
                message = "Please enter your username!";
            }
            else if (string.IsNullOrEmpty(signUpDto.Password))
            {
                message = "Please enter your password!";
            }
            else if (string.IsNullOrEmpty(signUpDto.ConfirmPassword))
            {
                message = "Please enter your confirm password!";
            }
            else if (signUpDto.Password != signUpDto.ConfirmPassword)
            {
                message = "Password and confirm password doesn't match!";
            }
            else if (IsUserWithThisUsernameExist(signUpDto.UserName))
            {
                message = "User with this username is already exist! Try another username please.";
            }
            else if (IsUserWithThisEmailExist(signUpDto.Email))
            {
                message = "User with this email is already exist! Try another email please.";
            }

            if (string.IsNullOrEmpty(message))
            {
                return new PayloadResponse()
                {
                    IsSuccess = true
                };
            }

            return new PayloadResponse()
            {
                IsSuccess = false,
                Message = message,
                PayloadType = "User signup"
            };
        }

        private bool IsUserWithThisEmailExist(string email)
        {
            var user = _userRepository
                .GetConditional(u => u.Email == email)
                .FirstOrDefault();

            return user is not null;
        }

        private bool IsUserWithThisUsernameExist(string userName)
        {
            var user = _userRepository
                .GetConditional(u => u.UserName == userName)
                .FirstOrDefault();

            return user is not null;
        }
    }
}

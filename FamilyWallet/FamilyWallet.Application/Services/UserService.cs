using FamilyWallet.Application.Repositories.Interfaces;
using FamilyWallet.Application.Services.Interfaces;
using FamilyWallet.Domain.DTOs;
using FamilyWallet.Domain.Models;
using System;
using BCrypt.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace FamilyWallet.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;


        public UserService(IUserRepository userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _config = config;
        }

        public async Task<ServiceResponse<ICollection<UserDto>>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            if (!users.Any())
            {
                return new ServiceResponse<ICollection<UserDto>> { Message = "No users found" };
            }
            var usersDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Balance = u.Balance,
            }).ToList();

            return new ServiceResponse<ICollection<UserDto>> { Success = true, Data = usersDtos };

        }

        public async Task<ServiceResponse<UserDto>> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new ServiceResponse<UserDto> { Message = "User not found" };
            }
            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Balance = user.Balance,
            };
            return new ServiceResponse<UserDto> { Success = true, Data = userDto };
        }

        public async Task<ServiceResponse> RegisterUserAsync(UserDto userDto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(userDto.Email);
            if (existingUser != null)
            {
                return new ServiceResponse { Message = "User with this email already exists", Success = false };
            }

            User newUser = new User()
            {
                Name = userDto.Name,
                Email = userDto.Email,
                PasswordHash = HashPassword(userDto.Password),
                Balance = userDto.Balance,
                Role = "User",
            };

            await _userRepository.AddAsync(newUser);
            return new ServiceResponse { Message = "User registered successfully", Success = true };
        }

        public async Task<ServiceResponse<string>> LoginAsync(UserDto userDto)
        {
            var user = await _userRepository.GetByEmailAsync(userDto.Email);
            if (user == null)
            {
                return new ServiceResponse<string> { Message = "User not found", Success = false };
            }
            if (!BCrypt.Net.BCrypt.Verify(userDto.Password, user.PasswordHash))
            {
                return new ServiceResponse<string> { Message = "Invalid credentials", Success = false };
            }
            string token = GenerateJwtToken(user);
            return new ServiceResponse<string> { Message = "Login successful", Success = true, Data = token };
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            //new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("userId", user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

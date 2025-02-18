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

namespace FamilyWallet.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository
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
                Id = userDto.Id,
                Name = userDto.Name,
                Email = userDto.Email,
                PasswordHash = HashPassword(userDto.Password),
                Balance = userDto.Balance,
                Role = "User",
            };

            await _userRepository.AddAsync(newUser);
            return new ServiceResponse { Message = "User registered successfully", Success = true };
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}

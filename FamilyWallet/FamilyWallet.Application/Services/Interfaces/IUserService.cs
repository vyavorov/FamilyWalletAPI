using FamilyWallet.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyWallet.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResponse> RegisterUserAsync(UserDto userDto);
        Task<ServiceResponse<UserDto>> GetUserByIdAsync(int userId);
        Task<ServiceResponse<ICollection<UserDto>>> GetAllUsersAsync();
    }
}

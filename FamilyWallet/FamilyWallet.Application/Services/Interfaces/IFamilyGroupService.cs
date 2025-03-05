using FamilyWallet.Domain.DTOs;
using FamilyWallet.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyWallet.Application.Services.Interfaces
{
    public interface IFamilyGroupService
    {
        Task<ServiceResponse<FamilyGroup>> CreateFamilyGroupAsync(string name);
        Task<ServiceResponse> AddUserToFamilyGroupAsync(int familyGroupId, int userId);
        Task<ServiceResponse> RemoveUserFromFamilyGroupAsync(int familyGroupId, int userId);
        Task<ServiceResponse> DeleteFamilyGroupAsync(int familyGroupId);
        Task<ServiceResponse<FamilyGroup>> UpdateFamilyGroupAsync(int familyGroupId, string name);
        Task<ServiceResponse<FamilyGroup>> GetFamilyGroupByIdAsync(int familyGroupId);
        Task<ServiceResponse<FamilyGroup>> GetFamilyGroupByNameAsync(string name);
        Task<ServiceResponse<IEnumerable<FamilyGroup>>> GetAllFamilyGroupsAsync();
        Task<ServiceResponse<IEnumerable<UserDto>>> GetUsersInFamilyGroupAsync(int familyGroupId);
    }
}

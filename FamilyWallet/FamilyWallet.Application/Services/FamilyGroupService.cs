using FamilyWallet.Application.Repositories.Interfaces;
using FamilyWallet.Application.Services.Interfaces;
using FamilyWallet.Domain.DTOs;
using FamilyWallet.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyWallet.Application.Services
{
    public class FamilyGroupService : IFamilyGroupService
    {
        private readonly IFamilyGroupRepository _familyGroupRepository;
        private readonly IUserRepository _userRepository;


        public FamilyGroupService(IFamilyGroupRepository familyGroupRepository, IUserRepository userRepository)
        {
            _familyGroupRepository = familyGroupRepository;
            _userRepository = userRepository;
        }

        public async Task<ServiceResponse> AddUserToFamilyGroupAsync(int familyGroupId, int userId)
        {
            var familyGroup = await _familyGroupRepository.GetByIdAsync(familyGroupId);
            if (familyGroup == null)
            {
                return new ServiceResponse { Message = "Family group not found" };
            }
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new ServiceResponse { Message = "User not found" };
            }
            if (familyGroup.Members.Any(m => m.Id == userId))
            {
                return new ServiceResponse { Message = "User already in family group" };
            }
            familyGroup.Members.Add(user);
            await _familyGroupRepository.UpdateAsync(familyGroup);
            return new ServiceResponse { Success = true, Message="User successfuly added"};

        }

        public async Task<ServiceResponse<FamilyGroup>> CreateFamilyGroupAsync(string name)
        {
            var familyGroup = await _familyGroupRepository.GetByNameAsync(name);
            if (familyGroup != null)
            {
                return new ServiceResponse<FamilyGroup> { Message = "Family group with this name already exists" };
            }
            familyGroup = new FamilyGroup { Name = name, Balance = 0 };
            await _familyGroupRepository.AddAsync(familyGroup);
            return new ServiceResponse<FamilyGroup> { Success = true, Message = "Family group created", Data = familyGroup };
        }

        public async Task<ServiceResponse> DeleteFamilyGroupAsync(int familyGroupId)
        {
            var familyGroup = await _familyGroupRepository.GetByIdAsync(familyGroupId);
            if (familyGroup == null)
            {
                return new ServiceResponse { Message = "Family group not found" };
            }
            await _familyGroupRepository.DeleteAsync(familyGroupId);
            return new ServiceResponse { Success = true, Message = "Family group deleted" };
        }

        public async Task<ServiceResponse<IEnumerable<FamilyGroup>>> GetAllFamilyGroupsAsync()
        {
            var familyGroups = await _familyGroupRepository.GetAllAsync();
            if (!familyGroups.Any())
            {
                return new ServiceResponse<IEnumerable<FamilyGroup>> { Message = "No family groups found" };
            }
            return new ServiceResponse<IEnumerable<FamilyGroup>> { Success = true, Data = familyGroups };
        }

        public async Task<ServiceResponse<FamilyGroup>> GetFamilyGroupByIdAsync(int familyGroupId)
        {
            var familyGroup = await _familyGroupRepository.GetByIdAsync(familyGroupId);
            if (familyGroup == null)
            {
                return new ServiceResponse<FamilyGroup> { Message = "Family group not found" };
            }
            return new ServiceResponse<FamilyGroup> { Success = true, Data = familyGroup };
        }

        public async Task<ServiceResponse<FamilyGroup>> GetFamilyGroupByNameAsync(string name)
        {
            var familyGroup = await _familyGroupRepository.GetByNameAsync(name);
            if (familyGroup == null)
            {
                return new ServiceResponse<FamilyGroup> { Message = "Family group not found" };
            }
            return new ServiceResponse<FamilyGroup> { Success = true, Message = "Family group found", Data=familyGroup };
        }

        public async Task<ServiceResponse<IEnumerable<UserDto>>> GetUsersInFamilyGroupAsync(int familyGroupId)
        {
            var familyGroup = await _familyGroupRepository.GetByIdAsync(familyGroupId);
            if (familyGroup == null)
            {
                return new ServiceResponse<IEnumerable<UserDto>> { Message = "Family group not found" };
            }
            var users = familyGroup.Members.Select(m => new UserDto { Id = m.Id, Name = m.Name });
            return new ServiceResponse<IEnumerable<UserDto>> { Success = true, Data = users };
        }

        public async Task<ServiceResponse> RemoveUserFromFamilyGroupAsync(int familyGroupId, int userId)
        {
            var familyGroup = await _familyGroupRepository.GetByIdAsync(familyGroupId);
            if (familyGroup == null)
            {
                return new ServiceResponse { Message = "Family group not found" };
            }
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new ServiceResponse { Message = "User not found" };
            }
            if (!familyGroup.Members.Any(m => m.Id == userId))
            {
                return new ServiceResponse { Message = "User not in family group" };
            }
            familyGroup.Members.Remove(user);
            await _familyGroupRepository.UpdateAsync(familyGroup);
            return new ServiceResponse { Success = true, Message = "User successfuly removed" };
        }

        public async Task<ServiceResponse<FamilyGroup>> UpdateFamilyGroupAsync(int familyGroupId, string name)
        {
            var familyGroup = await _familyGroupRepository.GetByIdAsync(familyGroupId);
            if (familyGroup == null)
            {
                return new ServiceResponse<FamilyGroup> { Message = "Family group not found" };
            }
            familyGroup.Name = name;
            await _familyGroupRepository.UpdateAsync(familyGroup);
            return new ServiceResponse<FamilyGroup> { Success = true, Message = "Family group updated", Data = familyGroup };
        }
    }
}

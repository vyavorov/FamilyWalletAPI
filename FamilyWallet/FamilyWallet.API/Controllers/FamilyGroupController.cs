using FamilyWallet.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace FamilyWallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FamilyGroupController : ControllerBase
    {
        private readonly IFamilyGroupService _familyGroupService;

        public FamilyGroupController(IFamilyGroupService familyGroupService)
        {
            _familyGroupService = familyGroupService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFamilyGroupAsync([FromBody] string name)
        {
            var response = await _familyGroupService.CreateFamilyGroupAsync(name);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("{familyGroupId}/addUser/{userId}")]
        public async Task<IActionResult> AddUserToFamilyGroupAsync(int familyGroupId, int userId)
        {
            var response = await _familyGroupService.AddUserToFamilyGroupAsync(familyGroupId, userId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("{familyGroupId}")]
        public async Task<IActionResult> DeleteFamilyGroupAsync(int familyGroupId)
        {
            var response = await _familyGroupService.DeleteFamilyGroupAsync(familyGroupId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("{familyGroupId}")]
        public async Task<IActionResult> UpdateFamilyGroupAsync(int familyGroupId, [FromBody] string name)
        {
            var response = await _familyGroupService.UpdateFamilyGroupAsync(familyGroupId, name);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        // GET: api/FamilyGroup/{id}
        [HttpGet("id/{familyGroupId}")]
        public async Task<IActionResult> GetFamilyGroupByIdAsync(int familyGroupId)
        {
            var response = await _familyGroupService.GetFamilyGroupByIdAsync(familyGroupId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        // GET: api/FamilyGroup/name/{name}
        [HttpGet("name/{familyGroupName}")]
        public async Task<IActionResult> GetFamilyGroupByNameAsync(string familyGroupName)
        {
            var response = await _familyGroupService.GetFamilyGroupByNameAsync(familyGroupName);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        // GET: api/FamilyGroup/All
        [HttpGet("All")]
        public async Task<IActionResult> GetAllFamilyGroupsAsync()
        {
            var response = await _familyGroupService.GetAllFamilyGroupsAsync();
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        // GET: api/FamilyGroup/{id}/users
        [HttpGet("{familyGroupId}/users")]
        public async Task<IActionResult> GetUsersInFamilyGroupAsync(int familyGroupId)
        {
            var response = await _familyGroupService.GetUsersInFamilyGroupAsync(familyGroupId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("{familyGroupId}/removeUser/{userId}")]
        public async Task<IActionResult> RemoveUserFromFamilyGroupAsync(int familyGroupId, int userId)
        {
            var response = await _familyGroupService.RemoveUserFromFamilyGroupAsync(familyGroupId, userId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}

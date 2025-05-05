using FamilyWallet.Domain.DTOs;
using FamilyWallet.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyWallet.Application.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<ServiceResponse> AddCategoryAsync(CategoryDto categoryDto);
        Task<ServiceResponse<IEnumerable<CategoryDto>>> GetCategoriesByUserAsync(int userId);
        Task<ServiceResponse<IEnumerable<CategoryDto>>> GetCategoriesByFamilyGroupAsync(int familyGroupId);

        Task<ServiceResponse> UpdateCategoryAsync(CategoryDto categoryDto);
        Task<ServiceResponse<CategoryDto>> GetCategoryByIdAsync(int categoryId);

        Task<ServiceResponse> DeleteCategoryAsync(int categoryId);

        Task<ServiceResponse<IEnumerable<CategoryDto>>> GetAllCategories();

        Task<ServiceResponse<IEnumerable<CategoryDto>>> GetExpenseCategoriesByUsage(int userId);

    }
}

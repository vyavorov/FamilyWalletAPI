using FamilyWallet.Application.Repositories;
using FamilyWallet.Application.Repositories.Interfaces;
using FamilyWallet.Application.Services.Interfaces;
using FamilyWallet.Domain.DTOs;
using FamilyWallet.Domain.Enums;
using FamilyWallet.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyWallet.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IUserRepository userRepository;
        public CategoryService(ICategoryRepository categoryRepository, IUserRepository userRepository)
        {
            this.categoryRepository = categoryRepository;
            this.userRepository = userRepository;
        }
        public async Task<ServiceResponse<CategoryDto>> AddCategoryAsync(CategoryDto categoryDto)
        {
            var user = await userRepository.GetByIdAsync(categoryDto.UserId);
            if (user == null)
            {
                return new ServiceResponse<CategoryDto> { Message = "User not found", Success = false };
            }
            var category = new Category
            {
                Name = categoryDto.Name,
                UserId = categoryDto.UserId,
                FamilyGroupId = categoryDto.FamilyGroupId,
            };
            await categoryRepository.AddAsync(category);

            return new ServiceResponse<CategoryDto>
            {
                Message = "Category added successfully",
                Success = true,
                Data = new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    UserId = category.UserId,
                    FamilyGroupId = category.FamilyGroupId
                }
            };
        }

        public async Task<ServiceResponse> DeleteCategoryAsync(int categoryId)
        {
            var category = categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                return new ServiceResponse { Message = "Category not found", Success = false };
            }
            await categoryRepository.DeleteAsync(categoryId);
            return new ServiceResponse { Message = "Category deleted successfully", Success = true };
        }

        public async Task<ServiceResponse<IEnumerable<CategoryDto>>> GetAllCategories()
        {
            var categories = await categoryRepository.GetAllAsync();
            if (!categories.Any())
            {
                return new ServiceResponse<IEnumerable<CategoryDto>> { Message = "No categories found", Success = false };
            }
            var categoriesDto = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                UserId = c.UserId,
                FamilyGroupId = c.FamilyGroupId,
            });

            return new ServiceResponse<IEnumerable<CategoryDto>> { Data = categoriesDto, Success = true };
        }

        public async Task<ServiceResponse<IEnumerable<CategoryDto>>> GetCategoriesByFamilyGroupAsync(int familyGroupId)
        {
            var categories = await categoryRepository.GetCategoriesByFamilyIdAsync(familyGroupId);
            if (!categories.Any())
            {
                return new ServiceResponse<IEnumerable<CategoryDto>> { Message = "No categories found", Success = false };
            }
            var categoriesDto = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                UserId = c.UserId,
                FamilyGroupId = c.FamilyGroupId,
            });
            return new ServiceResponse<IEnumerable<CategoryDto>> { Data = categoriesDto, Success = true };
        }

        public async Task<ServiceResponse<IEnumerable<CategoryDto>>> GetCategoriesByUserAsync(int userId)
        {
            var categories = await categoryRepository.GetCategoriesByUserIdOrderedByUsageAsync(userId);

            if (!categories.Any())
            {
                return new ServiceResponse<IEnumerable<CategoryDto>>
                {
                    Message = "No categories found",
                    Success = true,
                    Data = new List<CategoryDto>()
                };
            }

            var categoriesDto = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                UserId = c.UserId,
                FamilyGroupId = c.FamilyGroupId,
            });

            return new ServiceResponse<IEnumerable<CategoryDto>>
            {
                Data = categoriesDto,
                Success = true
            };
        }

        public async Task<ServiceResponse<CategoryDto>> GetCategoryByIdAsync(int categoryId)
        {
            var category = await categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                return new ServiceResponse<CategoryDto> { Message = "Category not found", Success = false };
            }
            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                UserId = category.UserId,
                FamilyGroupId = category.FamilyGroupId,
            };
            return new ServiceResponse<CategoryDto> { Data = categoryDto, Success = true };
        }

        public async Task<ServiceResponse> UpdateCategoryAsync(CategoryDto categoryDto)
        {
            var category = await categoryRepository.GetByIdAsync(categoryDto.Id);
            if (category == null)
            {
                return new ServiceResponse { Message = "Category not found", Success = false };
            }
            category.Name = categoryDto.Name;
            await categoryRepository.UpdateAsync(category);
            return new ServiceResponse { Message = "Category updated successfully", Success = true };
        }

        public async Task<ServiceResponse<IEnumerable<CategoryDto>>> GetExpenseCategoriesByUsage(int userId)
        {
            var categories = await categoryRepository.GetExpenseCategoriesOrderedByUsageAsync(userId);

            var categoriesDto = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                UserId = c.UserId,
                FamilyGroupId = c.FamilyGroupId
            });

            return new ServiceResponse<IEnumerable<CategoryDto>> { Data = categoriesDto, Success = true };
        }

    }
}

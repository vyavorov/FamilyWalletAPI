using FamilyWallet.Domain.DTOs;
using FamilyWallet.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyWallet.Application.Repositories.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<ICollection<Category>> GetCategoriesByFamilyIdAsync(int familyGroupId);
        Task<ICollection<Category>> GetCategoriesByUserIdAsync(int userId);

        Task<IEnumerable<Category>> GetExpenseCategoriesOrderedByUsageAsync(int userId);


    }
}

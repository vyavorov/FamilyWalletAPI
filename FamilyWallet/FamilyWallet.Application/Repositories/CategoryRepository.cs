using FamilyWallet.Application.Repositories.Interfaces;
using FamilyWallet.Domain.DTOs;
using FamilyWallet.Domain.Models;
using FamilyWallet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyWallet.Application.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(FamilyWalletDbContext context) : base(context) { }

        public async Task<ICollection<Category>> GetCategoriesByFamilyIdAsync(int familyGroupId) => await _context.Categories.Where(a => a.User.FamilyGroupId == familyGroupId).ToListAsync();

        public async Task<ICollection<Category>> GetCategoriesByUserIdAsync(int userId) => await _context.Categories.Where(a => a.UserId == userId).ToListAsync();
    }
}

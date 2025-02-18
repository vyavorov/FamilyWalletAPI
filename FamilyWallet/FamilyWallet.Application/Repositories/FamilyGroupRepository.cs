using FamilyWallet.Application.Repositories.Interfaces;
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
    public class FamilyGroupRepository : GenericRepository<FamilyGroup>, IFamilyGroupRepository
    {
        public FamilyGroupRepository(FamilyWalletDbContext context) : base(context) { }

        public async Task<FamilyGroup?> GetByNameAsync(string name)
        {
            return await _context.FamilyGroups.FirstOrDefaultAsync(fg => fg.Name == name);
        }
    }
}

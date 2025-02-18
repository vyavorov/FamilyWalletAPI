using FamilyWallet.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyWallet.Application.Repositories.Interfaces;
    public interface IFamilyGroupRepository : IGenericRepository<FamilyGroup>
    {
        Task<FamilyGroup?> GetByNameAsync(string name);
    }
}

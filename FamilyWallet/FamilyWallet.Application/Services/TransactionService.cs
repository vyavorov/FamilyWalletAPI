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
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFamilyGroupRepository _familyGroupRepository;

        public TransactionService(ITransactionRepository transactionRepository, IUserRepository userRepository, IFamilyGroupRepository familyGroupRepository)
        {
            _transactionRepository = transactionRepository;
            _userRepository = userRepository;
            _familyGroupRepository = familyGroupRepository;
        }

        public async Task<ServiceResponse> AddTransactionAsync(TransactionDto transactionDto)
        {
            var user = await _userRepository.GetByIdAsync(transactionDto.UserId);
            if (user == null)
            {
                return new ServiceResponse { Message = "User not found", Success = false };
            }
            if (transactionDto.Amount <= 0)
            {
                return new ServiceResponse { Message = "Amount must be greater than 0", Success = false };
            }
            if (transactionDto.Type == TransactionType.Expense && user.Balance < transactionDto.Amount)
            {
                return new ServiceResponse { Message = "Insufficient balance", Success = false };
            }
            var transaction = new Transaction
            {
                UserId = transactionDto.UserId,
                Amount = transactionDto.Amount,
                Date = transactionDto.Date,
                Type = transactionDto.Type,
                Category = transactionDto.Category,
                Description = transactionDto.Description,
            };
            await _transactionRepository.AddAsync(transaction);

            if (transactionDto.Type == TransactionType.Expense)
            {
                user.Balance -= transactionDto.Amount;
            }
            else if (transactionDto.Type == TransactionType.Income)
            {
                user.Balance += transactionDto.Amount;
            }
            await _userRepository.UpdateAsync(user);
            if (user.FamilyGroupId.HasValue)
            {
                var familyGroup = await _familyGroupRepository.GetByIdAsync(user.FamilyGroupId.Value);
                if (familyGroup != null)
                {
                    if (transactionDto.Type == TransactionType.Income)
                    {
                        familyGroup.Balance += transactionDto.Amount;
                    }
                    else if (transactionDto.Type == TransactionType.Expense)
                    {
                        familyGroup.Balance -= transactionDto.Amount;
                    }
                    await _familyGroupRepository.UpdateAsync(familyGroup);
                }
            }
            return new ServiceResponse { Message = "Transaction added successfully", Success = true };
        }

        public async Task<ServiceResponse<IEnumerable<TransactionDto>>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var transactions = await _transactionRepository.GetByDateRangeAsync(startDate, endDate);
            if (!transactions.Any())
            {
                return new ServiceResponse<IEnumerable<TransactionDto>> { Success = false, Message = "No transactions found for this user." };

            }
            var transactionDtos = transactions.Select(t => new TransactionDto
            {
                Id = t.Id,
                UserId = t.UserId,
                Amount = t.Amount,
                Date = t.Date,
                Type = t.Type,
                Category = t.Category,
                Description = t.Description,
            });
            return new ServiceResponse<IEnumerable<TransactionDto>> { Success = true, Data = transactionDtos };
        }

        public async Task<ServiceResponse<IEnumerable<TransactionDto>>> GetTransactionsByFamilyGroupAsync(int familyGroupId)
        {
            var transactions = await _transactionRepository.GetByFamilyGroupIdAsync(familyGroupId);
            if (!transactions.Any())
            {
                return new ServiceResponse<IEnumerable<TransactionDto>> { Success = false, Message = "No transactions found for this family group." };
            }
            var transactionDtos = transactions.Select(t => new TransactionDto
            {
                Id = t.Id,
                UserId = t.UserId,
                Amount = t.Amount,
                Date = t.Date,
                Type = t.Type,
                Category = t.Category,
                Description = t.Description,
            });
            return new ServiceResponse<IEnumerable<TransactionDto>> { Success = true, Data = transactionDtos };
        }

        public async Task<ServiceResponse<IEnumerable<TransactionDto>>> GetTransactionsByTypeAsync(int userId, TransactionType type)
        {
            var transactions = await _transactionRepository.GetByTypeAsync(type);
            var filteredTransactions = transactions.Where(t => t.UserId == userId);
            if (!filteredTransactions.Any())
            {
                return new ServiceResponse<IEnumerable<TransactionDto>> { Success = false, Message = "No transactions found for this user." };
            }
            var transactionDtos = filteredTransactions.Select(t => new TransactionDto
            {
                Id = t.Id,
                UserId = t.UserId,
                Amount = t.Amount,
                Date = t.Date,
                Type = t.Type,
                Category = t.Category,
                Description = t.Description,
            });
            return new ServiceResponse<IEnumerable<TransactionDto>> { Success = true, Data = transactionDtos };
        }

        public async Task<ServiceResponse<IEnumerable<TransactionDto>>> GetTransactionsByUserAsync(int userId)
        {
            var transactions = await _transactionRepository.GetByUserIdAsync(userId);
            if (!transactions.Any())
            {
                return new ServiceResponse<IEnumerable<TransactionDto>> { Success = false, Message = "No transactions found for this user." };
            }
            var transactionDtos = transactions.Select(t => new TransactionDto
            {
                Id = t.Id,
                UserId = t.UserId,
                Amount = t.Amount,
                Date = t.Date,
                Type = t.Type,
                Category = t.Category,
                Description = t.Description,
            });
            return new ServiceResponse<IEnumerable<TransactionDto>> { Success = true, Data = transactionDtos };
        }
    }
}

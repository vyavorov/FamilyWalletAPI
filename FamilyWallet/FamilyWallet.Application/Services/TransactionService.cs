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
        private readonly IAccountRepository _accountRepository;

        public TransactionService(ITransactionRepository transactionRepository, IUserRepository userRepository, IFamilyGroupRepository familyGroupRepository, IAccountRepository accountRepository)
        {
            _transactionRepository = transactionRepository;
            _userRepository = userRepository;
            _familyGroupRepository = familyGroupRepository;
            _accountRepository = accountRepository;
        }

        public async Task<ServiceResponse> AddTransactionAsync(TransactionDto transactionDto)
        {
            var account = await _accountRepository.GetByIdAsync(transactionDto.AccountId);
            var user = await _userRepository.GetByIdAsync(transactionDto.UserId);
            if (user == null)
            {
                return new ServiceResponse { Message = "User not found", Success = false };
            }
            if (transactionDto.Amount <= 0)
            {
                return new ServiceResponse { Message = "Amount must be greater than 0", Success = false };
            }
            if (account == null)
            {
                return new ServiceResponse { Message = "Account not found", Success = false };
            }
            if (transactionDto.Type == TransactionType.Expense && account.Balance< transactionDto.Amount)
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
                AccountId = account.Id
            };
            await _transactionRepository.AddAsync(transaction);

            if (transactionDto.Type == TransactionType.Expense)
            {
                user.Balance -= transactionDto.Amount;
                account.Balance -= transactionDto.Amount;
            }
            else if (transactionDto.Type == TransactionType.Income)
            {
                user.Balance += transactionDto.Amount;
                account.Balance += transactionDto.Amount;
            }
            await _userRepository.UpdateAsync(user);
            await _accountRepository.UpdateAsync(account);
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

        public async Task<ServiceResponse> DeleteTransactionAsync(int transactionId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction == null)
            {
                return new ServiceResponse { Message = "Transaction not found", Success = false };
            }
            var user = await _userRepository.GetByIdAsync(transaction.UserId);
            var account = await _accountRepository.GetByIdAsync(transaction.AccountId);
            if (user == null)
            {
                return new ServiceResponse { Message = "User not found", Success = false };
            }
            if (account == null)
            {
                return new ServiceResponse { Message = "Account not found", Success = false };
            }
            if (transaction.Type == TransactionType.Expense)
            {
                user.Balance += transaction.Amount;
                account.Balance += transaction.Amount;
            }
            else if (transaction.Type == TransactionType.Income)
            {
                user.Balance -= transaction.Amount;
                account.Balance -= transaction.Amount;
            }
            await _userRepository.UpdateAsync(user);
            await _accountRepository.UpdateAsync(account);
            await _transactionRepository.DeleteAsync(transactionId);
            return new ServiceResponse { Message = "Transaction deleted successfully", Success = true };
        }

        public async Task<ServiceResponse<IEnumerable<TransactionDto>>> GetAllTransactions()
        {
            var transactions = await _transactionRepository.GetAllAsync();
            if (!transactions.Any())
            {
                return new ServiceResponse<IEnumerable<TransactionDto>> { Success = false, Message = "No transactions yet" };

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

        public async Task<ServiceResponse<TransactionDto>> GetTransactionByIdAsync(int transactionId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction == null)
            {
                return new ServiceResponse<TransactionDto> { Success = false, Message = "No such transaction" };
            }
            var transactionDto = new TransactionDto()
            {
                AccountId = transaction.AccountId,
                Amount = transaction.Amount,
                Category = transaction.Category,
                Date = transaction.Date,
                Description = transaction.Description,
                Type = transaction.Type,
                UserId = transaction.UserId,
                Id = transaction.Id
            };
            return new ServiceResponse<TransactionDto> { Success = true, Data = transactionDto };
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

        public async Task<ServiceResponse> UpdateTransactionAsync(TransactionDto transactionDto)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionDto.Id);
            if (transaction == null)
            {
                return new ServiceResponse { Message = "Transaction not found", Success = false };
           }
            var account = await _accountRepository.GetByIdAsync(transactionDto.AccountId);
            var user = await _userRepository.GetByIdAsync(transactionDto.UserId);
            if (user == null)
            {
                return new ServiceResponse { Message = "User not found", Success = false };
            }
            if (transactionDto.Amount <= 0)
            {
                return new ServiceResponse { Message = "Amount must be greater than 0", Success = false };
            }
            if (account == null)
            {
                return new ServiceResponse { Message = "Account not found", Success = false };
            }
            if (transactionDto.Type == TransactionType.Expense && account.Balance < transactionDto.Amount)
            {
                return new ServiceResponse { Message = "Insufficient balance", Success = false };
            }
            if (transactionDto.Type == TransactionType.Expense)
            {
                user.Balance += transaction.Amount;
                account.Balance += transaction.Amount;
                user.Balance -= transactionDto.Amount;
                account.Balance -= transactionDto.Amount;
            }
            else if (transactionDto.Type == TransactionType.Income)
            {
                user.Balance -= transaction.Amount;
                account.Balance -= transaction.Amount;
                user.Balance += transactionDto.Amount;
                account.Balance += transactionDto.Amount;
            }
            transaction.UserId = transactionDto.UserId;
            transaction.Amount = transactionDto.Amount;
            transaction.Date = transactionDto.Date;
            transaction.Type = transactionDto.Type;
            transaction.Category = transactionDto.Category;
            transaction.Description = transactionDto.Description;
            transaction.AccountId = account.Id;
            await _transactionRepository.UpdateAsync(transaction);
            await _userRepository.UpdateAsync(user);
            await _accountRepository.UpdateAsync(account);
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
            return new ServiceResponse { Message = "Transaction updated successfully", Success = true };
        }

    }
}

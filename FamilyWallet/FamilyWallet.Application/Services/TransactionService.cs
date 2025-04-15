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
            if (account == null && transactionDto.Type != TransactionType.Transfer)
            {
                return new ServiceResponse { Message = "Account not found", Success = false };
            }
            if (transactionDto.Type == TransactionType.Expense && account.Balance < transactionDto.Amount)
            {
                return new ServiceResponse { Message = "Insufficient balance", Success = false };
            }

            var transaction = new Transaction
            {
                UserId = transactionDto.UserId,
                Amount = transactionDto.Amount,
                Date = transactionDto.Date,
                Type = transactionDto.Type,
                CategoryId = transactionDto.CategoryId,
                Description = transactionDto.Description,
            };

            if (transaction.Type == TransactionType.Transfer)
            {
                var fromAccount = await _accountRepository.GetByIdAsync(transactionDto.FromAccountId.Value);
                var toAccount = await _accountRepository.GetByIdAsync(transactionDto.ToAccountId.Value);
                if (fromAccount == null || toAccount == null)
                {
                    return new ServiceResponse { Message = "Account not found", Success = false };
                }
                if (fromAccount.Balance < transactionDto.Amount)
                {
                    return new ServiceResponse { Message = "Insufficient balance", Success = false };
                }
                transaction.FromAccountId = fromAccount.Id;
                transaction.ToAccountId = toAccount.Id;
                fromAccount.Balance -= transactionDto.Amount;
                toAccount.Balance += transactionDto.Amount;
                await _accountRepository.UpdateAsync(fromAccount);
                await _accountRepository.UpdateAsync(toAccount);
                await _transactionRepository.AddAsync(transaction);
                return new ServiceResponse { Message = "Transfer completed successfully", Success = true };
            }
            else
            {
                transaction.AccountId = account.Id;
            }
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
            await _transactionRepository.AddAsync(transaction);
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
            var toAccount = await _accountRepository.GetByIdAsync(transaction.ToAccountId);
            var fromAccount = await _accountRepository.GetByIdAsync(transaction.FromAccountId);
            if (user == null)
            {
                return new ServiceResponse { Message = "User not found", Success = false };
            }
            if (account == null && transaction.Type != TransactionType.Transfer)
            {
                return new ServiceResponse { Message = "Account not found", Success = false };
            }
            if (transaction.Type == TransactionType.Expense)
            {
                user.Balance += transaction.Amount;
                account!.Balance += transaction.Amount;
            }
            else if (transaction.Type == TransactionType.Income)
            {
                user.Balance -= transaction.Amount;
                account!.Balance -= transaction.Amount;
            }
            else
            {
                if (fromAccount == null || toAccount == null)
                {
                    return new ServiceResponse { Message = "Account not found", Success = false };
                }
                fromAccount.Balance += transaction.Amount;
                toAccount.Balance -= transaction.Amount;
                
            }
            await _userRepository.UpdateAsync(user);
            if (account != null)
            {
                await _accountRepository.UpdateAsync(account);
            }
            else if (fromAccount != null && toAccount != null)
            {
                await _accountRepository.UpdateAsync(fromAccount);
                await _accountRepository.UpdateAsync(toAccount);
            }
            await _transactionRepository.DeleteAsync(transactionId);
            return new ServiceResponse { Message = "Transaction deleted successfully", Success = true };
        }

        public async Task<ServiceResponse<IEnumerable<TransactionDto>>> GetAllTransactions()
        {
            var transactions = await _transactionRepository.GetAllOrderedTransactions();
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
                CategoryId = t.CategoryId,
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
                CategoryId = transaction.CategoryId,
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
                CategoryId = t.CategoryId,
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
                CategoryId = t.CategoryId,
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
                CategoryId = t.CategoryId,
                Description = t.Description,
            });
            return new ServiceResponse<IEnumerable<TransactionDto>> { Success = true, Data = transactionDtos };
        }

        public async Task<ServiceResponse<IEnumerable<TransactionDto>>> GetTransactionsByUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new ServiceResponse<IEnumerable<TransactionDto>> { Success = false, Message = "User not found." };
            }

            IEnumerable<Transaction> transactions = new List<Transaction>();
            if (user.FamilyGroupId.HasValue)
            {
                transactions = await _transactionRepository.GetByFamilyGroupIdAsync(user.FamilyGroupId.Value);
            }
            else
            {
                transactions = await _transactionRepository.GetByUserIdAsync(userId);
            }

            if (!transactions.Any())
            {
                return new ServiceResponse<IEnumerable<TransactionDto>> { Message = "No transactions found", Success = true, Data = new List<TransactionDto>() };
            }

            var transactionDtos = transactions.Select(t => new TransactionDto
            {
                Id = t.Id,
                Description = t.Description,
                Amount = t.Amount,
                Type = t.Type,
                Date = t.Date,
                CategoryId = t.CategoryId,
                AccountId = t.AccountId,
                FromAccountId = t.FromAccountId,
                ToAccountId = t.ToAccountId
            }).ToList();

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
            if (account == null && transactionDto.Type != TransactionType.Transfer)
            {
                return new ServiceResponse { Message = "Account not found", Success = false };
            }
            if (transactionDto.Type == TransactionType.Expense && account.Balance < transactionDto.Amount)
            {
                return new ServiceResponse { Message = "Insufficient balance", Success = false };
            }
            if (transactionDto.Type != TransactionType.Transfer && transaction.AccountId != transactionDto.AccountId)
            {
                var oldAccount = await _accountRepository.GetByIdAsync(transaction.AccountId);
                if (oldAccount == null)
                {
                    return new ServiceResponse { Message = "Account not found", Success = false };
                }
                if (transaction.Type == TransactionType.Income)
                {
                    account.Balance += transactionDto.Amount;
                    oldAccount.Balance -= transactionDto.Amount;
                }
                else if (transaction.Type == TransactionType.Expense)
                {
                    account.Balance -= transactionDto.Amount;
                    oldAccount.Balance += transactionDto.Amount;
                }
            }
            else
            {
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
                else if (transactionDto.Type == TransactionType.Transfer)
                {
                    var fromAccount = await _accountRepository.GetByIdAsync(transactionDto.FromAccountId);
                    var toAccount = await _accountRepository.GetByIdAsync(transactionDto.ToAccountId);

                    if (fromAccount == null || toAccount == null)
                    {
                        return new ServiceResponse { Message = "Account not found", Success = false };
                    }
                    if (fromAccount.Balance < transactionDto.Amount)
                    {
                        return new ServiceResponse { Message = "Insufficient balance", Success = false };
                    }
                    fromAccount.Balance += transaction.Amount;
                    toAccount.Balance -= transaction.Amount;
                    fromAccount.Balance -= transactionDto.Amount;
                    toAccount.Balance += transactionDto.Amount;
                    await _accountRepository.UpdateAsync(fromAccount);
                    await _accountRepository.UpdateAsync(toAccount);
                }
            }
            transaction.UserId = transactionDto.UserId;
            transaction.Amount = transactionDto.Amount;
            transaction.Date = transactionDto.Date;
            transaction.Type = transactionDto.Type;
            transaction.CategoryId = transactionDto.CategoryId;
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

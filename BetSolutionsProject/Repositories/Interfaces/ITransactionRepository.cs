using BetSolutionsProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BetSolutionsProject.Repositories.Interfaces
{

    public interface ITransactionRepository
    {
        int Withdraw(string userId, decimal amount);
        void UpdateTransactionStatus(int transactionId, string userId, int status);
        decimal GetCurrentBalance(string userId);
        IEnumerable<Transactions> GetTransactionHistory(string userId, DateTime? startDate, DateTime? endDate);
        void CreateWallet(string userId, decimal currentBalance);
        int Deposit(string userId, decimal amount);
    }
}

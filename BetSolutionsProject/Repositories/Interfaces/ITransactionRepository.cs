using BetSolutionsProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace BetSolutionsProject.Repositories.Interfaces
{

    public interface ITransactionRepository
    {
        decimal GetCurrentBalance(string userId);
        public IEnumerable<Transactions> GetTransactionHistory(string userId);
        void CreateWallet(string userId, decimal currentBalance);
        void Deposit(string userId, decimal amount);
        void Withdraw(string userId, decimal amount);
    }
}

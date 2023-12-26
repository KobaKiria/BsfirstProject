using BetSolutionsProject.Repositories.Interfaces;
using Dapper;
using System.Data;
using BetSolutionsProject.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore;

namespace BetSolutionsProject.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IDbConnection _dbConnection;

        public TransactionRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public IEnumerable<Transactions> GetTransactionHistory(string userId)
        {
            string query = "SELECT * FROM Transactions WHERE UserId = @UserId";
            return _dbConnection.Query<Transactions>(query, new { UserId = userId });
        }
         public void CreateWallet(string userId, decimal currentBalance)
        {
            try
            {
                _dbConnection.Execute(
                    "INSERT INTO Wallet (UserId, CurrentBalance) VALUES (@UserId, @CurrentBalance)",
                    new { UserId = userId, CurrentBalance = currentBalance }
                );
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Exception in CreateWallet: {ex.Message}");
            }
        }
        public void Deposit(string userId, decimal amount)
        {
            try
            {
                _dbConnection.Open();
                using (var transaction = _dbConnection.BeginTransaction())
                {
                    _dbConnection.Execute(
                        "UPDATE Wallet SET CurrentBalance = CurrentBalance + @Amount WHERE UserId = @UserId",
                        new { UserId = userId, Amount = amount },
                        transaction
                    );


                    _dbConnection.Execute(
                      "INSERT INTO Transactions (UserId, Amount, TransactionType, TransactionStatus, CurrentBalance, TransactionDate) " +
                      "VALUES (@UserId, @Amount, @TransactionType, '0', (SELECT CurrentBalance FROM Wallet WHERE UserId = @UserId), GETDATE())",
                      new { UserId = userId, Amount = amount, TransactionType = TransactionType.Deposit },
                      transaction
                  );

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in Deposit: {ex.Message}");
            }
        }

        public void Withdraw(string userId, decimal amount)
        {
            try
            {
                _dbConnection.Open();
                using (var transaction = _dbConnection.BeginTransaction())
                {
                    _dbConnection.Execute(
                        "UPDATE Wallet SET CurrentBalance = CurrentBalance - @Amount WHERE UserId = @UserId",
                        new { UserId = userId, Amount = amount },
                        transaction
                    );


                    _dbConnection.Execute(
                      "INSERT INTO Transactions (UserId, Amount, TransactionType, TransactionStatus, CurrentBalance, TransactionDate) " +
                      "VALUES (@UserId, @Amount, @TransactionType, '0', (SELECT CurrentBalance FROM Wallet WHERE UserId = @UserId), GETDATE())",
                      new { UserId = userId, Amount = amount, TransactionType = TransactionType.Withdraw },
                      transaction
                  );

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in Withdraw: {ex.Message}");
            }
            finally
            {
                _dbConnection.Close();
            }
            
        }

        decimal ITransactionRepository.GetCurrentBalance(string userId)
        {          
            string query = "SELECT TOP 1 CurrentBalance FROM Wallet WHERE UserId = @UserId";
            return _dbConnection.QueryFirstOrDefault<decimal?>(query, new { UserId = userId }) ?? 0m;
            
        }
    }
}

using BetSolutionsProject.Models;
using BetSolutionsProject.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BetSolutionsProject.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly IDbConnection _dbConnection;

        public WalletRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
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
                using (var connection = new SqlConnection("Server=DESKTOP-KNCH97K\\SQLEXPRESS;Database=BSProject;Trusted_Connection=True;TrustServerCertificate=True"))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            
                            connection.Execute(
                                "UPDATE Wallet SET CurrentBalance = CurrentBalance + @Amount WHERE UserId = @UserId",
                                new { UserId = userId, Amount = amount },
                                transaction
                            );

                            connection.Execute(
                                "INSERT INTO Transactions (UserId, Amount, TransactionType, TransactionStatus, CurrentBalance, TransactionDate) " +
                                "VALUES (@UserId, @Amount, @TransactionType, 0, (SELECT CurrentBalance FROM Wallet WHERE UserId = @UserId), GETDATE())",
                                new { UserId = userId, Amount = amount, TransactionType = TransactionType.Deposit },
                                transaction
                            );

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Exception in Deposit transaction: {ex.Message}");
                            transaction.Rollback();
                            throw; 
                        }
                    }
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
                      "VALUES (@UserId, @Amount, 'Withdraw', '0', (SELECT CurrentBalance FROM Wallet WHERE UserId = @UserId), GETDATE())",
                      new { UserId = userId, Amount = amount },
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
    }
}

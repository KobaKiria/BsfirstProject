using BetSolutionsProject.Repositories.Interfaces;
using Dapper;
using System.Data;
using BetSolutionsProject.Models;
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

        public IEnumerable<Transactions> GetTransactionHistory(string userId, DateTime? startDate, DateTime? endDate)
        {
            string query = "SELECT * FROM Transactions WHERE UserId = @UserId";
            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId);

            if (startDate.HasValue)
            {
                query += " AND TransactionDate >= @StartDate";
                parameters.Add("StartDate", startDate.Value);
            }

            if (endDate.HasValue)
            {
                query += " AND TransactionDate <= @EndDate";
                parameters.Add("EndDate", endDate.Value);
            }

            return _dbConnection.Query<Transactions>(query, parameters);
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
        public int Deposit(string userId, decimal amount)
        {
            try
            {
                _dbConnection.Open();
                using (var transaction = _dbConnection.BeginTransaction())
                {

                    string insertQuery = @"
                     INSERT INTO Transactions (UserId, Amount, TransactionType, TransactionStatus, CurrentBalance, TransactionDate) 
                     VALUES (@UserId, @Amount, @TransactionType, '0', (SELECT CurrentBalance FROM Wallet WHERE UserId = @UserId), GETDATE());
                      SELECT CAST(SCOPE_IDENTITY() as int);"; 

                    int transactionId = _dbConnection.QuerySingle<int>(insertQuery,
                        new { UserId = userId, Amount = amount, TransactionType = TransactionType.Deposit },
                        transaction);

                    transaction.Commit();
                    return transactionId; 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in Deposit: {ex.Message}");
                throw;
            }
            finally
            {
                _dbConnection.Close();
            }
        }


        public int Withdraw(string userId, decimal amount)
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

                    string insertQuery = @"
                INSERT INTO Transactions (UserId, Amount, TransactionType, TransactionStatus, CurrentBalance, TransactionDate) 
                OUTPUT INSERTED.TransactionId
                VALUES (@UserId, @Amount, @TransactionType, '0', (SELECT CurrentBalance FROM Wallet WHERE UserId = @UserId), GETDATE());";

                    int transactionId = _dbConnection.QuerySingle<int>(
                        insertQuery,
                        new { UserId = userId, Amount = amount, TransactionType = TransactionType.Withdraw },
                        transaction
                    );

                    transaction.Commit();
                    return transactionId;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in Withdraw: {ex.Message}");
                // Consider whether to rollback the transaction here
                throw; // rethrow the exception to handle it further up the call stack
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

        public void UpdateTransactionStatus(int transactionId, string userId, int status)
        {
            try
            {
                _dbConnection.Open();
                using (var transaction = _dbConnection.BeginTransaction())
                {
                    string updateStatusQuery = "UPDATE Transactions " +
                                               "SET TransactionStatus = @Status " +
                                               "WHERE TransactionId = @TransactionId";
                    _dbConnection.Execute(updateStatusQuery, new { TransactionId = transactionId, Status = status }, transaction);

                    if (status == 1)
                    {
                        string updateTransactionBalanceQuery = "UPDATE Transactions " +
                            "SET CurrentBalance = CurrentBalance + (SELECT Amount FROM Transactions WHERE TransactionId = @TransactionId) " +
                            "WHERE TransactionId = @TransactionId";

                        _dbConnection.Execute(updateTransactionBalanceQuery, new { TransactionId = transactionId, UserId = userId }, transaction);

                        string updateBalanceQuery = @"
                             UPDATE Wallet
                             SET CurrentBalance = (
                             SELECT CurrentBalance
                             FROM Transactions
                             WHERE TransactionId = @TransactionId
                             )
                             WHERE UserId = (
                             SELECT UserId
                             FROM Transactions
                             WHERE TransactionId = @TransactionId
                            )";
                        _dbConnection.Execute(updateBalanceQuery, new { TransactionId = transactionId, UserId = userId }, transaction);

                    }

                    transaction.Commit();
                }
            
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in UpdateTransactionStatus: {ex.Message}");
                throw;
            }
            finally
            {
                _dbConnection.Close();
            }
        }
        }
    }

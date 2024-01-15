using BetSolutionsProject.Repositories.Interfaces;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Data;

namespace BetSolutionsProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionApiController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IDbConnection _dbConnection;

        public TransactionApiController(ITransactionRepository transactionRepository, IDbConnection dbConnection)
        {
            _transactionRepository = transactionRepository;
            _dbConnection = dbConnection;
        }

        [HttpPost("UpdateTransactionStatus")]
        public IActionResult UpdateTransactionStatus([FromBody] tran bell)
        {
            try
            {
                string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                _transactionRepository.UpdateTransactionStatus(bell.transactionId, userId, bell.status);
                return Ok(new { message = "Transaction status updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpPost("UpdateWithdrawalStatus")]
        public IActionResult UpdateWithdrawalStatus([FromBody] tran bell)
        {
            try
            {
                _dbConnection.Open();
                using (var transaction = _dbConnection.BeginTransaction())
                {
                    
                    string updateTransactionStatusQuery = "UPDATE Transactions SET TransactionStatus = @Status WHERE TransactionId = @TransactionId";
                    _dbConnection.Execute(updateTransactionStatusQuery, new { TransactionId = bell.transactionId, Status = bell.status }, transaction);

                   
                    if (bell.status == 2) 
                    {
                        string getUserIdAndAmountQuery = "SELECT UserId, Amount FROM Transactions WHERE TransactionId = @TransactionId";
                        var result = _dbConnection.QueryFirstOrDefault(getUserIdAndAmountQuery, new { TransactionId = bell.transactionId }, transaction);

                        if (result != null)
                        {
                            string creditBackQuery = "UPDATE Wallet SET CurrentBalance = CurrentBalance + @Amount WHERE UserId = @UserId";
                            _dbConnection.Execute(creditBackQuery, new { UserId = result.UserId, Amount = result.Amount }, transaction);

                            string creditbackSecondQuery = "UPDATE Transactions " +
                                "SET CurrentBalance = (" +
                                "  SELECT Wallet.CurrentBalance " +
                                "  FROM Wallet   " +
                                " WHERE Wallet.UserId = Transactions.UserId" +
                                ")" +
                                " WHERE TransactionId = @TransactionId;";


                            _dbConnection.Execute(creditbackSecondQuery, new { TransactionId = bell.transactionId }, transaction);
                        }
                    }

                    transaction.Commit();
                    return Ok(new { message = "Transaction status updated successfully." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateWithdrawalStatus: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
            finally
            {
                _dbConnection.Close();
            }
        }
    }
    public class tran
    {
        public int transactionId { get; set; }
        public int status { get; set; }
    }

}

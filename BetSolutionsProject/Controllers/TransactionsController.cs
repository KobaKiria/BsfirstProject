using BetSolutionsProject.Models;
using BetSolutionsProject.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BetSolutionsProject.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionsController(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public IActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public IActionResult GetTransactionHistory(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var transactions = _transactionRepository.GetTransactionHistory(userId, startDate, endDate);
                Console.WriteLine("Transactions retrieved successfully.");
                return Json(transactions);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching transaction history: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }
        
   /*     public IActionResult Deposit(decimal amount)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var transactionId = _transactionRepository.Deposit(userId, amount);

            var bankAppUrl = $"https://localhost:7116/Transaction/Confirm?transactionId={transactionId}&userId={userId}&amount={amount}";
            return Redirect(bankAppUrl);
        }*/
    }
    }

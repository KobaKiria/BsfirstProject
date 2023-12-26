using BetSolutionsProject.Models;
using BetSolutionsProject.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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
        public IActionResult GetTransactionHistory()
        {
            try
            {
                var transactions = _transactionRepository.GetTransactionHistory();
                Console.WriteLine("Transactions retrieved successfully.");
                return Json(transactions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching transaction history: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }
    }

}

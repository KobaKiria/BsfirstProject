using BetSolutionsProject.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BetSolutionsProject.Controllers
{
    public class DepositController : Controller
    {
        private readonly ITransactionRepository _transactionRepository;

        public DepositController(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(decimal amount)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            _transactionRepository.Deposit(userId, amount);

            ViewData["Message"] = $"Successfully deposited {amount}.";

            return View();
        }
    }
}

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

            var transactionId = _transactionRepository.Deposit(userId, amount);

            var bankAppUrl = $"https://localhost:7116/Transaction/Confirm?transactionId={transactionId}&userId={userId}&amount={amount}"; 
            return Redirect(bankAppUrl);
        }
    }
}

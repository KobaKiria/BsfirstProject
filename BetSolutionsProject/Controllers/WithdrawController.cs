using BetSolutionsProject.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BetSolutionsProject.Controllers
{
    public class WithdrawController : Controller
    {
        private readonly ITransactionRepository _transactionRepository;
        public WithdrawController(ITransactionRepository walletRepository)
        {
            _transactionRepository = walletRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(decimal amount)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            _transactionRepository.Withdraw(userId, amount);

            ViewData["Message"] = $"Successfull Witdrawal: {amount}.";

            return View();
        }
    }
}

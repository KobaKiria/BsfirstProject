using BetSolutionsProject.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BetSolutionsProject.Controllers
{
    public class WithdrawController : Controller
    {
        private readonly IWalletRepository _walletRepository;
        public WithdrawController(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(decimal amount)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            _walletRepository.Withdraw(userId, amount);

            ViewData["Message"] = $"Successfull Witdrawal: {amount}.";

            return View();
        }
    }
}

using BetSolutionsProject.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BetSolutionsProject.Controllers
{
    public class DepositController : Controller
    {
        private readonly IWalletRepository _walletRepository;

        public DepositController(IWalletRepository walletRepository)
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

            _walletRepository.Deposit(userId, amount);

            ViewData["Message"] = $"Successfully deposited {amount}.";

            return View();
        }
    }
}

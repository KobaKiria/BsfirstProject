using BetSolutionsProject.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace BetSolutionsProject.Controllers
{
    public class WithdrawController : Controller
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        public WithdrawController(ITransactionRepository walletRepository, IHttpClientFactory httpClientFactory)
        {
            _transactionRepository = walletRepository;
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IndexAsync(decimal amount)
        {
            ViewData["Message"] = "Withdrawal request submitted. Pending approval";
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var transactionId = _transactionRepository.Withdraw(userId, amount);
            var bankAppUrl = "https://localhost:7116/api/withdrawal";
            var requestBody = new { TransactionId = transactionId, Amount = amount, UserId = userId };

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.PostAsJsonAsync(bankAppUrl, requestBody);

                
                //  check response.IsSuccessStatusCode and update ViewData["Message"] accordingly
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateWithdrawalStatus: {ex.Message}");
            }
            if (amount%2==0)
            {
                ViewData["Message"] = "Withdrawal Has Been Completed Succesfully";
            }
            else
            {
                ViewData["Message"] = "Withdrawal Has Been Rejected. ";
            }
            
            return View();
        }

    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace BankApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WithdrawalController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _mainAppApiUrl = "https://localhost:7004/api/TransactionApi/UpdateWithdrawalStatus";

        public WithdrawalController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessWithdrawal([FromBody] WithdrawalRequest request)
        {
            var httpClient = _httpClientFactory.CreateClient();
            int status = request.Amount % 2 == 0 ? 1 : 2;

            var callbackRequestBody = new
            {
                TransactionId = request.TransactionId,
                Status = status
            };

            var response = await httpClient.PostAsJsonAsync(_mainAppApiUrl, callbackRequestBody);

            if (response.IsSuccessStatusCode)
            {
                return Ok();
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error in processing withdrawal." });
            }
        }
    }

    public class WithdrawalRequest
    {
        public int TransactionId { get; set; }
        public decimal Amount { get; set; }
    }
}

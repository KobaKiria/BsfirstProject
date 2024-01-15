using BankApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System;

namespace BankApp.Controllers
{
    public class TransactionController : Controller
    {
        public IActionResult Confirm(int transactionId, string userId, decimal amount)
        {
            var viewModel = new ConfirmationView
            {
                TransactionId = transactionId,
                UserId = userId,
                Amount = amount
            };

            return View(viewModel);
        }
        [HttpPost]
            public async Task<IActionResult> ConfirmTransaction(int transactionId, string cardNumber, decimal amount, string userId)
            {
            try
            {
                int status= amount % 2 == 0 ? 1 : 2;
                var mainAppUrl = "https://localhost:7004/api/TransactionApi/UpdateTransactionStatus";
                var client = new RestClient(mainAppUrl);
                var something = new
                {
                    transactionId = transactionId,
                    status = status,
                    userId = userId
                };

                var request = new RestRequest();
                request.Method = Method.Post;
                request.AddHeader("Content-Type", "application/json");
                request.AddJsonBody(something);

                await client.ExecuteAsync(request);

                return new JsonResult(something);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in ConfirmTransaction: {ex}");
                return BadRequest(new { error = ex.Message });
            }

          
        }
            }
    }



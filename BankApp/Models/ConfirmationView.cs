namespace BankApp.Models
{
    public class ConfirmationView
    {
        public int TransactionId { get; set; }
        public string UserId { get; set; }
        public decimal Amount { get; set; }
    }
}

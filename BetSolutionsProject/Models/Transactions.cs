namespace BetSolutionsProject.Models
{
    public class Transactions
    {
        public int TransactionId { get; set; }
        public string UserId { get; set; }
        public decimal Amount { get; set; }

        public TransactionType TransactionType { get; set; }
        public int TransactionStatus { get; set; }
        public decimal CurrentBalance { get; set; }
        public DateTime TransactionDate { get; set; }
    }
    public enum TransactionType
    {
        Deposit =10,
        Withdraw = 20,
        Bet = 30,
        Win = 40
    }
}

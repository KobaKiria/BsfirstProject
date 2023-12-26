namespace BetSolutionsProject.Repositories.Interfaces
{
    public interface IWalletRepository
    {
        void CreateWallet(string userId, decimal currentBalance);
        void Deposit(string userId, decimal amount);
        void Withdraw(string userId, decimal amount);
    }
}

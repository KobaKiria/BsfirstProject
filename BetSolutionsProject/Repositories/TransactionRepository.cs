using BetSolutionsProject.Repositories.Interfaces;
using Dapper;
using System.Data;
using BetSolutionsProject.Models;

namespace BetSolutionsProject.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IDbConnection _dbConnection;

        public TransactionRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public IEnumerable<Transactions> GetTransactionHistory()
        {
            return _dbConnection.Query<Transactions>("SELECT * FROM Transactions");
        }
    }
}

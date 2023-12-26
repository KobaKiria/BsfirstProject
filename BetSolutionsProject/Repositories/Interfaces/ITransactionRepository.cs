using BetSolutionsProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace BetSolutionsProject.Repositories.Interfaces
{

    public interface ITransactionRepository
    {
            public IEnumerable<Transactions> GetTransactionHistory();
    }
}

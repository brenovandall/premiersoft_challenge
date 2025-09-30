using Application.CheckingAccount.Queries.GetBalance;

namespace Application.Services
{
    public interface ICheckingAccountService
    {
        Task<GetBalanceResponse?> GetAccountInfo(Guid checkingAccountId);
    }
}

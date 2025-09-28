using Application.CheckingAccount.Queries.GetBalance;

namespace Application.Services
{
    public interface ICheckingAccountService
    {
        GetBalanceResponse? GetAccountInfo(Guid checkingAccountId);
    }
}

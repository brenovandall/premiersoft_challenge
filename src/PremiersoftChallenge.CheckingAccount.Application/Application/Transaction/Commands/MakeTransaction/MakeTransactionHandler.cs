using Application.Authentication;
using Application.Data.Repository;
using Application.Exceptions;
using Domain;
using Domain.Enums;
using PremiersoftChallenge.BuildingBlocks.CQRS;
using PremiersoftChallenge.BuildingBlocks.Errors;
using PremiersoftChallenge.BuildingBlocks.Results;
using PremiersoftChallenge.SharedKernel.Exceptions;

namespace Application.Transaction.Commands.MakeTransaction
{
    public class MakeTransactionHandler : ICommandHandler<MakeTransactionCommand, bool>
    {
        private readonly ICheckingAccountRepository _checkingAccountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILoggedContext _loggedContext;

        public MakeTransactionHandler(
            ICheckingAccountRepository checkingAccountRepository,
            ITransactionRepository transactionRepository,
            ILoggedContext loggedContext)
        {
            _checkingAccountRepository = checkingAccountRepository;
            _transactionRepository = transactionRepository;
            _loggedContext = loggedContext;
        }

        public async Task<Result<bool>> Handle(MakeTransactionCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var account = GetCheckingAccount(_loggedContext.Id, null);
                var accountNumber = command.AccountNumber;

                if (!accountNumber.HasValue)
                {
                    accountNumber = account.Number;
                }
                else
                {
                    if (accountNumber != account.Number && command.TransactionFlow != "C")
                    {
                        return Result.Failure<bool>(TransactionErrors.InvalidFlowType);
                    }
                    else
                    {
                        account = GetCheckingAccount(null, accountNumber);
                    }
                }

                var transaction = Domain.Transaction.Create(account.Id, command.TransactionFlow, command.Value);
                _transactionRepository.Add(transaction);

                return Result.Success(true);
            }
            catch (InvalidAccountException)
            {
                return Result.Failure<bool>(CheckingAccountErrors.InvalidAccount);
            }
            catch (InvalidValueException)
            {
                return Result.Failure<bool>(TransactionErrors.InvalidValue);
            }
        }

        private ICheckingAccount GetCheckingAccount(Guid? id, long? number)
        {
            ICheckingAccount? account = id is not null
                ? _checkingAccountRepository.GetById(id.Value)
                : _checkingAccountRepository.GetByAccountNumberOrName(number!.Value.ToString());

            if (account is null)
                throw new InvalidAccountException(nameof(account));

            if (account.Status == CheckingAccountStatus.Inactive)
                throw new InvalidAccountException(nameof(account.Status));

            return account;
        }
    }
}

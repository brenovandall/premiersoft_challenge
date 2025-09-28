using Domain.ValueObjects;
using PremiersoftChallenge.SharedKernel;
using PremiersoftChallenge.SharedKernel.Exceptions;

namespace Domain
{
    public interface ITransaction
    {
        Guid Id { get; }
        Guid CheckingAccountId { get; }
        DateTime TransactionDate { get; }
        TransactionFlow TransactionFlow { get; }
        double Value { get; }
    }

    public class Transaction : Entity<Guid>, ITransaction
    {
        public Guid CheckingAccountId { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionFlow TransactionFlow { get; set; } = default!;
        public double Value { get; set; }

        public static ITransaction Create(Guid id, Guid checkingAccountId, string transactionFlow, double value)
        {
            if (value <= 0)
            {
                throw new InvalidValueException(nameof(value));
            }

            return new Transaction
            {
                Id = id,
                CheckingAccountId = checkingAccountId,
                TransactionDate = DateTime.Now,
                TransactionFlow = TransactionFlow.From(transactionFlow),
                Value = Math.Round(value, 2)
            };
        }
    }
}

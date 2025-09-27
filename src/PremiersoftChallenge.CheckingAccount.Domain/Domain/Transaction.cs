using Domain.ValueObjects;
using PremiersoftChallenge.SharedKernel;

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
        public TransactionFlow TransactionFlow { get; set; }
        public double Value { get; set; }
    }
}

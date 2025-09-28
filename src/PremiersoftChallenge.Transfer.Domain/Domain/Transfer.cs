using PremiersoftChallenge.SharedKernel;
using PremiersoftChallenge.SharedKernel.Exceptions;

namespace Domain
{
    public interface ITransfer
    {
        Guid Id { get; }
        Guid SourceCheckingAccountId { get; }
        Guid TargetCheckingAccountId { get; }
        DateTime TransactionDate { get; }
        double Value { get; }
    }

    public class Transfer : Entity<Guid>, ITransfer
    {
        public Guid SourceCheckingAccountId { get; private set; }
        public Guid TargetCheckingAccountId { get; private set; }
        public DateTime TransactionDate { get; private set; }
        public double Value { get; private set; }

        public static ITransfer Create(string sourceCheckingAccountId, string targetCheckingAccountId, double value)
        {
            if (!Guid.TryParse(sourceCheckingAccountId, out var sourceGuidOutput))
            {
                throw new DomainException($"GUID {sourceGuidOutput} inválido.");
            }

            if (!Guid.TryParse(targetCheckingAccountId, out var targetGuidOutput))
            {
                throw new DomainException($"GUID {targetGuidOutput} inválido.");
            }

            if (value < 0)
            {
                throw new DomainException($"O valor da transferência deve ser maior do que 0.");
            }

            return new Transfer
            {
                Id = Guid.NewGuid(),
                SourceCheckingAccountId = sourceGuidOutput,
                TargetCheckingAccountId = targetGuidOutput,
                TransactionDate = DateTime.Now,
                Value = value
            };
        }
    }
}

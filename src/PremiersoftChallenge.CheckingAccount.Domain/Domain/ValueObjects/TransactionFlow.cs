using PremiersoftChallenge.SharedKernel.Exceptions;

namespace Domain.ValueObjects
{
    public record TransactionFlow
    {
        public string Value { get; }
        public string Description { get; }

        private TransactionFlow(string value, string description)
        {
            Value = value;
            Description = description;
        }

        public static TransactionFlow Credit => new("C", "Credit");
        public static TransactionFlow Debit => new("D", "Debit");

        public static TransactionFlow From(string value) =>
            value switch
            {
                "C" => Credit,
                "D" => Debit,
                _ => throw new DomainException("Invalid flow!")
            };

        public override string ToString() => Description;
    }
}

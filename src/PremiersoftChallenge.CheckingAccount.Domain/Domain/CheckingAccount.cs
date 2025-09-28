using Domain.Enums;
using PremiersoftChallenge.SharedKernel;
using PremiersoftChallenge.SharedKernel.Exceptions;

namespace Domain
{
    public interface ICheckingAccount
    {
        Guid Id { get; }
        long Number { get; }
        string Name { get; }
        CheckingAccountStatus Status { get; }
        string Password { get; }
        string Salt { get; }

        ICheckingAccount Inactivate();
    }

    public class CheckingAccount : Entity<Guid>, ICheckingAccount
    {
        public long Number { get; private set; }
        public string Name { get; private set; } = default!;
        public CheckingAccountStatus Status { get; private set; }
        public string Password { get; private set; } = default!;
        public string Salt { get; private set; } = default!;

        public static ICheckingAccount Create(long number, string name, string password, string salt)
        {
            return new CheckingAccount
            {
                Id = Guid.NewGuid(),
                Number = number,
                Name = name,
                Status = CheckingAccountStatus.Active,
                Password = password,
                Salt = salt
            };
        }

        public ICheckingAccount Inactivate()
        {
            if (Status == CheckingAccountStatus.Inactive)
                throw new DomainException("A conta deve estar ativa para ser inativada.");

            Status = CheckingAccountStatus.Inactive;

            return this;
        }
    }
}

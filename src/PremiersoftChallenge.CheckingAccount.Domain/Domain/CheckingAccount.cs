using Domain.Enums;
using PremiersoftChallenge.SharedKernel;

namespace Domain
{
    public interface ICheckingAccount
    {
        long Number { get; }
        string Name { get; }
        CheckingAccountStatus Status { get; }
        string Password { get; }
        string Salt { get; }
    }

    public class CheckingAccount : Entity<Guid>, ICheckingAccount
    {
        public long Number { get; private set; }
        public string Name { get; private set; } = default!;
        public CheckingAccountStatus Status { get; private set; }
        public string Password { get; private set; } = default!;
        public string Salt { get; private set; } = default!;

        public static ICheckingAccount Create(string name, string password, string salt)
        {
            return new CheckingAccount
            {
                Id = Guid.NewGuid(),
                Number = 0,
                Name = name,
                Status = CheckingAccountStatus.Active,
                Password = password,
                Salt = salt
            };
        }
    }
}

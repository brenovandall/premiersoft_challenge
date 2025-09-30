using Application.Authentication;
using Application.CheckingAccount.Commands.CreateCheckingAccount;
using Application.Data.Repository;
using Moq;
using PremiersoftChallenge.BuildingBlocks.Errors;

namespace Application.UnitTests
{
    public class CreateCheckingAccountTests
    {
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<ICheckingAccountRepository> _repositoryMock;
        private readonly CreateCheckingAccountHandler _handler;

        private const string RandomPassword = "123";

        public CreateCheckingAccountTests()
        {
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _repositoryMock = new Mock<ICheckingAccountRepository>();
            _handler = new CreateCheckingAccountHandler(_passwordHasherMock.Object, _repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenCpfIsInvalid()
        {
            var invalidCpf = "12345678900";
            var command = new CreateCheckingAccountCommand(invalidCpf, RandomPassword);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(CheckingAccountErrors.InvalidCpf, result.Error);
            _repositoryMock.Verify(r => r.Add(It.IsAny<Domain.CheckingAccount>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldCreateAccount_WhenCpfIsValid()
        {
            var validCpf = "16124022087";
            var command = new CreateCheckingAccountCommand(validCpf, RandomPassword);

            _passwordHasherMock.Setup(p => p.Hash(command.Password)).Returns("hashed-salt");
            _repositoryMock.Setup(r => r.Count()).Returns(0);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Value.AccountNumber);

            _repositoryMock.Verify(r => r.Add(It.Is<Domain.CheckingAccount>(
                c => c.Name == validCpf &&
                     c.Password == "hashed" &&
                     c.Salt == "salt"
            )), Times.Once);
        }
    }
}

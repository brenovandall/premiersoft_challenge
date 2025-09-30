using Application.Authentication;
using Application.CheckingAccount.Commands.LoginCheckingAccount;
using Application.Data.Repository;
using Moq;
using PremiersoftChallenge.BuildingBlocks.Errors;

namespace Application.UnitTests
{
    public class LoginCheckingAccountTests
    {
        private readonly Mock<ICheckingAccountRepository> _repositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<ITokenProvider> _tokenProviderMock;
        private readonly LoginCheckingAccountHandler _handler;

        public LoginCheckingAccountTests()
        {
            _repositoryMock = new Mock<ICheckingAccountRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _tokenProviderMock = new Mock<ITokenProvider>();

            _handler = new LoginCheckingAccountHandler(
                _repositoryMock.Object,
                _passwordHasherMock.Object,
                _tokenProviderMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenAccountNotFound()
        {
            var command = new LoginCheckingAccountCommand("aaa", "aaa");
            _repositoryMock.Setup(r => r.GetByAccountNumberOrName(command.Identifier))
                           .ReturnsAsync((Domain.CheckingAccount?)null);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(CheckingAccountErrors.InvalidCredentials, result.Error);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenPasswordIsInvalid()
        {
            var account = (Domain.CheckingAccount)Domain.CheckingAccount.Create(1, "aaa", "aaa", "aaa");
            var command = new LoginCheckingAccountCommand(account.Name, "aaa");

            _repositoryMock.Setup(r => r.GetByAccountNumberOrName(account.Name)).ReturnsAsync(account);
            _passwordHasherMock.Setup(p => p.Verify(command.Password, account.Password, account.Salt)).Returns(false);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(CheckingAccountErrors.InvalidCredentials, result.Error);
        }

        [Fact]
        public async Task Handle_ShouldReturnToken_WhenCredentialsAreValid()
        {
            var account = (Domain.CheckingAccount)Domain.CheckingAccount.Create(1, "aaa", "aaa", "aaa");
            var command = new LoginCheckingAccountCommand(account.Name, "bbb");
            const string fakeToken = "JWT_TOKEN";

            _repositoryMock.Setup(r => r.GetByAccountNumberOrName(account.Name)).ReturnsAsync(account);
            _passwordHasherMock.Setup(p => p.Verify(command.Password, account.Password, account.Salt)).Returns(true);
            _tokenProviderMock.Setup(t => t.Create(account)).Returns(fakeToken);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(fakeToken, result.Value);
        }
    }
}

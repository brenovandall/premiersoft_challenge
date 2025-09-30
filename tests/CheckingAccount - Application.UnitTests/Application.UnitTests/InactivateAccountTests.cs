using Application.Authentication;
using Application.CheckingAccount.Commands.InactivateAccount;
using Application.Data.Repository;
using Domain;
using Domain.Enums;
using Moq;
using PremiersoftChallenge.BuildingBlocks.Errors;
using PremiersoftChallenge.SharedKernel.Exceptions;

namespace Application.UnitTests
{
    public class InactivateAccountTests
    {
        private readonly Mock<ICheckingAccountRepository> _repositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<ILoggedContext> _loggedContextMock;
        private readonly InactivateAccountHandler _handler;

        private const string RandomPassword = "123";

        public InactivateAccountTests()
        {
            _repositoryMock = new Mock<ICheckingAccountRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _loggedContextMock = new Mock<ILoggedContext>();

            _handler = new InactivateAccountHandler(
                _repositoryMock.Object,
                _passwordHasherMock.Object,
                _loggedContextMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenAccountNotFound()
        {
            _loggedContextMock.Setup(l => l.Id).Returns(Guid.NewGuid());
            _repositoryMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Domain.CheckingAccount?)null);

            var command = new InactivateAccountCommand(RandomPassword);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(CheckingAccountErrors.InvalidCredentials, result.Error);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenAccountAlreadyInactive()
        {
            var account = (Domain.CheckingAccount)Domain.CheckingAccount.Create(1, "aaa", "aaa", "aaa");
            account.Inactivate();

            _loggedContextMock.Setup(l => l.Id).Returns(account.Id);
            _repositoryMock.Setup(r => r.GetById(account.Id)).Returns(account);

            var command = new InactivateAccountCommand(RandomPassword);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(CheckingAccountErrors.InvalidCredentials, result.Error);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenPasswordIsInvalid()
        {
            var account = (Domain.CheckingAccount)Domain.CheckingAccount.Create(1, "aaa", "aaa", "aaa");

            _loggedContextMock.Setup(l => l.Id).Returns(account.Id);
            _repositoryMock.Setup(r => r.GetById(account.Id)).Returns(account);

            _passwordHasherMock
                .Setup(p => p.Verify(RandomPassword, account.Password, account.Salt))
                .Returns(false);

            var command = new InactivateAccountCommand(RandomPassword);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(CheckingAccountErrors.InvalidCredentials, result.Error);
        }

        [Fact]
        public async Task Handle_ShouldInactivateAccount_WhenPasswordIsValid()
        {
            var account = (Domain.CheckingAccount)Domain.CheckingAccount.Create(1, "aaa", "aaa", "aaa");

            _loggedContextMock.Setup(l => l.Id).Returns(account.Id);
            _repositoryMock.Setup(r => r.GetById(account.Id)).Returns(account);

            _passwordHasherMock
                .Setup(p => p.Verify(RandomPassword, account.Password, account.Salt))
                .Returns(true);

            var command = new InactivateAccountCommand(RandomPassword);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.True(result.Value);

            _repositoryMock.Verify(r => r.Update(It.Is<Domain.CheckingAccount>(
                c => c.Status == CheckingAccountStatus.Inactive
            )), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenDomainExceptionOccurs()
        {
            var account = new Mock<ICheckingAccount>();
            account.Setup(a => a.Status).Returns(CheckingAccountStatus.Active);
            account.Setup(a => a.Inactivate()).Throws(new DomainException("Error description"));

            _loggedContextMock.Setup(l => l.Id).Returns(Guid.NewGuid());
            _repositoryMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns(account.Object);

            _passwordHasherMock
                .Setup(p => p.Verify(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            var command = new InactivateAccountCommand(RandomPassword);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal("BUSINESS_ERROR", result.Error.Code);
            Assert.Equal("Domain exception: Error description", result.Error.Description);
        }
    }
}

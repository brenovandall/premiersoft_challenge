using Application.Authentication;
using Application.Data.Repository;
using Application.Transaction.Commands.MakeTransaction;
using Moq;
using PremiersoftChallenge.BuildingBlocks.Errors;
using PremiersoftChallenge.SharedKernel.Exceptions;

namespace Application.UnitTests
{
    public class MakeTransactionTests
    {
        private readonly Mock<ICheckingAccountRepository> _accountRepoMock;
        private readonly Mock<ITransactionRepository> _transactionRepoMock;
        private readonly Mock<ILoggedContext> _loggedContextMock;
        private readonly MakeTransactionHandler _handler;

        private readonly Domain.CheckingAccount _activeAccount;

        public MakeTransactionTests()
        {
            _accountRepoMock = new Mock<ICheckingAccountRepository>();
            _transactionRepoMock = new Mock<ITransactionRepository>();
            _loggedContextMock = new Mock<ILoggedContext>();

            _handler = new MakeTransactionHandler(
                _accountRepoMock.Object,
                _transactionRepoMock.Object,
                _loggedContextMock.Object
            );

            _activeAccount = (Domain.CheckingAccount)Domain.CheckingAccount.Create(1, "aaa", "aaa", "aaa");

            _loggedContextMock.Setup(c => c.Id).Returns(_activeAccount.Id);
            _accountRepoMock.Setup(r => r.GetById(_activeAccount.Id)).Returns(_activeAccount);
        }

        [Fact]
        public async Task Handle_ShouldSucceed_WhenUsingLoggedAccount()
        {
            var command = new MakeTransactionCommand(Guid.NewGuid().ToString(), null, 100, "D");

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            _transactionRepoMock.Verify(r => r.Add(It.IsAny<Domain.Transaction>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldFail_WhenAccountNumberDifferentAndFlowIsNotC()
        {
            _accountRepoMock.Setup(r => r.GetByAccountNumberOrName("999")).Returns(_activeAccount);

            var command = new MakeTransactionCommand(Guid.NewGuid().ToString(), 999, 50, "D");
            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(TransactionErrors.InvalidFlowType, result.Error);
        }

        [Fact]
        public async Task Handle_ShouldSucceed_WhenAccountNumberDifferntAndFlowIsC()
        {
            var otherAccount = (Domain.CheckingAccount)Domain.CheckingAccount.Create(2, "aaa", "aaa", "aaa");
            _accountRepoMock.Setup(r => r.GetByAccountNumberOrName(It.IsAny<string>())).Returns(otherAccount);

            var command = new MakeTransactionCommand(Guid.NewGuid().ToString(), 2, 200, "C");
            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            _transactionRepoMock.Verify(r => r.Add(It.IsAny<Domain.Transaction>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldFail_WhenRequestIdIsNotGuid()
        {
            var command = new MakeTransactionCommand("fake", null, 100, "D");

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal("INVALID_OPERATION", result.Error.Code);
        }

        [Fact]
        public async Task Handle_ShouldFail_WhenAccountNotFound()
        {
            _accountRepoMock.Setup(r => r.GetById(_activeAccount.Id)).Returns((Domain.CheckingAccount?)null);

            var command = new MakeTransactionCommand(Guid.NewGuid().ToString(), null, 50, "D");
            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(CheckingAccountErrors.InvalidAccount, result.Error);
        }

        [Fact]
        public async Task Handle_ShouldFail_WhenAccountIsInactive()
        {
            var inactiveAccount = (Domain.CheckingAccount)Domain.CheckingAccount.Create(3, "aaa", "aaa", "aaa");
            inactiveAccount.Inactivate();
            _accountRepoMock.Setup(r => r.GetById(inactiveAccount.Id)).Returns(inactiveAccount);
            _loggedContextMock.Setup(c => c.Id).Returns(inactiveAccount.Id);

            var command = new MakeTransactionCommand(Guid.NewGuid().ToString(), null, 100, "D");
            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(CheckingAccountErrors.InvalidAccount, result.Error);
        }

        [Fact]
        public async Task Handle_ShouldFail_WhenInvalidValueExceptionThrown()
        {
            _transactionRepoMock
                .Setup(r => r.Add(It.IsAny<Domain.Transaction>()))
                .Throws(new InvalidValueException("Invalid value"));

            var command = new MakeTransactionCommand(Guid.NewGuid().ToString(), null, -999, "D");
            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(TransactionErrors.InvalidValue, result.Error);
        }
    }
}

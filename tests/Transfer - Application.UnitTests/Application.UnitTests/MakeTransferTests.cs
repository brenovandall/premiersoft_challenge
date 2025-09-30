using Application.Authentication;
using Application.Data.Repository;
using Application.Dto;
using Application.Services;
using Application.Transfer.Commands.MakeTransfer;
using Moq;
using PremiersoftChallenge.BuildingBlocks.Results;

namespace Application.UnitTests
{
    public class MakeTransferTests
    {
        private readonly Mock<ITransferRepository> _transferRepositoryMock;
        private readonly Mock<ITransferService> _transferServiceMock;
        private readonly Mock<ILoggedContext> _loggedContextMock;
        private readonly MakeTransferHandler _handler;

        public MakeTransferTests()
        {
            _transferRepositoryMock = new Mock<ITransferRepository>();
            _transferServiceMock = new Mock<ITransferService>();
            _loggedContextMock = new Mock<ILoggedContext>();

            _loggedContextMock.Setup(x => x.Id).Returns(Guid.NewGuid());

            _handler = new MakeTransferHandler(
                _transferRepositoryMock.Object,
                _transferServiceMock.Object,
                _loggedContextMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenTransferIsValid()
        {
            var command = new MakeTransferCommand(12345, 100.456313);

            _transferServiceMock
                .Setup(x => x.GetCheckingAccountIdByNumber(command.TargetAccountNumber))
                .ReturnsAsync(Result.Success(Guid.NewGuid()));
            _transferServiceMock
                .Setup(x => x.SendTransactionRequest(It.IsAny<string>(), null, 100.46, "D"))
                .ReturnsAsync(Result.Success());
            _transferServiceMock
                .Setup(x => x.SendTransactionRequest(It.IsAny<string>(), command.TargetAccountNumber, 100.46, "C"))
                .ReturnsAsync(Result.Success());

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            _transferRepositoryMock.Verify(x => x.Add(It.IsAny<Domain.Transfer>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenTargetAccountNotFound()
        {
            var command = new MakeTransferCommand(12345, 50);

            _transferServiceMock
                .Setup(x => x.GetCheckingAccountIdByNumber(command.TargetAccountNumber))
                .ReturnsAsync(Result.Failure<Guid>(Error.Failure("NOT_FOUND", "Conta corrente não encontrada.")));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            _transferRepositoryMock.Verify(x => x.Add(It.IsAny<Domain.Transfer>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldTriggerFallback_WhenSourceTransactionFails()
        {
            var command = new MakeTransferCommand(12345, 200);

            _transferServiceMock
                .Setup(x => x.GetCheckingAccountIdByNumber(command.TargetAccountNumber))
                .ReturnsAsync(Result.Success(Guid.NewGuid()));
            _transferServiceMock
                .Setup(x => x.SendTransactionRequest(It.IsAny<string>(), null, 200, "D"))
                .ReturnsAsync(Result.Failure(Error.Failure("DEBIT_FAILED", "Falha no débito.")));
            _transferServiceMock
                .Setup(x => x.GetTransactionById(It.IsAny<string>()))
                .ReturnsAsync(Result.Success(new TransactionDto()));
            _transferServiceMock
                .Setup(x => x.SendTransactionRequest(It.IsAny<string>(), null, 200, "C"))
                .ReturnsAsync(Result.Success());

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            _transferRepositoryMock.Verify(x => x.Add(It.IsAny<Domain.Transfer>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldTriggerFallback_WhenTargetTransactionFails()
        {
            var command = new MakeTransferCommand(54321, 300);

            _transferServiceMock
                .Setup(x => x.GetCheckingAccountIdByNumber(command.TargetAccountNumber))
                .ReturnsAsync(Result.Success(Guid.NewGuid()));
            _transferServiceMock
                .Setup(x => x.SendTransactionRequest(It.IsAny<string>(), null, 300, "D"))
                .ReturnsAsync(Result.Success());
            _transferServiceMock
                .Setup(x => x.SendTransactionRequest(It.IsAny<string>(), command.TargetAccountNumber, 300, "C"))
                .ReturnsAsync(Result.Failure(Error.Failure("CREDIT_FAILED", "Falha no crédito.")));
            _transferServiceMock
                .Setup(x => x.GetTransactionById(It.IsAny<string>()))
                .ReturnsAsync(Result.Success(new TransactionDto()));
            _transferServiceMock
                .Setup(x => x.SendTransactionRequest(It.IsAny<string>(), null, 300, "C"))
                .ReturnsAsync(Result.Success());

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            _transferRepositoryMock.Verify(x => x.Add(It.IsAny<Domain.Transfer>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenExceptionOccurs()
        {
            var command = new MakeTransferCommand(11111, 500);

            _transferServiceMock
                .Setup(x => x.GetCheckingAccountIdByNumber(command.TargetAccountNumber))
                .ThrowsAsync(new Exception("Erro inesperado"));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsFailure);
        }
    }
}

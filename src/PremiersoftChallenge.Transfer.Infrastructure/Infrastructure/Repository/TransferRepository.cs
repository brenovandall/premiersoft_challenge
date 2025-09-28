using Application.Data.Repository;
using Domain;
using Infrastructure.Abstractions.Commands;
using Infrastructure.Extensions;

namespace Infrastructure.Repository
{
    internal sealed class TransferRepository : ITransferRepository
    {
        private readonly ISqlRawCommandFactory _sqlRawCommandFactory;

        public TransferRepository(ISqlRawCommandFactory sqlRawCommandFactory)
        {
            _sqlRawCommandFactory = sqlRawCommandFactory;
        }

        public void Add(ITransfer transfer)
        {
            var sql = @"INSERT INTO transferencia VALUES (@id, @sourceCheckingAccountId,
@targetCheckingAccountId, @transactionDate, @value)";
            var param = new
            {
                id = transfer.Id,
                sourceCheckingAccountId = transfer.SourceCheckingAccountId,
                targetCheckingAccountId = transfer.TargetCheckingAccountId,
                transactionDate = transfer.TransactionDate.ToString("dd/MM/yyyy"),
                value = transfer.Value,
            };

            GetSqlCommandFactory().SetCommand(sql).Execute(param);
        }

        private ISqlRawCommand GetSqlCommandFactory()
        {
            var factory = _sqlRawCommandFactory.Create(OrmsProviders.Dapper);

            if (factory == null)
                throw new Exception(nameof(factory));

            return factory;
        }
    }
}

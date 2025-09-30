using Application.Data.Repository;
using Domain;
using PremiersoftChallenge.Data;
using PremiersoftChallenge.Data.Abstractions.Commands;

namespace Infrastructure.Repository
{
    internal sealed class TransactionRepository : ITransactionRepository
    {
        private readonly ISqlRawCommandFactory _sqlRawCommandFactory;

        public TransactionRepository(ISqlRawCommandFactory sqlRawCommandFactory)
        {
            _sqlRawCommandFactory = sqlRawCommandFactory;
        }

        public void Add(ITransaction transaction)
        {
            var sql = "INSERT INTO movimento VALUES (@id, @accountId, @transactionDate, @flow, @value)";
            var param = new
            {
                id = transaction.Id,
                accountId = transaction.CheckingAccountId,
                transactionDate = transaction.TransactionDate.ToString("dd/MM/yyyy"),
                flow = transaction.TransactionFlow.Value,
                value = transaction.Value
            };

            GetSqlCommandFactory().SetCommand(sql).Execute(param);
        }

        private ISqlRawCommand GetSqlCommandFactory()
        {
            var factory = _sqlRawCommandFactory.Create(OrmProviders.Dapper, DbStrategies.Sqlite);

            if (factory == null)
                throw new Exception(nameof(factory));

            return factory;
        }
    }
}

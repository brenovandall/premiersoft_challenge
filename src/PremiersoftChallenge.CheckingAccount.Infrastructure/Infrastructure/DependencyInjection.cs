using Application.Authentication;
using Application.Data.Repository;
using Dapper;
using Infrastructure.Abstractions.Commands;
using Infrastructure.Abstractions.Queries;
using Infrastructure.Authentication;
using Infrastructure.Extensions;
using Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenProvider, TokenProvider>();

            // factories
            services.AddScoped<ISqlRawCommandFactory, SqlRawCommandFactory>();
            services.AddScoped<ISqlRawCommand, DapperSqlRawCommand>();
            services.AddScoped<IQueryExecutorFactory, QueryExecutorFactory>();
            services.AddScoped<IQueryExecutor, DapperQueryExecutor>();
            
            // repositories
            services.AddScoped<ICheckingAccountRepository, CheckingAccountRepository>();

            RegisterDapperSqlMappingFields();

            return services;
        }

        private static void RegisterDapperSqlMappingFields()
        {
            SqlMapper.AddTypeHandler(new GuidStringHandler());
        }
    }
}

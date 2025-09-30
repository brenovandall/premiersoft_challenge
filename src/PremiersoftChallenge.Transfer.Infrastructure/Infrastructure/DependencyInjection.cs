using Application.Authentication;
using Application.Data.Repository;
using Application.Services;
using Infrastructure.Authentication;
using Infrastructure.Data;
using Infrastructure.Http;
using Infrastructure.Repository;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PremiersoftChallenge.Data.Abstractions.Commands;
using PremiersoftChallenge.Data.Abstractions.Queries;
using PremiersoftChallenge.Security;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISqlRawCommandFactory, SqlRawCommandFactory>();
            services.AddScoped<ISqlRawCommand, SqliteSqlRawCommand>();
            services.AddScoped<IQueryExecutorFactory, QueryExecutorFactory>();
            services.AddScoped<IQueryExecutor, SqliteQueryExecutor>();

            services.AddScoped<ITransferRepository, TransferRepository>();

            services.AddScoped<ITransferService, TransferService>();
            services.AddScoped<IIdempotencyService, IdempotencyService>();

            services.AddSecurityBaseAuthentication(configuration["Jwt:Secret"]!, configuration["Jwt:Issuer"], configuration["Jwt:Audience"]);
            services.AddSecurityBaseAuthorization();
            services.AddHttpContextAccessor();

            services.AddScoped<ILoggedContext, LoggedContext>();
            services.AddScoped<IApiCall, ApiCall>();

            return services;
        }
    }
}

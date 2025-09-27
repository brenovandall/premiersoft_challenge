using Application.Authentication;
using Application.Data.Repository;
using Dapper;
using Infrastructure.Abstractions.Commands;
using Infrastructure.Abstractions.Queries;
using Infrastructure.Authentication;
using Infrastructure.Extensions;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISqlRawCommandFactory, SqlRawCommandFactory>();
            services.AddScoped<ISqlRawCommand, DapperSqlRawCommand>();
            services.AddScoped<IQueryExecutorFactory, QueryExecutorFactory>();
            services.AddScoped<IQueryExecutor, DapperQueryExecutor>();
            
            services.AddScoped<ICheckingAccountRepository, CheckingAccountRepository>();

            services.AddInternalAuthentication(configuration);
            services.AddInternalAuthorization();

            services.AddDapperSqlMappingHandlers();

            return services;
        }

        private static IServiceCollection AddInternalAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!)),
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddHttpContextAccessor();
            services.AddScoped<ILoggedContext, LoggedContext>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<ITokenProvider, TokenProvider>();

            return services;
        }

        private static IServiceCollection AddInternalAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization();

            return services;
        }

        private static IServiceCollection AddDapperSqlMappingHandlers(this IServiceCollection services)
        {
            SqlMapper.AddTypeHandler(new GuidStringHandler());

            return services;
        }
    }
}

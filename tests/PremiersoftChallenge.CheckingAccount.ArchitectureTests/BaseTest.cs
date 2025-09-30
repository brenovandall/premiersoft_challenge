using Application.Dto;
using Infrastructure.Data;
using System.Reflection;

namespace PremiersoftChallenge.CheckingAccount.ArchitectureTests
{
    public abstract class BaseTest
    {
        protected static readonly Assembly DomainAssembly = typeof(Domain.CheckingAccount).Assembly;
        protected static readonly Assembly ApplicationAssembly = typeof(IdempotentDto).Assembly;
        protected static readonly Assembly InfrastructureAssembly = typeof(SqliteQueryExecutor).Assembly;
        protected static readonly Assembly PresentationAssembly = typeof(Program).Assembly;
    }
}

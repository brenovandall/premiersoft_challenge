using Application.Dto;
using Infrastructure.Data;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Reflection;

namespace PremiersoftChallenge.Transfer.ArchitectureTests
{
    public abstract class BaseTest
    {
        protected static readonly Assembly DomainAssembly = typeof(Domain.Transfer).Assembly;
        protected static readonly Assembly ApplicationAssembly = typeof(IdempotentDto).Assembly;
        protected static readonly Assembly InfrastructureAssembly = typeof(SqliteQueryExecutor).Assembly;
        protected static readonly Assembly PresentationAssembly = typeof(Program).Assembly;
    }
}

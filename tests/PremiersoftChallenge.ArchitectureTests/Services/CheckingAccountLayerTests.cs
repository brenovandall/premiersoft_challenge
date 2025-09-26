using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PremiersoftChallenge.ArchitectureTests.Services
{
    public class CheckingAccountLayerTests
    {

        private static class BaseLayers
        {
            protected static readonly Assembly DomainAssembly = typeof(User).Assembly;
            protected static readonly Assembly ApplicationAssembly = typeof(ICommand).Assembly;
            protected static readonly Assembly InfrastructureAssembly = typeof(ApplicationDbContext).Assembly;
            protected static readonly Assembly PresentationAssembly = typeof(Program).Assembly;
        }
    }
}

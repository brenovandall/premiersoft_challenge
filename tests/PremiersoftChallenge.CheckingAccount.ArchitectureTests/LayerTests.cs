using NetArchTest.Rules;

namespace PremiersoftChallenge.CheckingAccount.ArchitectureTests
{
    public class LayerTests : BaseTest
    {
        [Fact]
        public void Domain_Should_NotHaveDependencyOnApplication()
        {
            TestResult result = Types.InAssembly(DomainAssembly)
                .Should()
                .NotHaveDependencyOn(ApplicationAssembly.GetName().Name)
                .GetResult();

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public void DomainLayer_ShouldNotHaveDependencyOn_InfrastructureLayer()
        {
            TestResult result = Types.InAssembly(DomainAssembly)
                .Should()
                .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
                .GetResult();

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public void DomainLayer_ShouldNotHaveDependencyOn_PresentationLayer()
        {
            TestResult result = Types.InAssembly(DomainAssembly)
                .Should()
                .NotHaveDependencyOn(PresentationAssembly.GetName().Name)
                .GetResult();

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public void ApplicationLayer_ShouldNotHaveDependencyOn_InfrastructureLayer()
        {
            TestResult result = Types.InAssembly(ApplicationAssembly)
                .Should()
                .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
                .GetResult();

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public void ApplicationLayer_ShouldNotHaveDependencyOn_PresentationLayer()
        {
            TestResult result = Types.InAssembly(ApplicationAssembly)
                .Should()
                .NotHaveDependencyOn(PresentationAssembly.GetName().Name)
                .GetResult();

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public void InfrastructureLayer_ShouldNotHaveDependencyOn_PresentationLayer()
        {
            TestResult result = Types.InAssembly(InfrastructureAssembly)
                .Should()
                .NotHaveDependencyOn(PresentationAssembly.GetName().Name)
                .GetResult();

            Assert.True(result.IsSuccessful);
        }
    }
}

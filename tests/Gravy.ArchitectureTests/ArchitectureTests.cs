using NetArchTest.Rules;

namespace Gravy.ArchitectureTests;

public class ArchitectureTests
{
    private const string DomainNamespace = "Gravy.Domain";
    private const string ApplicationNamespace = "Gravy.Application";
    private const string InfrastructureNamespace = "Gravy.Infrastructure";
    private const string PersistenceNamespace = "Gravy.Persistence";
    private const string PresentationNamespace = "Gravy.Presentation";
    private const string AppNamespace = "Gravy.App";

    [Fact]
    public void Domain_Should_Not_HaveDependencyOnOtherObjects()
    {
        // Arrange
        var assembly = typeof(Gravy.Domain.AssemblyReference).Assembly;

        var otherProjects = new[]
        {
            ApplicationNamespace,
            InfrastructureNamespace,
            PersistenceNamespace,
            PresentationNamespace,
            AppNamespace
        };

        // Act
        var testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_Should_Not_HaveDependencyOnOtherObjects()
    {
        // Arrange
        var assembly = typeof(Gravy.Application.AssemblyReference).Assembly;

        var otherProjects = new[]
        {
            InfrastructureNamespace,
            PersistenceNamespace,
            PresentationNamespace,
            AppNamespace
        };

        // Act
        var testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Handlers_Should_Have_DependencyOnDomain()
    {
        // Arrange
        var assembly = typeof(Gravy.Application.AssemblyReference).Assembly;

        // Act
        var testResult = Types
            .InAssembly(assembly)
            .That()
            .HaveNameEndingWith($"Handler")
            .Should()
            .HaveDependencyOn(DomainNamespace)
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Infrastructure_Should_Not_HaveDependencyOnOtherObjects()
    {
        // Arrange
        var assembly = typeof(Gravy.Infrastructure.AssemblyReference).Assembly;

        var otherProjects = new[]
        {
            PersistenceNamespace,
            PresentationNamespace,
            AppNamespace
        };

        // Act
        var testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Persistence_Should_Not_HaveDependencyOnOtherObjects()
    {
        // Arrange
        var assembly = typeof(Persistence.AssemblyReference).Assembly;

        var otherProjects = new[]
        {
            InfrastructureNamespace,
            PresentationNamespace,
            AppNamespace
        };

        // Act
        var testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Presentation_Should_Not_HaveDependencyOnOtherObjects()
    {
        // Arrange
        var assembly = typeof(Presentation.AssemblyReference).Assembly;

        var otherProjects = new[]
        {
            AppNamespace
        };

        // Act
        var testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Controllers_Should_HaveDependencyOnMediatR()
    {
        // Arrange
        var assembly = typeof(Presentation.AssemblyReference).Assembly;

        // Act
        var testResult = Types
            .InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Controllers")
            .Should()
            .HaveDependencyOn("MediatR")
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue();
    }
}
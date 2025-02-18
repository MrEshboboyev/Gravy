using System.Reflection;

namespace Gravy.ArchitectureTests;

public abstract class BaseTest
{
    protected static readonly Assembly DomainAssembly = typeof(Gravy.Domain.AssemblyReference).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(Gravy.Application.AssemblyReference).Assembly;
    protected static readonly Assembly PersistenceAssembly = typeof(Persistence.AssemblyReference).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(Infrastructure.AssemblyReference).Assembly;
    protected static readonly Assembly PresentationAssembly = typeof(Presentation.AssemblyReference).Assembly;
}
using System.Reflection;

namespace Gravy.Application.Abstractions;

/// <summary> 
/// Provides a reference to the assembly containing application abstractions. 
/// This is used for assembly scanning and loading types or resources from the assembly. 
/// </summary>
public static class AssemblyReference
{
    // Holds a reference to the current assembly
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
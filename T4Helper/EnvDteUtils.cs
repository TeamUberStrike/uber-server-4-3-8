using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class EnvDteUtils
{
    //
    // 1. Replacement for GetParameterKind
    //
    public static ParameterKind GetParameterKind(ITypeSymbol type)
    {
        switch (type)
        {
            case INamedTypeSymbol named when named.TypeKind == TypeKind.Enum:
                return ParameterKind.Enum;

            case INamedTypeSymbol named when named.TypeKind == TypeKind.Struct:
                return ParameterKind.Struct;

            case INamedTypeSymbol named when named.TypeKind == TypeKind.Class:
                if (named.Name == "List")
                    return ParameterKind.List;
                if (named.Name == "Dictionary")
                    return ParameterKind.Dictionary;
                return ParameterKind.Class;

            case IArrayTypeSymbol _:
                return ParameterKind.Array;

            case ITypeSymbol primitive when primitive.SpecialType == SpecialType.System_String:
                return ParameterKind.String;
            case ITypeSymbol primitive when primitive.SpecialType == SpecialType.System_Byte:
                return ParameterKind.Byte;
            case ITypeSymbol primitive when primitive.SpecialType == SpecialType.System_Int16:
                return ParameterKind.Int16;
            case ITypeSymbol primitive when primitive.SpecialType == SpecialType.System_Int32:
                return ParameterKind.Int32;
            case ITypeSymbol primitive when primitive.SpecialType == SpecialType.System_Int64:
                return ParameterKind.Int64;
            case ITypeSymbol primitive when primitive.SpecialType == SpecialType.System_Single:
                return ParameterKind.Single;
            case ITypeSymbol primitive when primitive.SpecialType == SpecialType.System_Decimal:
                return ParameterKind.Decimal;
            case ITypeSymbol primitive when primitive.SpecialType == SpecialType.System_Boolean:
                return ParameterKind.Boolean;
            case ITypeSymbol primitive when primitive.SpecialType == SpecialType.System_Void:
                return ParameterKind.Void;

            default:
                throw new Exception(string.Format("Unsupported type: {0}", type.ToDisplayString()));
        }
    }

    //
    // 2. Replace FindProjectItem + ContainingProject
    //
    public static Project GetProjectContainingFile(string solutionPath, string filePath)
    {
        var workspace = MSBuildWorkspace.Create();
        try
        {
            var solution = workspace.OpenSolutionAsync(solutionPath).Result;

            foreach (var project in solution.Projects)
            {
                foreach (var document in project.Documents)
                {
                    if (string.Equals(
                        Path.GetFullPath(document.FilePath),
                        Path.GetFullPath(filePath),
                        StringComparison.OrdinalIgnoreCase))
                    {
                        return project;
                    }
                }
            }
        }
        finally
        {
            workspace.Dispose();
        }

        throw new Exception(string.Format("Project containing file '{0}' not found.", filePath));
    }

    //
    // 3. Replace GetProjectWithName
    //
    public static Project GetProjectWithName(string solutionPath, string projectName)
    {
        var workspace = MSBuildWorkspace.Create();
        try
        {
            var solution = workspace.OpenSolutionAsync(solutionPath).Result;

            var project = solution.Projects
                .FirstOrDefault(p => string.Equals(p.Name, projectName, StringComparison.OrdinalIgnoreCase));

            if (project == null)
                throw new Exception(string.Format("Project '{0}' not found.", projectName));

            return project;
        }
        finally
        {
            workspace.Dispose();
        }
    }

    //
    // 4. GetAllFunctions
    //
    public static List<IMethodSymbol> GetAllFunctions(INamedTypeSymbol type)
    {
        var members = type.GetMembers();
        var list = new List<IMethodSymbol>();

        foreach (var member in members)
        {
            var method = member as IMethodSymbol;
            if (method != null && method.MethodKind == MethodKind.Ordinary)
            {
                list.Add(method);
            }
        }

        return list;
    }

    //
    // 5. GetAllInterfaces (recursive)
    //
    public static List<INamedTypeSymbol> GetAllInterfaces(Project project)
    {
        var result = new List<INamedTypeSymbol>();
        var compilation = project.GetCompilationAsync().Result;

        CollectAllInterfaces(compilation.GlobalNamespace, result);

        return result;
    }

    // Helper recursive method to get all types including nested namespaces
    private static void CollectAllInterfaces(INamespaceSymbol ns, List<INamedTypeSymbol> collector)
    {
        foreach (var member in ns.GetMembers())
        {
            var namespaceSymbol = member as INamespaceSymbol;
            if (namespaceSymbol != null)
            {
                CollectAllInterfaces(namespaceSymbol, collector);
            }
            else
            {
                var namedType = member as INamedTypeSymbol;
                if (namedType != null && namedType.TypeKind == TypeKind.Interface)
                {
                    collector.Add(namedType);
                }
            }
        }
    }

    //
    // 6. GetAllScripts – all C# files in the project
    //
    public static List<Document> GetAllScripts(Project project)
    {
        var list = new List<Document>();

        foreach (var doc in project.Documents)
        {
            if (doc.SourceCodeKind == SourceCodeKind.Regular &&
                doc.FilePath != null &&
                doc.FilePath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
            {
                list.Add(doc);
            }
        }

        return list;
    }

    //
    // 7. HasAttributeWithName
    //
    public static bool HasAttributeWithName(INamedTypeSymbol type, string attributeName)
    {
        foreach (var attr in type.GetAttributes())
        {
            if (string.Equals(attr.AttributeClass?.Name, attributeName, StringComparison.Ordinal))
            {
                return true;
            }
        }
        return false;
    }
}

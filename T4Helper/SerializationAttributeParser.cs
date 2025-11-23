using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

public static class SerializationAttributeParser
{
    private const string SerializationAttribute = "SerializableAttribute"; // attribute class name

    public static ProjectModels CreateModelSerialization(Project roslynProject)
    {
        return GetAllModelsOfProject(roslynProject);
    }

    private static ProjectModels GetAllModelsOfProject(Project project)
    {
        var metaData = new ProjectModels();

        var compilationTask = project.GetCompilationAsync();
        compilationTask.Wait();
        var compilation = compilationTask.Result;

        // Recursively get all named types (classes, structs, enums) in the compilation
        IEnumerable<INamedTypeSymbol> allTypes = GetAllNamedTypes(compilation.GlobalNamespace);

        foreach (var typeSymbol in allTypes)
        {
            if (IsSerializable(typeSymbol))
            {
                if (typeSymbol.TypeKind == TypeKind.Class)
                {
                    metaData.Classes.Add(new ModelProperties(typeSymbol.Name));
                }
                else if (typeSymbol.TypeKind == TypeKind.Struct)
                {
                    metaData.Structs.Add(new ModelProperties(typeSymbol.Name));
                }
                else if (typeSymbol.TypeKind == TypeKind.Enum)
                {
                    // You can examine underlying type if needed, default is int
                    string enumUnderlyingType = typeSymbol.EnumUnderlyingType?.Name ?? "int";
                    metaData.Enums.Add(new EnumProperties(typeSymbol.Name, enumUnderlyingType));
                }
            }
        }

        return metaData;
    }

    private static bool IsSerializable(INamedTypeSymbol typeSymbol)
    {
        if (typeSymbol.TypeKind == TypeKind.Enum)
            return true;

        if (typeSymbol.TypeKind == TypeKind.Class)
        {
            foreach (var attr in typeSymbol.GetAttributes())
            {
                if (attr.AttributeClass != null && attr.AttributeClass.Name == SerializationAttribute)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static IEnumerable<INamedTypeSymbol> GetAllNamedTypes(INamespaceSymbol @namespace)
    {
        var result = new List<INamedTypeSymbol>();

        foreach (var type in @namespace.GetTypeMembers())
        {
            result.Add(type);
        }

        foreach (var childNs in @namespace.GetNamespaceMembers())
        {
            result.AddRange(GetAllNamedTypes(childNs));
        }

        return result;
    }
}

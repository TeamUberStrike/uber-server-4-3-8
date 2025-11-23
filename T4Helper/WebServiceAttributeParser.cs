using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static class WebServiceAttributeParser
{
    private const string WebServiceAttribute = "CmuneWebServiceInterfaceAttribute";
    private const string EncryptionAttribute = "DontEncryptMethodAttribute";
    private const string SerializationProperty = "UseBinaryProtocol";

    public static List<InterfaceProperties> GetProjectInterfaces(Microsoft.CodeAnalysis.Project project)
    {
        var interfaces = new List<InterfaceProperties>();

        try
        {
            var compilation = project.GetCompilationAsync().Result;
            if (compilation == null)
                throw new Exception("Failed to get compilation for project.");

            // Collect all interface symbols in the project
            var allInterfaces = GetAllInterfaces(compilation.GlobalNamespace);

            foreach (var iface in allInterfaces)
            {
                if (HasAttribute(iface, WebServiceAttribute))
                {
                    var ip = GetInterfaceProperties(iface);
                    interfaces.Add(ip);
                }
            }

            // Ensure unique method suffixes for duplicates
            foreach (var intfc in interfaces)
            {
                var duplicates = intfc.Methods.GroupBy(m => m.Name)
                                             .Where(g => g.Count() > 1);
                foreach (var group in duplicates)
                {
                    int index = 1;
                    foreach (var method in group)
                    {
                        method.Suffix = "_" + index++;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            T4Utils.Print(ex.GetType() + ": " + ex.Message);
            T4Utils.Print(ex.StackTrace);
            throw;
        }

        return interfaces;
    }

    private static List<INamedTypeSymbol> GetAllInterfaces(INamespaceSymbol ns)
    {
        var list = new List<INamedTypeSymbol>();

        foreach (var member in ns.GetMembers())
        {
            if (member is INamespaceSymbol nestedNs)
            {
                list.AddRange(GetAllInterfaces(nestedNs));
            }
            else if (member is INamedTypeSymbol namedType && namedType.TypeKind == TypeKind.Interface)
            {
                list.Add(namedType);
            }
        }

        return list;
    }

    private static InterfaceProperties GetInterfaceProperties(INamedTypeSymbol iface)
    {
        var inter = new InterfaceProperties(iface.Name, iface.ToDisplayString());

        inter.UseBinarySerialization = IsWebServiceBinarySerializationEnabled(iface);

        var methods = iface.GetMembers()
            .OfType<IMethodSymbol>()
            .Where(m => m.MethodKind == MethodKind.Ordinary);

        foreach (var method in methods)
        {
            if (method.ReturnType.ToDisplayString() == "byte[]")
            {
                T4Utils.Comment("Ignored : " + inter.Name + "." + method.Name + " with return type byte[]");
                continue;
            }

            var argKinds = new List<ParameterKind>();
            var args = new List<KeyValuePair<string, string>>();

            foreach (var param in method.Parameters)
            {
                args.Add(new KeyValuePair<string, string>(param.Type.ToDisplayString(), param.Name));
                argKinds.Add(EnvDteUtils.GetParameterKind(param.Type));
            }

            inter.Methods.Add(new MethodProperties()
            {
                Name = method.Name,
                ReturnType = method.ReturnType.ToDisplayString(),
                ReturnKind = EnvDteUtils.GetParameterKind(method.ReturnType),
                ObsoleteMessage = GetObsoleteMessage(method),
                IsObsolete = IsObsolete(method),
                Arguments = args,
                ArgumentKinds = argKinds,
                EnableEncryption = IsWebServiceEncryptionEnabled(method)
            });
        }

        return inter;
    }

    private static bool HasAttribute(ISymbol symbol, string attributeName)
    {
        return symbol.GetAttributes()
            .Any(a => a.AttributeClass != null &&
                      a.AttributeClass.Name == attributeName);
    }

    private static bool IsWebServiceEncryptionEnabled(IMethodSymbol method)
    {
        // Disable encryption if method has DontEncryptMethodAttribute
        return !HasAttribute(method, EncryptionAttribute);
    }

    private static bool IsWebServiceBinarySerializationEnabled(INamedTypeSymbol iface)
    {
        // Look for CmuneWebServiceInterface attribute with UseBinaryProtocol=false
        var attr = iface.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == WebServiceAttribute);

        if (attr == null)
            return true;

        // Find if named argument UseBinaryProtocol is false
        foreach (var arg in attr.NamedArguments)
        {
            if (arg.Key == SerializationProperty && arg.Value.Value is bool b)
            {
                return b; // true or false as per attribute property
            }
        }

        return true;
    }

    private static bool IsObsolete(IMethodSymbol method)
    {
        return HasAttribute(method, "ObsoleteAttribute");
    }

    private static string GetObsoleteMessage(IMethodSymbol method)
    {
        var attr = method.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == "ObsoleteAttribute");

        if (attr == null)
            return string.Empty;

        if (attr.ConstructorArguments.Length > 0)
        {
            var msg = attr.ConstructorArguments[0].Value as string;
            return msg ?? string.Empty;
        }

        return string.Empty;
    }
}

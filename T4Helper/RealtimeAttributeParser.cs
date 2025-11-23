using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

public static class RealtimeAttributeParser
{
    public static RpcInterfaceCollection GetRpcInterfaces(Project project)
    {
        var metaData = new RpcInterfaceCollection();

        var compilationTask = project.GetCompilationAsync();
        compilationTask.Wait();
        var compilation = compilationTask.Result;

        // Get all interfaces recursively
        var interfaces = GetAllInterfaces(compilation.GlobalNamespace);

        foreach (var iface in interfaces)
        {
            if (HasAttributeWithName(iface, "RoomEvents"))
                metaData.RoomEvents.Add(GetRpcInterfaceView(iface));
            else if (HasAttributeWithName(iface, "RoomOperations"))
                metaData.RoomOperations.Add(GetRpcInterfaceView(iface));
            else if (HasAttributeWithName(iface, "PeerOperations"))
                metaData.ServerOperations.Add(GetRpcInterfaceView(iface));
            else if (HasAttributeWithName(iface, "PeerEvents"))
                metaData.ServerEvents.Add(GetRpcInterfaceView(iface));
        }

        return metaData;
    }

    private static List<INamedTypeSymbol> GetAllInterfaces(INamespaceSymbol ns)
    {
        var list = new List<INamedTypeSymbol>();

        foreach (var type in ns.GetTypeMembers())
        {
            if (type.TypeKind == TypeKind.Interface)
                list.Add(type);
        }

        foreach (var childNs in ns.GetNamespaceMembers())
        {
            list.AddRange(GetAllInterfaces(childNs));
        }

        return list;
    }

    private static bool HasAttributeWithName(INamedTypeSymbol symbol, string attributeName)
    {
        foreach (var attr in symbol.GetAttributes())
        {
            if (attr.AttributeClass != null && attr.AttributeClass.Name == attributeName)
                return true;
        }
        return false;
    }

    private static RpcInterfaceView GetRpcInterfaceView(INamedTypeSymbol iface)
    {
        var view = new RpcInterfaceView(iface.Name);

        if (iface.Interfaces.Length > 0)
            view.Interface = iface.Interfaces[0].Name;

        int methodId = 0;

        foreach (var member in iface.GetMembers().OfType<IMethodSymbol>())
        {
            if (member.MethodKind != MethodKind.Ordinary)
                continue;

            var arguments = new List<KeyValuePair<string, string>>();
            var argKinds = new List<ParameterKind>();

            foreach (var param in member.Parameters)
            {
                arguments.Add(new KeyValuePair<string, string>(param.Type.ToDisplayString(), param.Name));
                argKinds.Add(EnvDteUtils.GetParameterKind(param.Type));
            }

            view.NetworkMethods.Add(new NetworkMethod(member.Name, ++methodId)
            {
                Arguments = arguments,
                ArgumentKinds = argKinds,
            });
        }

        view.MethodIdOffset = methodId;

        return view;
    }
}

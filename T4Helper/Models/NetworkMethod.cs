using System.Collections.Generic;

public class NetworkMethod
{
    public int MethodId { get; set; }
    public string Name { get; private set; }
    public List<KeyValuePair<string, string>> Arguments { get; set; }
    public List<ParameterKind> ArgumentKinds { get; set; }

    public NetworkMethod(string name, int methodId)
    {
        Name = name;
        MethodId = methodId;
    }

    public string NamePrint(string prefix = "")
    {
        string arg = string.Empty;
        foreach (var v in Arguments)
        {
            arg += v.Value + ", ";
        }
        return Arguments.Count > 0 ? prefix + arg.Trim(", ".ToCharArray()) : arg.Trim(", ".ToCharArray());
    }

    public string TypePrint()
    {
        string arg = string.Empty;
        foreach (var v in Arguments)
        {
            arg += v.Key + ", ";
        }
        return arg.Trim(", ".ToCharArray());
    }

    public string ArgPrint(string prefix = "")
    {
        string arg = string.Empty;
        foreach (var v in Arguments)
        {
            arg += v.Key + " " + v.Value + ", ";
        }
        return Arguments.Count > 0 ? prefix + arg.Trim(", ".ToCharArray()) : arg.Trim(", ".ToCharArray());
    }
}
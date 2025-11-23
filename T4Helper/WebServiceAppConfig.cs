using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

public static class WebServiceAppConfig
{
    const string projectName = "UberStrike.DataCenter.WebService";
    const string binding = "basicHttpBinding";

    public static void Update(Workspace workspace, string configFileName, string baseUrl = "")
    {
        // Find the project by name
        var project = workspace.CurrentSolution.Projects.FirstOrDefault(p => p.Name == projectName);
        if (project == null)
            throw new Exception($"Project '{projectName}' not found.");

        // Find the document (file) matching configFileName
        var document = project.Documents.FirstOrDefault(d => d.Name.EndsWith(configFileName, StringComparison.OrdinalIgnoreCase));
        if (document == null)
            throw new Exception($"Config file '{configFileName}' not found in project '{projectName}'.");

        string path = document.FilePath;
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            throw new Exception($"Config file path '{path}' does not exist.");

        string content;
        using (var reader = new StreamReader(path))
        {
            content = reader.ReadToEnd();
        }

        XElement e = XElement.Parse(content);

        // Remove old system.serviceModel elements and previous comments
        foreach (var n in e.Elements().Where(x => x.Name.LocalName == "system.serviceModel").ToList())
        {
            var comment = n.PreviousNode as XComment;
            comment?.Remove();
            n.Remove();
        }

        XElement main = new XElement("system.serviceModel");
        XElement serviceHostingEnvironment = new XElement("serviceHostingEnvironment", new XAttribute("multipleSiteBindingsEnabled", "true"));
        main.Add(serviceHostingEnvironment);

        XElement services = new XElement("services");

        // You need to replace this with your Roslyn-based interface fetch or another method 
        var interfaces = WebServiceAttributeParser.GetProjectInterfaces(project); // This method needs to accept Roslyn Project now!

        foreach (var i in interfaces)
        {
            if (i.UseBinarySerialization)
            {
                services.Add(CreateWebServiceElement(i.ClassName, i.Name, baseUrl, "basicHttpBinding", "", true));
            }
            else
            {
                services.Add(CreateWebServiceElement(i.ClassName, i.Name, baseUrl, "webHttpBinding", "DescriptorBehaviour", false));
            }
        }

        main.Add(services);

        // Add bindings and behaviors as you have in your code (same as original)

        // ... omitted for brevity, copy your existing XElement code here ...

        e.Add(new XComment(T4Utils.XmlHeader()));
        e.Add(main);

        using (var writer = new StreamWriter(path))
        {
            writer.Write(e.ToString());
        }

        // Optional logging
        // T4Utils.Comment(path + " was updated " + DateTime.Now);
    }

    private static XElement CreateWebServiceElement(string service, string contract, string baseUrl, string binding, string behaviour, bool serializedWebService = true)
    {
        string serviceUrl = String.Empty;
        if (serializedWebService)
        {
            contract = projectName + "." + WebServiceConstants.InternalWebServiceNamespace + "." + contract + WebServiceConstants.InternalWebServiceSuffix;
            service = projectName + "." + WebServiceConstants.InternalWebServiceNamespace + "." + service + WebServiceConstants.InternalWebServiceSuffix;
            serviceUrl = baseUrl + "/" + service + WebServiceConstants.WebServiceEndpointSuffix + "/";
        }
        else
        {
            contract = projectName + ".Interfaces." + contract;
            service = projectName + "." + service;
            serviceUrl = baseUrl + "/" + service + WebServiceConstants.WebServiceEndpointSuffix + "/";
        }

        XElement serviceElement = new XElement("service", new XAttribute("name", service));
        XElement endpoint = null;
        if (string.IsNullOrEmpty(behaviour))
        {
            endpoint = new XElement("endpoint",
                new XAttribute("address", ""),
                new XAttribute("binding", binding),
                new XAttribute("bindingConfiguration", binding),
                new XAttribute("contract", contract));
        }
        else
        {
            endpoint = new XElement("endpoint",
                new XAttribute("address", ""),
                new XAttribute("behaviorConfiguration", behaviour),
                new XAttribute("binding", binding),
                new XAttribute("contract", contract));
        }
        serviceElement.Add(endpoint);

        XElement host = new XElement("host");
        XElement baseAddresses = new XElement("baseAddresses");
        baseAddresses.Add(new XElement("add", new XAttribute("baseAddress", service.Contains("CrossDomain") ? baseUrl : serviceUrl)));
        host.Add(baseAddresses);
        serviceElement.Add(host);

        return serviceElement;
    }
}

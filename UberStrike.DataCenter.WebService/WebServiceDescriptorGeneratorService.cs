using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using UberStrike.DataCenter.WebService.Attributes;

namespace UberStrike.DataCenter.WebService
{
    public static class WebServiceDescriptorGeneratorService
    {
        public static XElement Generate(Assembly[] assemblies)
        {
            XElement xml = new XElement("services");

            var interfaces = new List<Type>();
            foreach (var a in assemblies)
            {
                try
                {
                    foreach (var t in a.GetTypes())
                    {
                        if (t.IsInterface && t.GetCustomAttributes(typeof(CmuneWebServiceInterfaceAttribute), false).Length > 0)
                        {
                            interfaces.Add(t);
                        }
                    }
                }
                catch
                {
                    // Assembly is probably locked
                }
            }

            foreach (var interfac in interfaces)
            {
                XElement xmlService = new XElement("service", new XAttribute("name", interfac.Name+"Contract"));
                XElement xmlOperations = new XElement("operations");

                // Find all webservice methods
                var methods = interfac.GetMethods();//.Where(
                   // m => m.GetCustomAttributes(typeof(CmuneWebServiceMethodAttribute), true).Length > 0);

                foreach (var method in methods)
                {
                    XElement xmlOperation = new XElement("operation",
                        new XAttribute("name", method.Name),
                        new XAttribute("returnType", method.ReturnType.ToString().Replace("`1", "").Replace('[', '<').Replace(']', '>')));


                    XElement xmlParameters = new XElement("parameters");

                    var parameters = method.GetParameters();
                    foreach (var parameter in parameters)
                    {
                        xmlParameters.Add(new XElement("parameter",
                            new XAttribute("name", parameter.Name),
                            new XAttribute("type", parameter.ParameterType.ToString().Replace("`1", "").Replace('[', '<').Replace(']', '>'))));
                    }

                    xmlOperation.Add(xmlParameters);
                    xmlOperations.Add(xmlOperation);
                }

                xmlService.Add(xmlOperations);
                xml.Add(xmlService);
            }

            return xml;
        }
    }
}

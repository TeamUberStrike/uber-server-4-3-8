// -----------------------------------------------------------------------
// <copyright file="ManagedServerIpCollectionModel.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Cmune.Instrumentation.Monitoring.Common.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    [XmlRootAttribute(Namespace = "", ElementName = "", IsNullable = true)]
    public class ManagedServerIpCollectionModel
    {
        public List<ManagedServerIpModel> ManagedServerIpList { get; set; }
    }
}

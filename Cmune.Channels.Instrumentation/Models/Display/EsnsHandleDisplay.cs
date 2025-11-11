using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Utils;

namespace Cmune.Channels.Instrumentation.Models.Display
{
    public class EsnsHandleDisplay
    {
        public string EsnsName { get; set; }
        public string EsnsProfileLink { get; set; }
        public string EsnsMemberId { get; set; }

        public EsnsHandleDisplay(EsnsType esnsType, string esnsMemberId)
        {
            this.EsnsName = esnsType.ToString();
            this.EsnsMemberId = esnsMemberId;

            if (CommonConfig.EsnsProfilesUrl.ContainsKey(esnsType) && !CommonConfig.EsnsProfilesUrl[esnsType].IsNullOrFullyEmpty())
            {
                this.EsnsProfileLink = CommonConfig.EsnsProfilesUrl[esnsType] + esnsMemberId;
            }
            else
            {
                // TODO => What link should we redirect to?
            }
        }
    }

}
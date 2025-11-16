using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.Common.Utils;
using UberStrike.DataCenter.DataAccess;
using Cmune.Channels.Instrumentation.Utils.Extensions;
using UberStrike.Core.Types;
using Newtonsoft.Json;

namespace Cmune.Channels.Instrumentation.Helper
{
    public static class ChannelVersionHelper
    {
        public static List<SelectListItem> ListOfChannelTypeToSelectItems(this HtmlHelper html, List<ChannelType> channelTypes)
        {
            var toto = from ct in channelTypes select new SelectListItem() { Text = ct.ToString(), Value = ((int)ct).ToString() };
            return toto.ToList();
        }

        public static List<SelectListItem> ListOfChannelTypeToSelectItems(this HtmlHelper html)
        {
            var listOfChannelType = new List<SelectListItem>();
            foreach (var channel in CommonConfig.ActiveChannels)
            {
                listOfChannelType.Add(new SelectListItem() { Text = channel.ToString(), Value = ((int)channel).ToString() });
            }
            return listOfChannelType;
        }

        public static HtmlString ChannelTypesToJson(this HtmlHelper html, List<ChannelType> channelsWithUniqueApplicationVersion)
        {
            return new HtmlString(JsonConvert.SerializeObject(channelsWithUniqueApplicationVersion));
        }

        public static List<SelectListItem> ListOfChannelElementToSelectItems(this HtmlHelper html, List<ChannelElement> channelTypes)
        {
            var toto = from ct in channelTypes select new SelectListItem() { Text = ct.ToString(), Value = ((int)ct).ToString() };
            return toto.ToList();
        }

        public static List<SelectListItem> ListOfPhotonGroupToSelectItems(this HtmlHelper html, List<PhotonsGroup> photonGroupList)
        {
            List<SelectListItem> photonsGroupDropDownList = new List<SelectListItem>();

            foreach (PhotonsGroup photonsGroup in photonGroupList)
            {
                SelectListItem listItem = new SelectListItem() { Text = photonsGroup.Name, Value = photonsGroup.PhotonsGroupID.ToString() };
                photonsGroupDropDownList.Add(listItem);
            }

            return photonsGroupDropDownList;
        }


    }
}
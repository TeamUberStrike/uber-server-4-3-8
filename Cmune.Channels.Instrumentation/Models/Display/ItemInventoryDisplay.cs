using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Cmune.DataCenter.Common.Entities;

namespace Cmune.Channels.Instrumentation.Models.Display
{
    public class ItemInventoryDisplay
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public string Expiration { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string AmountRemaining { get; set; }

        public ItemInventoryDisplay(int itemId, string name, DateTime? expirationDate, DateTime now, int amountRemaining)
        {
            this.ItemId = itemId;
            this.Name = name;
            this.ExpirationDate = expirationDate;
            this.AmountRemaining = "na";

            if (amountRemaining != CommonConfig.ItemMallFieldDisable)
            {
                this.AmountRemaining = amountRemaining.ToString();
            }

            if (expirationDate == null)
            {
                this.Expiration = "permanent";
            }
            else
            {
                bool remainingMinutesOnly = false;
                TimeSpan timeRemaining = ((DateTime)expirationDate).Subtract(now);
                StringBuilder expirationText = new StringBuilder();

                expirationText.Append("<span title=\"");
                expirationText.Append(((DateTime)expirationDate).ToString("MM/dd/yyyy HH:mm:ss"));
                expirationText.Append("\">");

                if (timeRemaining.TotalDays > 1)
                {
                    expirationText.Append((int)timeRemaining.TotalDays);
                    expirationText.Append(" days");
                }

                if (timeRemaining.Hours > 0)
                {
                    if (timeRemaining.TotalDays > 1)
                    {
                        expirationText.Append(" and");
                    }

                    expirationText.Append(" ");
                    expirationText.Append(timeRemaining.Hours);
                    expirationText.Append(" hour");

                    if (timeRemaining.Hours > 1)
                    {
                        expirationText.Append("s");
                    }
                }

                if ((int)timeRemaining.TotalDays == 0 && timeRemaining.Hours == 0 && timeRemaining.TotalSeconds > 0)
                {
                    int minutes = (int)timeRemaining.TotalSeconds / 60;
                    expirationText.Append(minutes);
                    expirationText.Append(" minute");

                    if (minutes > 1)
                    {
                        expirationText.Append("s");
                    }

                    remainingMinutesOnly = true;
                }
                else if (timeRemaining.TotalDays == 0 && timeRemaining.Hours == 0 && timeRemaining.Seconds == 0)
                {
                    expirationText.Append("expired");
                }

                if (timeRemaining.TotalDays > 1 || timeRemaining.Hours > 0 || remainingMinutesOnly)
                {
                    expirationText.Append(" left");
                }

                expirationText.Append("</span>");

                this.Expiration = expirationText.ToString();
            }
        }
    }
}
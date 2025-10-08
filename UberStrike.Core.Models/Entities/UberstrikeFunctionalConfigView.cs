using System;
using System.Text;

namespace UberStrike.DataCenter.Common.Entities
{
    public class UberstrikeFunctionalConfigView
    {
        public int LevelRequired { get; set; }

        public override string ToString()
        {
            StringBuilder configDisplay = new StringBuilder();

            configDisplay.Append("[UberstrikeFunctionalConfigView: ");
            configDisplay.Append("]");

            return configDisplay.ToString();
        }
    }
}

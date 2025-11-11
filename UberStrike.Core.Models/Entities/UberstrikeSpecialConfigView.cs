using System;
using System.Text;

namespace UberStrike.DataCenter.Common.Entities
{
    public class UberstrikeSpecialConfigView
    {
        #region Properties

        public int LevelRequired { get; set; }

        #endregion Properties

        #region Constructors

        public UberstrikeSpecialConfigView()
        {
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            StringBuilder configDisplay = new StringBuilder();

            configDisplay.Append("[UberstrikeSpecialConfigView: ");
            configDisplay.Append("]");

            return configDisplay.ToString();
        }

        #endregion Methods
    }
}
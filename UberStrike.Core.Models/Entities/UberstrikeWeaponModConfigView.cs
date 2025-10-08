using System;
using System.Text;

namespace UberStrike.DataCenter.Common.Entities
{
    public class UberstrikeWeaponModConfigView
    {
        #region Properties

        public int LevelRequired { get; set; }

        #endregion Properties

        #region Constructors

        public UberstrikeWeaponModConfigView()
        {
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            StringBuilder configDisplay = new StringBuilder();

            configDisplay.Append("[UberstrikeWeaponModConfigView: ");
            configDisplay.Append("]");

            return configDisplay.ToString();
        }

        #endregion Methods
    }
}
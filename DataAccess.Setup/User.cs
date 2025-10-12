using System;

namespace UberStrike.DataCenter.DataAccess
{
    /// <summary>
    /// 
    /// </summary>
    public partial class User
    {
        /// <summary>
        /// Precision Display
        /// </summary>
        public String PrecisionDisplay
        {
            get
            {
                if (this._Shots > 0)
                {
                    return Math.Round((double)this._Hits / (double)this._Shots * 100, 1).ToString();
                }
                else
                {
                    return "0";
                }
            }
        }
    }
}

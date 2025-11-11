using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.DataCenter.Common.Entities
{
    [Serializable]
    public class AccountCompletionResultView
    {
        #region Properties

        public int Result { get; set; }
        public Dictionary<int, int> ItemsAttributed { get; set; }
        public List<string> NonDuplicateNames { get; set; }

        #endregion

        #region Constructors

        public AccountCompletionResultView()
        {
            ItemsAttributed = new Dictionary<int, int>();
            NonDuplicateNames = new List<string>();
        }

        public AccountCompletionResultView(int result)
            : this()
        {
            this.Result = result;
        }

        public AccountCompletionResultView(int result, Dictionary<int, int> itemsAttributed, List<string> nonDuplicateNames)
        {
            this.Result = result;
            this.ItemsAttributed = itemsAttributed;
            this.NonDuplicateNames = nonDuplicateNames;
        }

        #endregion
    }
}
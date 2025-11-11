using System.Collections.Generic;

namespace Cmune.Channels.Instrumentation.Business
{
    public class ItemCache
    {
        #region Properties

        Dictionary<int, string> ItemsName { get; set; }

        #endregion

        #region Constructors

        public ItemCache(int applicationId, bool getDeprecatedItems = false)
        {
            ItemsName = AdminCache.LoadItemNames(applicationId, getDeprecatedItems);
        }

        public string GetItemName(int itemId)
        {
            string itemName = "Non existing item";

            if (ItemsName != null && ItemsName.ContainsKey(itemId))
            {
                itemName = ItemsName[itemId];
            }

            return itemName;
        }

        #endregion
    }
}
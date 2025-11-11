using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cmune.Channels.Instrumentation.Models
{
    public class ItemSyncModel
    {
        public int ItemId { get; private set; }
        public string DevItemName { get; private set; }
        public List<ItemSyncFieldModel<Object>> Fields { get; private set; }

        public ItemSyncModel(int itemId, string itemName, List<ItemSyncFieldModel<Object>> fields)
        {
            ItemId = itemId;
            DevItemName = DevItemName;
            Fields = fields;
        }
    }

    public class ItemSyncFieldModel<T>
    {
        public string Name { get; private set; }
        public T DevValue { get; private set; }
        public T ProdValue { get; private set; }
    }
}
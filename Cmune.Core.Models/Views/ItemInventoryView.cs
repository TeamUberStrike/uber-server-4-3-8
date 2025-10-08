using System;

namespace Cmune.DataCenter.Common.Entities
{
    [Serializable]
    public class ItemInventoryView
    {
        #region Properties

        public int Cmid { get; set; }

        public int ItemId { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public int AmountRemaining { get; set; }

        #endregion Properties

        #region Constructors

        public ItemInventoryView()
        { }

        public ItemInventoryView(int itemId, DateTime? expirationDate, int amountRemaining)
        {
            this.ItemId = itemId;
            this.ExpirationDate = expirationDate;
            this.AmountRemaining = amountRemaining;
        }

        public ItemInventoryView(int itemId, DateTime? expirationDate, int amountRemaining, int cmid)
            : this(itemId, expirationDate, amountRemaining)
        {
            this.Cmid = cmid;
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            string liveInventoryDisplay = "[LiveInventoryView: ";
            liveInventoryDisplay += "[Item Id: " + this.ItemId + "]";
            liveInventoryDisplay += "[Expiration date: " + this.ExpirationDate + "]";
            liveInventoryDisplay += "[Amount remaining:" + this.AmountRemaining + "]";
            liveInventoryDisplay += "]";

            return liveInventoryDisplay;
        }

        #endregion Methods
    }
}
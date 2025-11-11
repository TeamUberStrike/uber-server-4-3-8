using System.Text;
using System.Collections.Generic;
using System.Reflection;

namespace Cmune.DataCenter.Common.Entities
{
    /// <summary>
    /// Item view
    /// </summary>
    public class ItemView
    {
        #region Properties

        public int ItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int CreditsPerDay { get; set; }
        public int PointsPerDay { get; set; }
        public int PermanentPoints { get; set; }
        public int PermanentCredits { get; set; }

        public bool IsDisable { get; set; }
        public bool IsForSale { get; set; }
        public bool IsNew { get; set; }
        public bool IsPopular { get; set; }
        public bool IsFeatured { get; set; }

        public PurchaseType PurchaseType { get; set; }

        public int TypeId { get; set; }
        public int ClassId { get; set; }

        public int AmountRemaining { get; set; }
        public int PackOneAmount { get; set; }
        public int PackTwoAmount { get; set; }
        public int PackThreeAmount { get; set; }

        public bool Enable1Day { get; set; }
        public bool Enable7Days { get; set; }
        public bool Enable30Days { get; set; }
        public bool Enable90Days { get; set; }

        public int MaximumDurationDays { get; set; }
        public int MaximumOwnableAmount { get; set; }

        public Dictionary<string, string> CustomProperties { get; set; }

        public bool EnablePermanent
        {
            get
            {
                return PermanentCredits != CommonConfig.ItemMallFieldDisable || PermanentPoints != CommonConfig.ItemMallFieldDisable;
            }
        }

        #endregion Properties

        #region Constructors

        public ItemView() { }

        protected ItemView(ItemView itemView)
        {
            this.AmountRemaining = itemView.AmountRemaining;
            this.ClassId = itemView.ClassId;
            this.CreditsPerDay = itemView.CreditsPerDay;
            this.Description = itemView.Description;
            this.IsFeatured = itemView.IsFeatured;
            this.IsForSale = itemView.IsForSale;
            this.IsNew = itemView.IsNew;
            this.IsPopular = itemView.IsPopular;
            this.ItemId = itemView.ItemId;
            this.Name = itemView.Name;
            this.PermanentCredits = itemView.PermanentCredits;
            this.PointsPerDay = itemView.PointsPerDay;
            this.PurchaseType = itemView.PurchaseType;
            this.TypeId = itemView.TypeId;
            this.PackOneAmount = itemView.PackOneAmount;
            this.PackTwoAmount = itemView.PackTwoAmount;
            this.PackThreeAmount = itemView.PackThreeAmount;
            this.MaximumOwnableAmount = itemView.MaximumOwnableAmount;
            this.Enable1Day = itemView.Enable1Day;
            this.Enable7Days = itemView.Enable7Days;
            this.Enable30Days = itemView.Enable30Days;
            this.Enable90Days = itemView.Enable90Days;
            this.MaximumDurationDays = itemView.MaximumDurationDays;
            this.PermanentPoints = itemView.PermanentPoints;
            this.IsDisable = itemView.IsDisable;
            this.CustomProperties = itemView.CustomProperties != null ? new Dictionary<string, string>(itemView.CustomProperties) : new Dictionary<string, string>();
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            StringBuilder display = new StringBuilder();

            display.Append("[ItemView: [AmountRemaining: ");
            display.Append(this.AmountRemaining);
            display.Append("][ClassId: ");
            display.Append(this.ClassId);
            display.Append("][CreditsPerDayShop: ");
            display.Append(this.CreditsPerDay);
            display.Append("][Description: ");
            display.Append(this.Description);
            display.Append("][IsFeatured: ");
            display.Append(this.IsFeatured);
            display.Append("][IsForSale: ");
            display.Append(this.IsForSale);
            display.Append("][IsNew: ");
            display.Append(this.IsNew);
            display.Append("][IsPopular: ");
            display.Append(this.IsPopular);
            display.Append("][ItemId: ");
            display.Append(this.ItemId);
            display.Append("][Name: ");
            display.Append(this.Name);
            display.Append("][PermanentCredits: ");
            display.Append(this.PermanentCredits);
            display.Append("][PointsPerDayShop: ");
            display.Append(this.PointsPerDay);
            display.Append("][PurchaseType: ");
            display.Append(this.PurchaseType);
            display.Append("][TypeId: ");
            display.Append(this.TypeId);
            display.Append("][PackOneAmount: ");
            display.Append(this.PackOneAmount);
            display.Append("][PackTwoAmount: ");
            display.Append(this.PackTwoAmount);
            display.Append("][PackThreeAmount: ");
            display.Append(this.PackThreeAmount);
            display.Append("][MaximumOwnableAmount: ");
            display.Append(this.MaximumOwnableAmount);
            display.Append("][Enable1Day: ");
            display.Append(this.Enable1Day);
            display.Append("][Enable7Days: ");
            display.Append(this.Enable7Days);
            display.Append("][Enable30Days: ");
            display.Append(this.Enable30Days);
            display.Append("][Enable90Days: ");
            display.Append(this.Enable90Days);
            display.Append("][MaximumDurationDays: ");
            display.Append(this.MaximumDurationDays);
            display.Append("][PermanentPoints: ");
            display.Append(this.PermanentPoints);
            display.Append("][IsDisable: ");
            display.Append(this.IsDisable);
            display.Append("]]");

            return display.ToString();
        }

        #endregion Methods
    }
}
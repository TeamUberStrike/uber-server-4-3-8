using System.Collections.Generic;

namespace Cmune.DataCenter.Common.Entities
{
    [System.Serializable]
    public class PackageView
    {
        #region Properties

        /// <summary>
        /// Bonus in % (ie a value of 10 indicates a 10% bonus)
        /// </summary>
        
        public int Bonus { get; set; }
        /// <summary>
        /// Price in US$
        /// </summary>
        
        public decimal Price { get; set; }
        
        public List<int> Items { get; set; }
        
        public string Name { get; set; }

        #endregion Properties

        #region Constructors

        public PackageView()
        {
            this.Bonus = 0;
            this.Price = 0;
            this.Items = new List<int>();
            this.Name = string.Empty;
        }

        public PackageView(int bonus, decimal price, List<int> items, string name)
        {
            this.Bonus = bonus;
            this.Price = price;
            this.Items = items;
            this.Name = name;
        }

        #endregion Constructors
    }
}
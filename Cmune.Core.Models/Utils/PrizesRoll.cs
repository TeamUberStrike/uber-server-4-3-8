using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cmune.DataCenter.Common.Utils
{
    public class PrizeElement
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; }
    }


    public static class PrizesRoll
    {
        private static Random random = new Random();
        
        public static PrizeElement CaculateSum(List<PrizeElement> prizes)
        {
            int sum_of_weight = 0;
            for (int i = 0; i < prizes.Count; i++)
            {
                sum_of_weight += prizes[i].Weight;
            }
            int rnd = random.Next(0, sum_of_weight);
            for (int i = 0; i < prizes.Count; i++)
            {
                if (rnd < prizes[i].Weight)
                    return prizes.ElementAt(i);
                rnd -= prizes[i].Weight;
            }
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cmune.DataCenter.Common.Utils
{
    public static class PrizesRoll
    {
        private static Random random = new Random();

        /// <summary>
        /// Pick count elements randomly according to their weight
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<PrizeElementView> PickElements(List<PrizeElementView> elements, int count)
        {
            if (count > elements.Count)
                throw new ArgumentException(String.Format("There are only {0} elements in the list and you want to pick up {1} elements", elements.Count, count), "count");

            var wins = new List<PrizeElementView>(count);
            List<PrizeElementView> localElements = elements.ConvertAll(q => q);

            for (int i = 0; i < count; i++)
            {
                Shuffle(localElements);
                int weightMax = localElements.Max(e => e.Weight);
                int randomWeight = random.Next(1, weightMax);
                var win = localElements.First(e => e.Weight >= randomWeight);
                localElements.Remove(win);
                wins.Add(win);
            }

            return wins;
        }

        /// <summary>
        /// TODO: to move to a helper
        /// </summary>
        /// <typeparam name="T">List element type.</typeparam>
        /// <param name="list">List to shuffle.</param>
        private static void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count; i > 1; i--)
            {
                // Pick random element to swap.
                int j = random.Next(i); // 0 <= j <= i-1
                // Swap.
                T tmp = list[j];
                list[j] = list[i - 1];
                list[i - 1] = tmp;
            }
        }
    }
}

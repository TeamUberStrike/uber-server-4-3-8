using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cmune.DataCenter.Utils
{
    public static class MathematicNaturalLogarithm
    {
        public static decimal GetNaturalLogarithmicAlpha(int y, int x)
        {
            return ((decimal) y) / ((decimal) Math.Log(x));
        }

        public static int CalculateY(decimal alpha, int x)
        {
            return (int) (alpha * (decimal)Math.Log(x));
        }
    }
}
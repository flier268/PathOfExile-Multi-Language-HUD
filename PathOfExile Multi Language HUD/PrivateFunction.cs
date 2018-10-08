using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfExile_Multi_Language_HUD
{
    public class PrivateFunction
    {
        public static double CalcDifference_Rate(string A, string B)
        {
            var AArray = A.Split(' ');
            var BArray = B.Split(' ');
            return AArray.Except(BArray).Count() + Math.Abs(A.Length - B.Length) / Math.Max(AArray.Length, BArray.Length);
        }
    }
}

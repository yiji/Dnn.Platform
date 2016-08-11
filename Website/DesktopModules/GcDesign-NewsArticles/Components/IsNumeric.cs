using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GcDesign.NewsArticles
{
    public class Numeric
    {
        public static bool IsNumeric(string expression)
        {
            bool isNum;
            double retNum;
            isNum = double.TryParse(expression, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }
    }
}
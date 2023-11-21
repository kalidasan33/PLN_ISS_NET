using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Kendo.Mvc;
using Kendo.Mvc.UI.Fluent;
using Kendo.Mvc.UI;

namespace ISS.Web.Helpers
{
    public class FilterHelper
    {
        public static void FilterString(StringOperatorsBuilder str)
        {
            str.Clear();
            str.StartsWith("Begins With");
            str.Contains("Contains");
            str.DoesNotContain("Does Not Contain");
            str.IsEqualTo("Equal To");          
            str.IsNotEqualTo("Not Equal To");
            str.EndsWith("Ends With");

        }

        public static void FilterNumber(NumberOperatorsBuilder num)
        {
            num.Clear();
            num.IsEqualTo("Equal To");
            num.IsGreaterThanOrEqualTo("Greater Than or Equal To");
            //num.IsLessThan("Less Than");
            num.IsLessThanOrEqualTo("Less Than or Equal To");
            num.IsNotEqualTo("Not Equal To");

    
        }

        public static void FilterDate(DateOperatorsBuilder dte)
        {
            dte.Clear();
            dte.IsEqualTo("Equal To");
            dte.IsGreaterThan("After");
            dte.IsGreaterThanOrEqualTo("After or Equal To");
            dte.IsLessThan("Before");
            dte.IsLessThanOrEqualTo("Before or Equal To");

        }
    }
}
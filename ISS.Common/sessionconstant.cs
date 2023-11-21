using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS.Common
{
    public class SessionConstant
    {
        public static readonly string OUT_PUT_MESSAGE;
        public static readonly string TEXTILE_ALLOCATIONS;
        public static readonly string FROM_DATE;

        public static readonly string MENU_ITEMS_AUTH;
        public static readonly string REQ_DETAILS;

        static SessionConstant()
        {
            string _systemPrefix = "__sys__";
            string _systemSuffix = "__sys__";
            string format = "__sys__{0}__sys__";

            OUT_PUT_MESSAGE = _systemPrefix + "OUT_PUT_MESSAGE" + _systemSuffix;
            TEXTILE_ALLOCATIONS = string.Format(format, "TEXTILE_ALLOCATIONS");
            FROM_DATE = string.Format(format, "FROM_DATE");
            MENU_ITEMS_AUTH = string.Format(format, "MENU_ITEMS_AUTH");
            REQ_DETAILS = string.Format(format, "REQ_DETAILS");
        }
    }
}

using ISS.Common;
using ISS.Core.Model.Textiles;
using System;
using System.Collections.Generic;
using System.Web;
using ISS.Core.Model.Order;

namespace ISS.Web.Helpers
{
    public class SessionHelper
    {

        /// <summary>
        /// Set in Session
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Set<T>(string key, T value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new Exception("Session key is empty!");

            HttpContext.Current.Session[key] = value;
        }

        /// <summary>
        /// Get From Session
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            if (HttpContext.Current.Session[key] == null)
                return default(T);
            else
                return (T)HttpContext.Current.Session[key];
        }

        /// <summary>
        /// Clear From Session
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static void Clear(string key)
        {
            if (HttpContext.Current.Session[key] != null)
                HttpContext.Current.Session[key] = null;
        }

        public static string OutputMessage
        {
            set { Set<string>(SessionConstant.OUT_PUT_MESSAGE, value); }
            get { return Get<string>(SessionConstant.OUT_PUT_MESSAGE); }
        }

        public static IList<TextileAllocation> Textiles
        {
            set { Set<IList<TextileAllocation>>(SessionConstant.TEXTILE_ALLOCATIONS, value); }
            get { return Get<IList<TextileAllocation>>(SessionConstant.TEXTILE_ALLOCATIONS); }
        }

        public static string FROMDATE
        {
            set { Set<string>(SessionConstant.FROM_DATE, value); }
            get { return Get<string>(SessionConstant.FROM_DATE); }
        }

        public static List<RequisitionOrder> RequisitionOrder
        {
            set { Set<List<RequisitionOrder>>(SessionConstant.REQ_DETAILS, value); }
            get { return Get<List<RequisitionOrder>>(SessionConstant.REQ_DETAILS); }
        }
    }
}
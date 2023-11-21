using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace ISS.Common
{
    public static class ISSExtentions
    {
        public static String GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());

            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static Decimal RoundCustom(this Decimal value,int decimals)
        {            
            return decimal.Round(value  , decimals, MidpointRounding.AwayFromZero);
        }

        public static Decimal ConvertDzToEaches(this Decimal Qty)
        {
            var dz = (int)(Qty / 1);
            var Eaches = Qty % 1;
            return dz * LOVConstants.Dozen + Eaches * 100; //
        }

        public static Decimal ConvertEachesToDz(this Decimal Qty)
        {
            var dz = (int)(Qty / LOVConstants.Dozen);
            var Eaches = (Qty % LOVConstants.Dozen)/100;
            return dz + Eaches  ; //
        }

        public static T AsObject<T>(this XmlElement ele) where T : class
        {
            XmlNodeReader r = new XmlNodeReader(ele);
            XmlSerializer xs = new XmlSerializer(typeof(T), new XmlRootAttribute(ele.Name));

            return xs.Deserialize(r) as T;
        }

        public static string ToNumberString(this decimal value)
        {
            return value.ToString("n0");
        }

        public static IEnumerable<string> SplitByLength(this string stringToSplit, int maximumLineLength)
        {
            var words = stringToSplit.Split(' ').Concat(new[] { "" });
            return
                words
                    .Skip(1)
                    .Aggregate(
                        words.Take(1).ToList(),
                        (a, w) =>
                        {
                            var last = a.Last();
                            while (last.Length > maximumLineLength)
                            {
                                a[a.Count() - 1] = last.Substring(0, maximumLineLength);
                                last = last.Substring(maximumLineLength);
                                a.Add(last);
                            }
                            var test = last + " " + w;
                            if (test.Length > maximumLineLength)
                            {
                                a.Add(w);
                            }
                            else
                            {
                                a[a.Count() - 1] = test;
                            }
                            return a;
                        });

            
        }

        /// <summary>
        /// Html Encodes the <paramref name="text"/> 
        /// </summary>
        /// <param name="text">Text to be encoded</param>
        /// <returns></returns>
        public static string HtmlEncode(this string text)
        {
            return System.Web.HttpContext.Current.Server.HtmlEncode(text);
        }

        

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS.Common.Utility
{
    public class SequenceGenerator
    {

        public static string CurrentValue(List<string> LineNumList)
        {
            return LineNumList.Where(s => s.ToCharArray().All(n => Char.IsLetter(n))).Max();

        }
        public static string NextValue(string currVal)
        {
            if (String.IsNullOrEmpty(currVal) ||currVal.Length<2)
            {
                return "AA";
            }

            string nextValue = String.Empty;
            char prefix = Convert.ToChar(currVal.Substring(0, 1).ToUpper());
            char suffix = Convert.ToChar(currVal.Substring(1, 1).ToUpper());

            if (currVal.ToUpper() == "ZZ")
            {
                nextValue = "Unable to generate";

            }
            else
            {
                if (suffix == 'Z')
                {
                    prefix = Convert.ToChar(prefix + 1);

                    suffix = Convert.ToChar(65);
                }
                else
                {

                    suffix = Convert.ToChar(suffix + 1);

                }

                nextValue += Convert.ToChar(prefix).ToString() + Convert.ToChar(suffix).ToString();
            }


            return nextValue;
        }


    }
}

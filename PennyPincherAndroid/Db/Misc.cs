using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;


    class Misc
    {
        public static Decimal Val(object x)
        {
            string s = Convert.ToString(x);
            s = s.Replace(",", "");
            s = s.Replace("$", "");
            Decimal d;
            if (Decimal.TryParse(s, out d))
            {
                return d;
            }
            else return 0;

        }

        public static Decimal ValSum(string s)
        {
            StringBuilder b = new StringBuilder(s);
            b.Replace(":", ";");
            b.Replace("-", ";-");
            b.Replace("=", ";");
            b.Replace("\r", ";");
            b.Replace("\n", ";");
            var items = b.ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            decimal sum = 0;
            for (var i = 0; i < items.Length; i++)
            {
                sum += Val(items[i].Trim());
            }
            return sum;
        }
    }

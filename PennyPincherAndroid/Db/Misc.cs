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
        public static Decimal Val(string s)
        {
            s = s.Replace(",", "");
            s = s.Replace("$", "");

            Decimal d;
            if (Decimal.TryParse(s, out d))
            {
                return d;
            }
            else return 0;

        }
    }

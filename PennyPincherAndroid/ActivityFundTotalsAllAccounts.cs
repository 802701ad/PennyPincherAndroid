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

namespace PennyPincher
{
    [Activity(Label = "Fund Totals for All Accounts")]
    public class ActivityFundTotalsAllAccounts : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.TotalsReport);
            var scrollview = FindViewById<ScrollView>(Resource.Id.scrollview);

            var t = new TableLayout(this);
            t.StretchAllColumns = true;
            foreach (Fund f in Db.getFunds())
            {
                var tr = new TableRow(this);
                var tdFundName = new TextView(this);
                tdFundName.Text = f.fund_name;
                tdFundName.Tag = f.fund_id;

                tr.AddView(tdFundName);

                var tdAmount = new TextView(this);
                tdAmount.Tag = f.fund_id;
                tdAmount.Text = String.Format("{0:C}", Db.getFundTotal(fund_id: f.fund_id));
                tdAmount.Gravity = GravityFlags.Right;
                tr.AddView(tdAmount);
              
                t.AddView(tr);
            }
            scrollview.AddView(t);
        }

    }
}
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
    [Activity(Label = "Fund Totals")]
    public class ActivityFundTotalsInAccount : Activity
    {
        protected string account_id;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            account_id = Intent.GetStringExtra("account_id");
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
                tdFundName.Click += Fund_Click;

                tr.AddView(tdFundName);

                var tdAmount = new TextView(this);
                tdAmount.Tag = f.fund_id;
                tdAmount.Click += Fund_Click;
                tdAmount.Text = String.Format("{0:C}", Db.getFundTotal(account_id: account_id, fund_id: f.fund_id));
                tdAmount.Gravity = GravityFlags.Right;
                tr.AddView(tdAmount);
              
                t.AddView(tr);
            }
            scrollview.AddView(t);
        }

        public void Fund_Click(object sender, EventArgs e)
        {
            var i = new Intent(this, typeof(ActivityFundActivityInAccount));
            i.PutExtra("fund_id", Convert.ToString((sender as View).Tag));
            i.PutExtra("account_id", account_id);
            StartActivityForResult(i, 0);
        }
    }
}
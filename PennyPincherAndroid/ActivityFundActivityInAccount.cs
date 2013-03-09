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
    [Activity(Label = "Fund Activity")]
    public class ActivityFundActivityInAccount : Activity
    {
        protected string account_id;
        protected string fund_id;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            account_id = Intent.GetStringExtra("account_id");
            fund_id = Intent.GetStringExtra("fund_id");
            SetContentView(Resource.Layout.TotalsReport);
            var scrollview = FindViewById<ScrollView>(Resource.Id.scrollview);
            this.Title += " - " + Db.getFund(fund_id).fund_name;
            var t = new TableLayout(this);
            t.StretchAllColumns = true;
            foreach (TransactionMain m in Db.getTransactions(account_id, DateTime.Now.AddYears(-5).ToShortDateString(), DateTime.Now.AddYears(5).ToShortDateString()))
            {
                var amount = Db.getTransactionDetailAmount(m.transaction_id, fund_id);
                if (amount != 0)
                {
                    var tr = new TableRow(this);
                    tr.Tag = m.transaction_id;
                    tr.Click += Transaction_Click;
                    var td = new TextView(this);
                    td.Text = m.transaction_title;
                    tr.AddView(td);
                    td = new TextView(this);
                    td.Text = m.transaction_date.ToShortDateString();
                    td.Gravity = GravityFlags.CenterHorizontal;
                    if (m.is_active != "1") td.SetTextColor(Android.Graphics.Color.DarkGray);
                    tr.AddView(td);
                    td = new TextView(this);
                    td.Text = String.Format("{0:C}", amount);
                    td.Gravity = GravityFlags.Right;
                    if (m.is_active != "1") td.SetTextColor(Android.Graphics.Color.DarkGray);
                    tr.AddView(td);
                    t.AddView(tr);
                }
            }
            scrollview.AddView(t);
        }

        public void Transaction_Click(object sender, EventArgs e)
        {
            var i = new Intent(this, typeof(ActivityTransactionEdit));
            i.PutExtra("transaction_id", Convert.ToString((sender as View).Tag));
            i.PutExtra("account_id", account_id);
            StartActivityForResult(i, 0);
        }

    }
}
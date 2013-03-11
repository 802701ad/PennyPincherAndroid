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
    [Activity(Label = "Transactions", MainLauncher=false)]
    public class ActivityTransactions : Activity
    {

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Transactions);

            FindViewById<Button>(Resource.Id.btnAdd).Click += btnAdd_Click;
            FindViewById<Button>(Resource.Id.btnFunds).Click += btnFunds_Click;
            FindViewById<Button>(Resource.Id.btnRefresh).Click += btnRefresh_Click;

            hScroll = FindViewById<ScrollView>(Resource.Id.hScroll);
			holder = FindViewById<LinearLayout>(Resource.Id.hldr);
            dpFromDate = FindViewById<DatePicker>(Resource.Id.dpFromDate);
            dpToDate = FindViewById<DatePicker>(Resource.Id.dpToDate);
            var From=DateTime.Now.AddMonths(-1);
            dpFromDate.UpdateDate(From.Year, From.Month, From.Day);
            var To=DateTime.Now.AddYears(1);
            dpToDate.UpdateDate(To.Year, To.Month, To.Day);
            account_id = Intent.GetStringExtra("account_id");
        }

        protected DatePicker dpFromDate;
        protected DatePicker dpToDate;
        protected string account_id;
        public ScrollView hScroll;
		public LinearLayout holder;
        public void Refresh()
        {
			holder.RemoveAllViews();
            FindViewById<TextView>(Resource.Id.tvAccountTotal).Text=String.Format("{0:C}", Db.getAccountTotal(account_id));
            var t = new TableLayout(this);
            t.StretchAllColumns = true;

            var From=dpFromDate.Month + "/" + dpFromDate.DayOfMonth + "/" + dpFromDate.Year;
            var To=dpToDate.Month + "/" + dpToDate.DayOfMonth + "/" + dpToDate.Year;
			Android.Util.Log.Info("Transaction Count:",Db.getTransactions (account_id, From, To).Count.ToString() + " - " + From+"~"+To);
			foreach (TransactionMain a in Db.getTransactions(account_id, From, To))
            {
                var tr = new TableRow(this);
                {
                    var b = new TextView(this);
                    b.Text = a.transaction_title;
                    b.Tag = a.transaction_id;
                    b.Click += Transaction_Click;
                    b.Gravity = GravityFlags.DisplayClipHorizontal;
                    tr.AddView(b);
                }
                {
                    var b = new TextView(this);
                    b.Text = a.transaction_date.ToString("MMM d");
                    b.Tag = a.transaction_id;
                    b.Click += Transaction_Click;
                    b.Gravity = GravityFlags.CenterHorizontal;
                    tr.AddView(b);
                }
                {
                    var b = new TextView(this);
                    b.Text = String.Format("{0:C}", a.amount);
                    b.Tag = a.transaction_id;
                    b.Click += Transaction_Click;
                    b.Gravity = GravityFlags.Right;
                    tr.AddView(b);
                }
                t.AddView(tr);
            }
			holder.AddView(t);

            {
                var lblAccountInfo = FindViewById<TextView>(Resource.Id.lblAccountInfo);
                lblAccountInfo.Text = Db.getAccount(account_id).account_name;
                lblAccountInfo.Click += Account_Click;
            }

        }

        public void btnRefresh_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        public void btnFunds_Click(object sender, EventArgs e)
        {
            var i = new Intent(this, typeof(ActivityFundTotalsInAccount));
            i.PutExtra("account_id", account_id);
            StartActivity(i);
        }

        public void btnAdd_Click(object sender, EventArgs e)
        {
            var i = new Intent(this, typeof(ActivityTransactionEdit));
            i.PutExtra("transaction_id", "");
            i.PutExtra("account_id", account_id);
            StartActivityForResult(i, 0);
        }

        public void Transaction_Click(object sender, EventArgs e)
        {
            var i = new Intent(this, typeof(ActivityTransactionEdit));
            i.PutExtra("transaction_id", Convert.ToString((sender as View).Tag));
            i.PutExtra("account_id", account_id);
            StartActivityForResult(i, 0);
        }

        public void Account_Click(object sender, EventArgs e)
        {
            var i = new Intent(this, typeof(ActivityAccountEdit));
            i.PutExtra("account_id", account_id);
            StartActivityForResult(i, 0);
        }
        protected override void OnResume()
        {
            base.OnResume();
            Refresh();
        }

     
             
    }
}
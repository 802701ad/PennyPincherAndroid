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
            hScroll = FindViewById<HorizontalScrollView>(Resource.Id.hScroll);
            txtFromDate = FindViewById<EditText>(Resource.Id.txtFromDate);
            txtToDate = FindViewById<EditText>(Resource.Id.txtToDate);
            account_id = Intent.GetStringExtra("account_id");
            {
                var lblAccountInfo = FindViewById<TextView>(Resource.Id.lblAccountInfo);
                lblAccountInfo.Text = Db.getAccount(account_id).account_name;
                lblAccountInfo.Click += Account_Click;
            }
        }

        protected EditText txtFromDate;
        protected EditText txtToDate;
        protected string account_id;
        public HorizontalScrollView hScroll;
        public void Refresh()
        {
            hScroll.RemoveAllViews();
            var t = new TableLayout(this);
            t.StretchAllColumns = true;
            foreach (TransactionMain a in Db.getTransactions(account_id, txtFromDate.Text, txtFromDate.Text))
            {
                var tr = new TableRow(this);
                {
                    var b = new TextView(this);
                    b.Text = a.transaction_title;
                    b.Tag = a.transaction_id;
                    b.Click += Transaction_Click;
                    tr.AddView(b);
                }
                {
                    var b = new TextView(this);
                    b.Text = a.transaction_date.ToShortDateString();
                    b.Tag = a.transaction_id;
                    b.Click += Transaction_Click;
                    tr.AddView(b);
                }
                {
                    var b = new TextView(this);
                    b.Text = String.Format("{0:C}", a.amount);
                    b.Tag = a.transaction_id;
                    b.Click += Transaction_Click;
                    tr.AddView(b);
                }
                t.AddView(tr);
            }
            hScroll.AddView(t);
        }

        public void btnFunds_Click(object sender, EventArgs e)
        {
            //var i = new Intent(this, typeof(ActivityFundList));
            //StartActivity(i);
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
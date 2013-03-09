using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace PennyPincher
{
    [Activity(Label = "PennyPincher", MainLauncher = true, Icon = "@drawable/penny")]//drawable/icon
    public class ActivityMain : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            FindViewById<Button>(Resource.Id.btnAdd).Click += btnAdd_Click;
            FindViewById<Button>(Resource.Id.btnFunds).Click += btnFunds_Click;
            hScroll = FindViewById<ScrollView>(Resource.Id.hScroll);
            Refresh();
        }


        public ScrollView hScroll;
        public void Refresh()
        {
            hScroll.RemoveAllViews();
            var t = new TableLayout(this);
            foreach (Account a in Db.getAccounts())
            {
                var tr = new TableRow(this);
                var b = new Button(this);
                b.Text = a.account_name;
                b.Tag = a.account_id;
                b.Click += Account_Click;
                tr.AddView(b);
                t.AddView(tr);
            }
            hScroll.AddView(t);
        }

        public void btnFunds_Click(object sender, EventArgs e)
        {
            var i = new Intent(this, typeof(ActivityFundList));
            StartActivity(i);
        }

        public void btnAdd_Click(object sender, EventArgs e)
        {
            var i = new Intent(this, typeof(ActivityAccountEdit));
            i.PutExtra("account_id", "");
            StartActivityForResult(i,0);
        }

        public void Account_Click(object sender, EventArgs e)
        {
            var i = new Intent(this, typeof(ActivityTransactions));
            i.PutExtra("account_id", Convert.ToString((sender as Button).Tag));
            StartActivityForResult(i,0);
        }

        protected override void OnResume()
        {
            base.OnResume();
            Refresh();
        }
        protected void OnActivityResult(int RequestCode, int ResultCode, Intent intent)
        {
            //This whole method doesn't work for some reason.
            //Refresh();
        }
    }
}


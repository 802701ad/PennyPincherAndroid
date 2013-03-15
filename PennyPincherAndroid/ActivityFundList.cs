using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace PennyPincher
{
    [Activity(Label = "PennyPincher - Fund List", MainLauncher = false, Icon = "@drawable/icon")]
    public class ActivityFundList : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Funds);

            FindViewById<Button>(Resource.Id.btnAdd).Click += btnAdd_Click;
            FindViewById<Button>(Resource.Id.btnFundTotalsAllAccounts).Click += btnViewTotals_Click;
            
            hScroll = FindViewById<ScrollView>(Resource.Id.hScroll);
            Refresh();
        }


        public ScrollView hScroll;
        public void Refresh()
        {
            hScroll.RemoveAllViews();
            var t = new TableLayout(this);
            foreach (Fund a in Db.getFunds())
            {
                var tr = new TableRow(this);
                var b = new Button(this);
                b.Text = a.fund_name;
                b.Tag = a.fund_id;
                b.Click += Item_Click;
                tr.AddView(b);
                t.AddView(tr);
            }
            hScroll.AddView(t);
        }

        public void btnAdd_Click(object sender, EventArgs e)
        {
            var i = new Intent(this, typeof(ActivityFundEdit));
            i.PutExtra("fund_id", "");
            StartActivityForResult(i,0);
        }

        public void btnViewTotals_Click(object sender, EventArgs e)
        {
            var i = new Intent(this, typeof(ActivityFundTotalsAllAccounts));
            StartActivityForResult(i, 0);
        }

        public void Item_Click(object sender, EventArgs e)
        {
            var i = new Intent(this, typeof(ActivityFundEdit));
            i.PutExtra("fund_id", Convert.ToString((sender as Button).Tag));
            StartActivity(i);
        }

        protected override void OnResume()
        {
            base.OnResume();
            Refresh();
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu.Add(0, 1, 0, System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)));
            return true;
        }
    }
}

